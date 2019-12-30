using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Extension.Event;
using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core.Media.Animation.EventData;
using Rogue.NET.Core.Media.Animation.Interface;
using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Content.Doodad;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Scenario.Content.ViewModel.LevelCanvas.Inteface;
using Rogue.NET.Scenario.Processing.Service.Interface;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Rogue.NET.Scenario.Content.ViewModel.LevelCanvas
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(ILevelCanvasViewModel))]
    public class LevelCanvasViewModel : NotifyViewModel, ILevelCanvasViewModel
    {
        readonly IScenarioUIService _scenarioUIService;

        // Targeting animation (singular)
        IAnimationPlayer _targetAnimationPlayer;

        // Layers
        DrawingImage[,] _visibleLayer;
        DrawingImage[,] _exploredLayer;
        DrawingImage[,] _revealedLayer;

        // Opacity Masks
        DrawingBrush _exploredDrawingBrush;
        DrawingBrush _revealedDrawingBrush;
        DrawingBrush _visibleDrawingBrush;

        // Player (separate from contents)
        LevelCanvasImage _player;

        public event SimpleEventHandler LayoutUpdated;

        [ImportingConstructor]
        public LevelCanvasViewModel(IScenarioUIService scenarioUIService)
        {
            _scenarioUIService = scenarioUIService;

            this.Animations = new ObservableCollection<FrameworkElement>();
            this.Auras = new ObservableCollection<LevelCanvasShape>();
            this.Doodads = new ObservableCollection<LevelCanvasImage>();
            this.Items = new ObservableCollection<LevelCanvasImage>();
            this.Characters = new ObservableCollection<LevelCanvasImage>();
            this.LightRadii = new ObservableCollection<LevelCanvasShape>();

            this.ExploredOpacityMask = new DrawingBrush();
            this.RevealedOpacityMask = new DrawingBrush();
            this.VisibleOpacityMask = new DrawingBrush();

            RenderOptions.SetBitmapScalingMode(this.ExploredOpacityMask, BitmapScalingMode.LowQuality);
            RenderOptions.SetBitmapScalingMode(this.RevealedOpacityMask, BitmapScalingMode.LowQuality);
            RenderOptions.SetBitmapScalingMode(this.VisibleOpacityMask, BitmapScalingMode.LowQuality);

            RenderOptions.SetCachingHint(this.ExploredOpacityMask, CachingHint.Cache);
            RenderOptions.SetCachingHint(this.RevealedOpacityMask, CachingHint.Cache);
            RenderOptions.SetCachingHint(this.VisibleOpacityMask, CachingHint.Cache);

            this.ExploredOpacityMask.ViewboxUnits = BrushMappingMode.Absolute;
            this.RevealedOpacityMask.ViewboxUnits = BrushMappingMode.Absolute;
            this.VisibleOpacityMask.ViewboxUnits = BrushMappingMode.Absolute;

            this.ExploredOpacityMask.ViewportUnits = BrushMappingMode.Absolute;
            this.RevealedOpacityMask.ViewportUnits = BrushMappingMode.Absolute;
            this.VisibleOpacityMask.ViewportUnits = BrushMappingMode.Absolute;
        }

        #region (public) Properties
        public DrawingImage[,] VisibleLayer
        {
            get { return _visibleLayer; }
            set { this.RaiseAndSetIfChanged(ref _visibleLayer, value); }
        }
        public DrawingImage[,] ExploredLayer
        {
            get { return _exploredLayer; }
            set { this.RaiseAndSetIfChanged(ref _exploredLayer, value); }
        }
        public DrawingImage[,] RevealedLayer
        {
            get { return _revealedLayer; }
            set { this.RaiseAndSetIfChanged(ref _revealedLayer, value); }
        }
        public DrawingBrush ExploredOpacityMask
        {
            get { return _exploredDrawingBrush; }
            set { this.RaiseAndSetIfChanged(ref _exploredDrawingBrush, value); }
        }
        public DrawingBrush RevealedOpacityMask
        {
            get { return _revealedDrawingBrush; }
            set { this.RaiseAndSetIfChanged(ref _revealedDrawingBrush, value); }
        }
        public DrawingBrush VisibleOpacityMask
        {
            get { return _visibleDrawingBrush; }
            set { this.RaiseAndSetIfChanged(ref _visibleDrawingBrush, value); }
        }
        public ObservableCollection<LevelCanvasShape> LightRadii { get; set; }
        public ObservableCollection<LevelCanvasShape> Auras { get; set; }
        public ObservableCollection<LevelCanvasImage> Doodads { get; set; }
        public ObservableCollection<LevelCanvasImage> Items { get; set; }
        public ObservableCollection<LevelCanvasImage> Characters { get; set; }
        public ObservableCollection<FrameworkElement> Animations { get; set; }
        public LevelCanvasImage Player
        {
            get { return _player; }
            set { this.RaiseAndSetIfChanged(ref _player, value); }
        }
        #endregion

        #region (public) Update Methods
        /// <summary>
        /// Draws entire layout and applies visibility
        /// </summary>
        public void UpdateLayout()
        {
            // Re-draw the layout layers
            this.VisibleLayer = new DrawingImage[_scenarioUIService.LevelWidth, _scenarioUIService.LevelHeight];
            this.ExploredLayer = new DrawingImage[_scenarioUIService.LevelWidth, _scenarioUIService.LevelHeight];
            this.RevealedLayer = new DrawingImage[_scenarioUIService.LevelWidth, _scenarioUIService.LevelHeight];

            // Create DrawingImage instances from the resource cache
            //
            _scenarioUIService.CreateLayoutDrawings(this.VisibleLayer, this.ExploredLayer, this.RevealedLayer);

            OnPropertyChanged(() => this.VisibleLayer);
            OnPropertyChanged(() => this.ExploredLayer);
            OnPropertyChanged(() => this.RevealedLayer);

            OnLevelDimensionChange();

            if (this.LayoutUpdated != null)
                this.LayoutUpdated();
        }

        public void UpdateContent(IEnumerable<ScenarioObject> contents, Player player)
        {
            // Remove (Filter out anything that isn't in the level)
            this.Doodads.Filter(x => contents.None(z => z.Id == x.ScenarioObjectId));
            this.Characters.Filter(x => contents.None(z => z.Id == x.ScenarioObjectId) && x.ScenarioObjectId != player.Id);
            this.Items.Filter(x => contents.None(z => z.Id == x.ScenarioObjectId));
            this.LightRadii.Filter(x => contents.None(z => z.Id == x.ScenarioObjectId) && x.ScenarioObjectId != player.Id);
            this.Auras.Filter(x => contents.None(z => z.Id == x.ScenarioObjectId) && x.ScenarioObjectId != player.Id);

            // Update / Add
            foreach (var scenarioObject in contents)
            {
                // TODO: REFACTOR. PLAYER IS NOW A PART OF THE LEVEL CONTENT
                if (scenarioObject is Player)
                    continue;

                // Characters
                if (scenarioObject is Character)
                {
                    // Character
                    var character = scenarioObject as Character;
                    var characterViewModel = this.Characters.FirstOrDefault(x => x.ScenarioObjectId == scenarioObject.Id);

                    // Update Content
                    if (characterViewModel != null)
                        _scenarioUIService.UpdateContent(characterViewModel, scenarioObject);

                    else
                        this.Characters.Add(CreateContent(scenarioObject));

                    // Check for Character Light Radius
                    if (character.SymbolType == SymbolType.Smiley)
                    {
                        var characterLightRadius = this.LightRadii.FirstOrDefault(x => x.ScenarioObjectId == character.Id);

                        // Update Light Radius
                        if (characterLightRadius != null)
                            _scenarioUIService.UpdateLightRadius(characterLightRadius, character, new Rect(0, 0, _scenarioUIService.LevelUIWidth, _scenarioUIService.LevelUIHeight));

                        // Add Light Radius
                        else
                            this.LightRadii.Add(CreateLightRadius(character));
                    }

                    // Auras
                    var characterAuras = character.Alteration.GetAuraSourceParameters();
                    var auraUpdates = this.Auras.Where(x => characterAuras.Select(z => z.Item1).Contains(x.Id));
                    var auraAdditions = characterAuras.Where(x => !auraUpdates.Any(z => z.Id == x.Item1));

                    // Update Auras
                    foreach (var aura in auraUpdates)
                        _scenarioUIService.UpdateAura(aura, characterAuras.First(x => x.Item1 == aura.Id).Item2.AuraColor,
                                                      characterAuras.First(x => x.Item1 == aura.Id).Item2.AuraRange,
                                                      character,
                                                      new Rect(0, 0, _scenarioUIService.LevelUIWidth, _scenarioUIService.LevelUIHeight));

                    // Add Auras
                    foreach (var aura in auraAdditions)
                        this.Auras.Add(CreateAura(character, aura.Item1, aura.Item2.AuraColor, aura.Item2.AuraRange));

                    // Remove Auras*** This has to be checked because there may be characters that have had their
                    //                 equipment removed
                    this.Auras.Filter(x => x.ScenarioObjectId == character.Id &&
                                           !characterAuras.Any(z => z.Item1 == x.Id));
                }

                // Items
                if (scenarioObject is ItemBase)
                {
                    var item = scenarioObject as ItemBase;
                    var itemViewModel = this.Items.FirstOrDefault(x => x.ScenarioObjectId == item.Id);

                    // Update Content
                    if (itemViewModel != null)
                        _scenarioUIService.UpdateContent(itemViewModel, scenarioObject);

                    else
                        this.Items.Add(CreateContent(scenarioObject));
                }

                // Doodads
                else if (scenarioObject is DoodadBase)
                {
                    var doodad = scenarioObject as DoodadBase;
                    var doodadViewModel = this.Doodads.FirstOrDefault(x => x.ScenarioObjectId == doodad.Id);

                    // Update Content
                    if (doodadViewModel != null)
                        _scenarioUIService.UpdateContent(doodadViewModel, scenarioObject);

                    else
                        this.Doodads.Add(CreateContent(scenarioObject));
                }
            }

            // Player Update
            var lightRadius = this.LightRadii.FirstOrDefault(x => x.ScenarioObjectId == player.Id);

            // Update
            if (this.Player == null)
                this.Player = new LevelCanvasImage(player.Id);

            _scenarioUIService.UpdateContent(this.Player, player);

            // Update Light Radius
            if (lightRadius != null)
                _scenarioUIService.UpdateLightRadius(lightRadius, player, new Rect(0, 0, _scenarioUIService.LevelUIWidth, _scenarioUIService.LevelUIHeight));

            // Add Light Radius
            else
                this.LightRadii.Add(CreateLightRadius(player));
        }

        /// <summary>
        /// Draws visibility visual used as an opacity mask for the level
        /// </summary>
        public void UpdateLayoutVisibility(IEnumerable<GridLocation> exploredLocations,
                                           IEnumerable<GridLocation> visibleLocations,
                                           IEnumerable<GridLocation> revealedLocations)
        {
            var exploredGeometry = _scenarioUIService.CreateGeometry(exploredLocations);
            var visibleGeometry = _scenarioUIService.CreateGeometry(visibleLocations);
            var revealedGeometry = _scenarioUIService.CreateGeometry(revealedLocations);

            // Top Layer = Visible Mask ^ Explored Mask ^ Revealed Mask
            this.VisibleOpacityMask.Drawing = new GeometryDrawing(ModelConstants.FrontEnd.LevelBackground,
                                                                  new Pen(Brushes.Transparent, 0),
                                                                  visibleGeometry);

            // Middle Layer = Explored Geometry
            this.ExploredOpacityMask.Drawing = new GeometryDrawing(ModelConstants.FrontEnd.LevelBackground,
                                                                   new Pen(Brushes.Transparent, 0),
                                                                   exploredGeometry);

            // Bottom Layer = Revealed Mask
            this.RevealedOpacityMask.Drawing = new GeometryDrawing(ModelConstants.FrontEnd.LevelBackground,
                                                                   new Pen(Brushes.Transparent, 0),
                                                                   revealedGeometry);

            OnPropertyChanged(() => this.ExploredOpacityMask);
            OnPropertyChanged(() => this.VisibleOpacityMask);
            OnPropertyChanged(() => this.RevealedOpacityMask);
        }
        #endregion

        #region (private) Add / Update collections
        private LevelCanvasImage CreateContent(ScenarioObject scenarioObject)
        {
            var image = new LevelCanvasImage(scenarioObject.Id);

            // NOTE*** Have to set the dimensions of the image here and the stretch because
            //         the image sources are scaled to the cell size. This must be set to
            //         the cell size with no stretch so that the drawing transforms apply
            //         properly.
            //
            image.Width = ModelConstants.CellWidth;
            image.Height = ModelConstants.CellHeight;
            image.Stretch = Stretch.None;

            _scenarioUIService.UpdateContent(image, scenarioObject);

            return image;
        }

        private LevelCanvasShape CreateLightRadius(Character character)
        {
            var canvasShape = new LevelCanvasShape(character.Id, character.Id, new RectangleGeometry());

            _scenarioUIService.UpdateLightRadius(canvasShape, character, new Rect(0, 0, _scenarioUIService.LevelUIWidth, _scenarioUIService.LevelUIHeight));

            return canvasShape;
        }

        private LevelCanvasShape CreateAura(Character character, string alterationEffectId, string auraColor, int auraRange)
        {
            var canvasShape = new LevelCanvasShape(alterationEffectId, character.Id, new RectangleGeometry());

            _scenarioUIService.UpdateAura(canvasShape, auraColor, auraRange, character, new Rect(0, 0, _scenarioUIService.LevelUIWidth, _scenarioUIService.LevelUIHeight));

            return canvasShape;
        }

        protected void OnLevelDimensionChange()
        {
            // Fix Drawing Brush Properties
            //
            this.ExploredOpacityMask.Viewport = new Rect(0, 0, _scenarioUIService.LevelUIWidth, _scenarioUIService.LevelUIHeight);
            this.RevealedOpacityMask.Viewport = new Rect(0, 0, _scenarioUIService.LevelUIWidth, _scenarioUIService.LevelUIHeight);
            this.VisibleOpacityMask.Viewport = new Rect(0, 0, _scenarioUIService.LevelUIWidth, _scenarioUIService.LevelUIHeight);

            this.ExploredOpacityMask.Viewbox = new Rect(0, 0, _scenarioUIService.LevelUIWidth, _scenarioUIService.LevelUIHeight);
            this.RevealedOpacityMask.Viewbox = new Rect(0, 0, _scenarioUIService.LevelUIWidth, _scenarioUIService.LevelUIHeight);
            this.VisibleOpacityMask.Viewbox = new Rect(0, 0, _scenarioUIService.LevelUIWidth, _scenarioUIService.LevelUIHeight);
        }
        #endregion

        #region (private) Animations
        /// <summary>
        /// Creates IRogue2TimedGraphic set for each of the animation templates and returns the
        /// last one as a handle
        /// </summary>
        public async Task PlayAnimationSeries(IAnimationPlayer player)
        {
            // Hook event to change-over graphics
            player.AnimationPlayerStartEvent += OnAnimationPlayerStartEvent;
            player.AnimationPlayerChangeEvent += OnAnimationPlayerChangeEvent;

            // Run animations (SHARES MAIN THREAD VIA ASYNC / AWAIT)
            player.Start();

            // Start a delay (to allow processing of animation sequence)
            await Task.Delay(player.AnimationTime);
        }

        private void OnAnimationPlayerStartEvent(AnimationPlayerStartEventData eventData)
        {
            // Add primitives to the canvas (view model)
            foreach (var primitive in eventData.Primitives)
                this.Animations.Add(primitive);
        }
        private void OnAnimationPlayerChangeEvent(IAnimationPlayer sender, AnimationPlayerChangeEventData eventData)
        {
            // Remove old primitives from the canvas
            foreach (var primitive in eventData.OldPrimitives)
                this.Animations.Remove(primitive);

            // Add new primitives to the canvas (view model)
            if (!eventData.SequenceFinished)
            {
                foreach (var primitive in eventData.NewPrimitives)
                    this.Animations.Add(primitive);
            }

            // SEQUENCE FINISHED:  Unhook events
            else
            {
                sender.AnimationPlayerStartEvent -= OnAnimationPlayerStartEvent;
                sender.AnimationPlayerChangeEvent -= OnAnimationPlayerChangeEvent;
            }
        }

        public void PlayTargetAnimation(IAnimationPlayer targetAnimationPlayer)
        {
            _targetAnimationPlayer = targetAnimationPlayer;
            _targetAnimationPlayer.AnimationPlayerStartEvent += OnStartTargetAnimation;

            _targetAnimationPlayer.Start();
        }

        private void OnStartTargetAnimation(AnimationPlayerStartEventData eventData)
        {
            // Add primitives to the canvas (view model)
            foreach (var primitive in eventData.Primitives)
                this.Animations.Add(primitive);
        }

        public void StopTargetAnimation()
        {
            if (_targetAnimationPlayer != null)
            {
                _targetAnimationPlayer.AnimationPlayerStartEvent -= OnStartTargetAnimation;
                _targetAnimationPlayer.Stop();
                _targetAnimationPlayer = null;

                // Go ahead and clear animations because we're exiting targeting mode
                this.Animations.Clear();
            }
        }
        #endregion
    }
}

using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Extension.Event;
using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core.Media.Animation.EventData;
using Rogue.NET.Core.Media.Animation.Interface;
using Rogue.NET.Core.Model;
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
        DrawingImage[,] _layoutLayer;

        // Opacity Mask for masking UI layers with respect to the player's visual range
        DrawingBrush _visibileOpacityMask;

        // Player (separate from contents)
        LevelCanvasImage _player;

        public event SimpleEventHandler LayoutUpdated;
        public event SimpleEventHandler VisibilityUpdated;

        [ImportingConstructor]
        public LevelCanvasViewModel(IScenarioUIService scenarioUIService)
        {
            _scenarioUIService = scenarioUIService;

            this.Animations = new ObservableCollection<FrameworkElement>();
            this.Auras = new ObservableCollection<LevelCanvasShape>();
            this.Doodads = new ObservableCollection<LevelCanvasImage>();
            this.Items = new ObservableCollection<LevelCanvasImage>();
            this.Characters = new ObservableCollection<LevelCanvasImage>();

            this.VisibileOpacityMask = new DrawingBrush();

            RenderOptions.SetBitmapScalingMode(this.VisibileOpacityMask, BitmapScalingMode.LowQuality);
            RenderOptions.SetCachingHint(this.VisibileOpacityMask, CachingHint.Cache);

            this.VisibileOpacityMask.ViewboxUnits = BrushMappingMode.Absolute;
            this.VisibileOpacityMask.ViewportUnits = BrushMappingMode.Absolute;
        }

        #region (public) Properties
        public DrawingImage[,] VisibleLayer
        {
            get { return _layoutLayer; }
            set { this.RaiseAndSetIfChanged(ref _layoutLayer, value); }
        }
        public DrawingBrush VisibileOpacityMask
        {
            get { return _visibileOpacityMask; }
            set { this.RaiseAndSetIfChanged(ref _visibileOpacityMask, value); }
        }
        
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

            // Create DrawingImage instances from the resource cache
            //
            _scenarioUIService.CreateLayoutDrawings(this.VisibleLayer, /*this.ExploredLayer*/ null, /*this.RevealedLayer*/ null);

            OnPropertyChanged(() => this.VisibleLayer);

            OnLevelDimensionChange();

            if (this.LayoutUpdated != null)
                this.LayoutUpdated();
        }

        public void UpdateContent(IEnumerable<ScenarioObject> content,
                                  IEnumerable<ScenarioObject> memorizedContent,
                                  Player player)
        {
            // NOTE*** Memorized content includes ONLY Doodads and Items
            //
            // Remove (Filter out anything that isn't in the level)
            this.Doodads.Filter(x => content.None(z => z.Id == x.ScenarioObjectId) &&
                                     memorizedContent.None(z => z.Id == x.ScenarioObjectId));

            this.Characters.Filter(x => content.None(z => z.Id == x.ScenarioObjectId) &&
                                        x.ScenarioObjectId != player.Id);

            this.Items.Filter(x => content.None(z => z.Id == x.ScenarioObjectId) &&
                                   memorizedContent.None(z => z.Id == x.ScenarioObjectId));

            this.Auras.Filter(x => content.None(z => z.Id == x.ScenarioObjectId));

            // Create content dictionary with unique references - showing memorized content
            var allContent = content.Union(memorizedContent)
                                    .Distinct()
                                    .ToDictionary(x => x, x => memorizedContent.Contains(x));

            // Doodads
            foreach (var scenarioObject in allContent.Keys.Where(x => x is DoodadBase))
            {
                var doodad = scenarioObject as DoodadBase;
                var doodadViewModel = this.Doodads.FirstOrDefault(x => x.ScenarioObjectId == doodad.Id);

                // Update Content
                if (doodadViewModel != null)
                    _scenarioUIService.UpdateContent(doodadViewModel, scenarioObject, allContent[scenarioObject]);

                else
                    this.Doodads.Add(CreateContent(scenarioObject, allContent[scenarioObject]));
            }

            // Items
            foreach (var scenarioObject in allContent.Keys.Where(x => x is ItemBase))
            {
                var item = scenarioObject as ItemBase;
                var itemViewModel = this.Items.FirstOrDefault(x => x.ScenarioObjectId == item.Id);

                // Update Content
                if (itemViewModel != null)
                    _scenarioUIService.UpdateContent(itemViewModel, scenarioObject, allContent[scenarioObject]);

                else
                    this.Items.Add(CreateContent(scenarioObject, allContent[scenarioObject]));
            }

            // Characters
            foreach (var scenarioObject in allContent.Keys.Where(x => x is CharacterBase))
            {
                // Character
                var character = scenarioObject as CharacterBase;
                var characterViewModel = this.Characters.FirstOrDefault(x => x.ScenarioObjectId == scenarioObject.Id);

                // Update Content
                if (characterViewModel != null)
                    _scenarioUIService.UpdateContent(characterViewModel, scenarioObject, allContent[scenarioObject]);

                else
                    this.Characters.Add(CreateContent(scenarioObject, allContent[scenarioObject]));

                // SET PLAYER REFERENCE
                if (scenarioObject is Player)
                    this.Player = this.Characters.First(character => character.ScenarioObjectId == player.Id);

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
        }

        /// <summary>
        /// Draws visibility visual used as an opacity mask for the level
        /// </summary>
        public void UpdateLayoutVisibility(IEnumerable<GridLocation> exploredLocations,
                                           IEnumerable<GridLocation> visibleLocations,
                                           IEnumerable<GridLocation> revealedLocations)
        {
            var visibleGeometry = _scenarioUIService.CreateOutlineGeometry(visibleLocations);

            // FREEZE TO BOOST RENDERING PERFORMANCE
            visibleGeometry.Freeze();

            // Top Layer = Visible Mask ^ Explored Mask ^ Revealed Mask
            this.VisibileOpacityMask.Drawing = new GeometryDrawing(ModelConstants.FrontEnd.LevelBackground,
                                                                     new Pen(Brushes.Transparent, 0),
                                                                     visibleGeometry);

            OnPropertyChanged(() => this.VisibileOpacityMask);

            if (this.VisibilityUpdated != null)
                this.VisibilityUpdated();
        }
        #endregion

        #region (private) Add / Update collections
        private LevelCanvasImage CreateContent(ScenarioObject scenarioObject, bool isMemorized)
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

            _scenarioUIService.UpdateContent(image, scenarioObject, isMemorized);

            return image;
        }

        private LevelCanvasShape CreateAura(CharacterBase character, string alterationEffectId, string auraColor, int auraRange)
        {
            var canvasShape = new LevelCanvasShape(alterationEffectId, character.Id, new RectangleGeometry());

            _scenarioUIService.UpdateAura(canvasShape, auraColor, auraRange, character, new Rect(0, 0, _scenarioUIService.LevelUIWidth, _scenarioUIService.LevelUIHeight));

            return canvasShape;
        }

        protected void OnLevelDimensionChange()
        {
            // Fix Drawing Brush Properties
            //
            this.VisibileOpacityMask.Viewport = new Rect(0, 0, _scenarioUIService.LevelUIWidth, _scenarioUIService.LevelUIHeight);
            this.VisibileOpacityMask.Viewbox = new Rect(0, 0, _scenarioUIService.LevelUIWidth, _scenarioUIService.LevelUIHeight);
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

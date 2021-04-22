using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.GameRouter.GameEvent.Backend.Enum;
using Rogue.NET.Core.Media.Animation.Interface;
using Rogue.NET.Core.Media.SymbolEffect.Utility;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Processing.Command.Frontend.Data;
using Rogue.NET.Core.Processing.Command.Frontend.Enum;
using Rogue.NET.Core.Processing.Event.Backend;
using Rogue.NET.Core.Processing.Event.Backend.EventData;
using Rogue.NET.Core.Processing.Event.Level;
using Rogue.NET.Core.Processing.Model.Content.Enum;
using Rogue.NET.Core.Processing.Service.Interface;
using Rogue.NET.Scenario.Content.ViewModel.LevelCanvas.Inteface;
using Rogue.NET.Scenario.Processing.Controller.Interface;
using Rogue.NET.Scenario.Processing.Service.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Rogue.NET.Scenario.Processing.Controller
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IFrontendController))]
    public class FrontendController : IFrontendController
    {
        readonly IRogueEventAggregator _eventAggregator;
        readonly IScenarioUIService _scenarioUIService;
        readonly ITargetingService _targetingService;
        readonly IModelService _modelService;

        // View-Model Components - Subscribe to updates here
        readonly ILevelCanvasViewModel _levelCanvasViewModel;

        [ImportingConstructor]
        public FrontendController(
                IRogueEventAggregator eventAggregator,
                IModelService modelService,
                ITargetingService targetingService,
                IScenarioUIService scenarioUIService,
                ILevelCanvasViewModel levelCanvasViewModel)
        {
            _eventAggregator = eventAggregator;
            _modelService = modelService;
            _targetingService = targetingService;
            _scenarioUIService = scenarioUIService;
            _levelCanvasViewModel = levelCanvasViewModel;

            // PRIMARY HOOK FOR LEVEL EVENTS
            eventAggregator.GetEvent<LevelEvent>()
                           .Subscribe(OnLevelEvent);

            // MOVED FROM LEVEL CANVAS VIEW MODEL
            eventAggregator.GetEvent<LevelLoadedEvent>().Subscribe(() =>
            {
                // Clear Animations and Contents
                _levelCanvasViewModel.Animations.Clear();
                _levelCanvasViewModel.Doodads.Clear();
                _levelCanvasViewModel.Items.Clear();
                _levelCanvasViewModel.Characters.Clear();

                // Clear Targeting Animation
                _levelCanvasViewModel.StopTargetAnimation();

                // Load Level Layout
                OnUpdateLayout();

                // Load Level Content
                OnUpdateAllContent();

                // Load Level Layout Visibility
                OnUpdateLayoutVisibility();

            // SET PRIORITY TO EXECUTE AFTER THE LEVEL IS INVALIDATED
            }, RogueEventPriority.High);

            // ANIMATION EVENTS
            eventAggregator.GetEvent<AnimationStartEvent>().Subscribe(async eventData =>
            {
                var levelUIBounds = new Rect(0, 0, scenarioUIService.LevelUIWidth, scenarioUIService.LevelUIHeight);
                var animation = scenarioUIService.CreateAnimation(eventData, levelUIBounds);

                await _levelCanvasViewModel.PlayAnimationSeries(animation);
            });

            eventAggregator.GetEvent<ProjectileAnimationStartEvent>().Subscribe(async eventData =>
            {
                var levelUIBounds = new Rect(0, 0, scenarioUIService.LevelUIWidth, scenarioUIService.LevelUIHeight);
                var animation = scenarioUIService.CreateAnimation(eventData, levelUIBounds);

                await _levelCanvasViewModel.PlayAnimationSeries(animation);
            });
        }

        public Task PublishCommand(FrontendCommandData commandData)
        {
            switch (commandData.Type)
            {
                case FrontendCommandType.MoveTarget:

                    // Move the target tracker
                    if (_targetingService.MoveTarget(commandData.Direction))
                    {
                        // Update the targert animation
                        UpdateTargetAnimation();
                    }
                    break;
                case FrontendCommandType.CycleTarget:
                    {
                        // Cycle target through visible enemies
                        _targetingService.CycleTarget(commandData.Direction);

                        // Update the targert animation
                        UpdateTargetAnimation();
                    }
                    break;
                case FrontendCommandType.StartTargeting:
                    {
                        // Begin target tracking starting with the player
                        _targetingService.StartTargeting(_modelService.PlayerLocation);

                        // Update the animation
                        UpdateTargetAnimation();
                    }
                    break;
                case FrontendCommandType.SelectTarget:
                    {
                        // Capture target information and end targeting
                        _targetingService.EndTargeting();

                        // Halt animation
                        StopTargetAnimation();
                    }
                    break;
                case FrontendCommandType.EndTargeting:
                    {
                        // Capture target information and end targeting
                        _targetingService.Clear();

                        // Halt animation
                        StopTargetAnimation();
                    }
                    break;
                default:
                    throw new Exception("Unhandled Frontend Command Type");
            }

            return Task.Delay(1);
        }

        // PRIMARY LISTENER FOR LEVEL EVENTS (for the frontend)
        private void OnLevelEvent(LevelEventData eventData)
        {
            switch (eventData.LevelUpdateType)
            {
                case LevelEventType.ContentAll:
                case LevelEventType.ContentVisible:
                    OnUpdateAllContent();
                    OnUpdateLayoutVisibility();
                    break;
                case LevelEventType.ContentReveal:
                    OnUpdateAllContent();
                    break;
                case LevelEventType.ContentRemove:
                    OnRemoveContent(eventData.ContentIds);
                    break;
                case LevelEventType.ContentMove:
                    break;
                case LevelEventType.ContentUpdate:
                    break;
                case LevelEventType.ContentAdd:
                    OnUpdateAllContent();
                    break;
                case LevelEventType.LayoutAll:
                case LevelEventType.LayoutTopology:
                    OnUpdateLayout();
                    OnUpdateLayoutVisibility();
                    break;
                case LevelEventType.LayoutVisible:
                case LevelEventType.LayoutReveal:
                    OnUpdateLayoutVisibility();
                    break;
                case LevelEventType.PlayerLocation:
                    _scenarioUIService.UpdateContent(_levelCanvasViewModel.Player, _modelService.Player, false);
                    break;
                case LevelEventType.PlayerConsumableRemove:
                    break;
                case LevelEventType.PlayerConsumableAddOrUpdate:
                    break;
                case LevelEventType.PlayerEquipmentRemove:
                    break;
                case LevelEventType.PlayerEquipmentAddOrUpdate:
                    break;
                case LevelEventType.PlayerSkillSetAdd:
                    break;
                case LevelEventType.PlayerSkillSetRefresh:
                    break;
                case LevelEventType.PlayerStats:
                    break;
                case LevelEventType.PlayerAll:
                    break;
                case LevelEventType.EncyclopediaIdentify:
                    break;
                case LevelEventType.EncyclopediaCurseIdentify:
                    break;
                default:
                    break;
            }
        }

        #region (private) Update Methods
        private void OnUpdateAllContent()
        {
            _levelCanvasViewModel.UpdateContent(_modelService.Level.Content.AllContent, 
                                                _modelService.Level.MemorizedContent.AllContent,
                                                _modelService.Player);
        }
        private void OnUpdateLayout()
        {
            _levelCanvasViewModel.UpdateLayout();
        }
        private void OnUpdateLayoutVisibility()
        {
            _levelCanvasViewModel.UpdateLayoutVisibility(_modelService.Level
                                                                      .Movement
                                                                      .GetVisibleLocations());
        }
        private void OnRemoveContent(IEnumerable<string> contentIds)
        {
            // Filter out contents with matching id's
            _levelCanvasViewModel.Doodads.Filter(x => contentIds.Contains(x.ScenarioObjectId));
            _levelCanvasViewModel.Items.Filter(x => contentIds.Contains(x.ScenarioObjectId));
            _levelCanvasViewModel.Characters.Filter(x => contentIds.Contains(x.ScenarioObjectId));
            _levelCanvasViewModel.Auras.Filter(x => contentIds.Contains(x.ScenarioObjectId));
        }
        #endregion

        #region (private) Animation Methods
        private void PlayTargetAnimation(GridLocation location, Color fillColor, Color strokeColor)
        {
            _levelCanvasViewModel.PlayTargetAnimation(_scenarioUIService.CreateTargetAnimation(location, fillColor, strokeColor));
        }
        private void StopTargetAnimation()
        {
            _levelCanvasViewModel.StopTargetAnimation();
        }
        #endregion

        #region (private) Targeting Methods
        private void UpdateTargetAnimation()
        {
            StopTargetAnimation();

            // Create a new targeting animation
            switch (_targetingService.GetTrackedTargetType())
            {
                case TargetType.None:
                    break;
                case TargetType.Location:
                    PlayTargetAnimation(_targetingService.GetTrackedTargetLocation(),
                                        Colors.Blue,
                                        Colors.LightYellow);
                    break;
                case TargetType.Character:
                    PlayTargetAnimation(_targetingService.GetTrackedTargetLocation(),
                                        Colors.Magenta,
                                        Colors.Magenta);
                    break;
                default:
                    throw new Exception("Unhandled TargetType IFrontendController");
            }
        }
        #endregion
    }
}

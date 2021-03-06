﻿using Prism.Mef.Modularity;
using Prism.Modularity;

using System;
using System.ComponentModel.Composition;
using System.Windows;

using Rogue.NET.Scenario.Intro.Views.GameSetup;
using Rogue.NET.Scenario.Outro.Views;
using Rogue.NET.Scenario.Views;
using Rogue.NET.Scenario.Outro;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Common.Extension.Prism.RegionManager.Interface;
using Rogue.NET.Scenario.Constant;
using Rogue.NET.Scenario.Intro.Views;
using Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Effect;
using Rogue.NET.Scenario.Content.Views.Alteration;
using Rogue.NET.Scenario.Content.Views.Dialog.Interface;
using Rogue.NET.Core.Processing.Event.Backend;
using Rogue.NET.Core.GameRouter.GameEvent.Backend.Enum;
using Rogue.NET.Core.Processing.Event.Dialog;
using Rogue.NET.Core.Processing.Command.Backend.CommandData;
using Rogue.NET.Core.Processing.Command.Backend;
using Rogue.NET.Core.Processing.Service.Interface;
using Rogue.NET.Scenario.Processing.Controller.Interface;
using Rogue.NET.Core.Processing.Event.Level;
using Rogue.NET.Core.Processing.Event.Scenario;
using Rogue.NET.Core.Processing.Event.ScenarioEditor;
using Rogue.NET.Scenario.Processing.Event;
using Rogue.NET.Scenario.Processing.Event.Content;
using Rogue.NET.Scenario.Processing.Event.Content.SkillTree;
using System.Linq;

namespace Rogue.NET.Scenario
{
    [ModuleExport("ScenarioModule", typeof(ScenarioModule), InitializationMode = InitializationMode.WhenAvailable)]
    public class ScenarioModule : IModule
    {
        readonly IRogueRegionManager _regionManager;
        readonly IRogueEventAggregator _eventAggregator;
        readonly IGameController _gameController;
        readonly IScenarioResourceService _scenarioResourceService;

        [ImportingConstructor]
        public ScenarioModule(
            IRogueRegionManager regionManager,
            IRogueEventAggregator eventAggregator,
            IGameController gameController,
            IScenarioResourceService scenarioResourceService)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            _gameController = gameController;
            _scenarioResourceService = scenarioResourceService;
        }

        public void Initialize()
        {
            _gameController.Initialize();

            _eventAggregator.GetEvent<LevelLoadedEvent>().Subscribe(() =>
            {
                _regionManager.LoadSingleInstance(RegionName.MainRegion, typeof(GameView));
                _regionManager.LoadSingleInstance(RegionName.GameRegion, typeof(LevelView));
            });

            _eventAggregator.GetEvent<ScenarioEvent>().Subscribe(update =>
            {
                if (update.ScenarioUpdateType == ScenarioUpdateType.PlayerDeath)
                {
                    var view = _regionManager.LoadSingleInstance(RegionName.MainRegion, typeof(DeathDisplay));

                    (view as DeathDisplay).DiedOfText = update.PlayerDeathMessage;
                }
                else if (update.ScenarioUpdateType == ScenarioUpdateType.ScenarioCompleted)
                {
                    var view = _regionManager.LoadSingleInstance(RegionName.MainRegion, typeof(OutroDisplay));
                }

            });

            _eventAggregator.GetEvent<ExitScenarioEvent>().Subscribe(() =>
            {
                _regionManager.LoadSingleInstance(RegionName.MainRegion, typeof(GameSetupView));
                _regionManager.LoadSingleInstance(RegionName.GameSetupRegion, typeof(NewOpenEdit), true);
            });

            _eventAggregator.GetEvent<ExitScenarioEditorEvent>().Subscribe(() =>
            {
                _regionManager.LoadSingleInstance(RegionName.MainRegion, typeof(GameSetupView));
                _regionManager.LoadSingleInstance(RegionName.GameSetupRegion, typeof(NewOpenEdit), true);
            });

            _eventAggregator.GetEvent<IntroFinishedEvent>().Subscribe(() =>
            {
                _regionManager.LoadSingleInstance(RegionName.MainRegion, typeof(GameSetupView));
                _regionManager.LoadSingleInstance(RegionName.GameSetupRegion, typeof(NewOpenEdit), true);
            });

            _eventAggregator.GetEvent<GameSetupDisplayFinished>().Subscribe((e) =>
            {
                _regionManager.LoadSingleInstance(RegionName.GameSetupRegion, e.NextDisplayType);
            });

            _eventAggregator.GetEvent<RequestNaviateParametersDisplayEvent>().Subscribe((type) =>
            {
                _regionManager.LoadSingleInstance(RegionName.ChooseParametersRegion, type);
            });

            _eventAggregator.GetEvent<OutroFinishedEvent>().Subscribe(() =>
            {
                _regionManager.LoadSingleInstance(RegionName.MainRegion, typeof(IntroView));
            });

            _eventAggregator.GetEvent<RequestNavigateToLevelViewEvent>().Subscribe(() =>
            {
                _regionManager.LoadSingleInstance(RegionName.GameRegion, typeof(LevelView));
            });
            _eventAggregator.GetEvent<RequestNavigateToEquipmentSelectionEvent>().Subscribe(() =>
            {
                _regionManager.LoadSingleInstance(RegionName.GameRegion, typeof(EquipmentSelectionCtrl));
            });
            _eventAggregator.GetEvent<RequestNavigateToEncyclopediaEvent>().Subscribe(() =>
            {
                _regionManager.LoadSingleInstance(RegionName.GameRegion, typeof(DungeonEncyclopedia));
            });

            // Content -> Skill Tree
            _eventAggregator.GetEvent<SkillTreeLoadAlterationEffectRegionEvent>().Subscribe((region, payload) =>
            {
                // TODO: Could move these to an attribute for the view models

                if (payload is AttackAttributeAuraAlterationEffectViewModel)
                    _regionManager.Load(region, typeof(AttackAttributeAuraAlterationEffectView));

                else if (payload is AttackAttributeMeleeAlterationEffectViewModel)
                    _regionManager.Load(region, typeof(AttackAttributeMeleeAlterationEffectView));

                else if (payload is AttackAttributePassiveAlterationEffectViewModel)
                    _regionManager.Load(region, typeof(AttackAttributePassiveAlterationEffectView));

                else if (payload is AttackAttributeTemporaryAlterationEffectViewModel)
                    _regionManager.Load(region, typeof(AttackAttributeTemporaryAlterationEffectView));

                else if (payload is AuraAlterationEffectViewModel)
                    _regionManager.Load(region, typeof(AuraAlterationEffectView));

                else if (payload is ChangeLevelAlterationEffectViewModel)
                    _regionManager.Load(region, typeof(ChangeLevelAlterationEffectView));

                else if (payload is CreateEnemyAlterationEffectViewModel)
                    _regionManager.Load(region, typeof(CreateEnemyAlterationEffectView));

                else if (payload is CreateFriendlyAlterationEffectViewModel)
                    _regionManager.Load(region, typeof(CreateFriendlyAlterationEffectView));

                else if (payload is CreateTemporaryCharacterAlterationEffectViewModel)
                    _regionManager.Load(region, typeof(CreateTemporaryCharacterAlterationEffectView));

                else if (payload is DrainMeleeAlterationEffectViewModel)
                    _regionManager.Load(region, typeof(DrainMeleeAlterationEffectView));

                else if (payload is EquipmentDamageAlterationEffectViewModel)
                    _regionManager.Load(region, typeof(EquipmentDamageAlterationEffectView));

                else if (payload is EquipmentEnhanceAlterationEffectViewModel)
                    _regionManager.Load(region, typeof(EquipmentEnhanceAlterationEffectView));

                else if (payload is IdentifyAlterationEffectViewModel)
                    _regionManager.Load(region, typeof(OtherAlterationEffectView));

                else if (payload is PassiveAlterationEffectViewModel)
                    _regionManager.Load(region, typeof(PassiveAlterationEffectView));

                else if (payload is PermanentAlterationEffectViewModel)
                    _regionManager.Load(region, typeof(PermanentAlterationEffectView));

                else if (payload is RemedyAlterationEffectViewModel)
                    _regionManager.Load(region, typeof(RemedyAlterationEffectView));

                else if (payload is RevealAlterationEffectViewModel)
                    _regionManager.Load(region, typeof(RevealAlterationEffectView));

                else if (payload is StealAlterationEffectViewModel)
                    _regionManager.Load(region, typeof(StealAlterationEffectView));

                else if (payload is TeleportManualAlterationEffectViewModel)
                    _regionManager.Load(region, typeof(TeleportManualAlterationEffectView));

                else if (payload is TeleportRandomAlterationEffectViewModel)
                    _regionManager.Load(region, typeof(TeleportRandomAlterationEffectView));

                else if (payload is TemporaryAlterationEffectViewModel)
                    _regionManager.Load(region, typeof(TemporaryAlterationEffectView));

                else if (payload is TransmuteAlterationEffectViewModel)
                    _regionManager.Load(region, typeof(TransmuteAlterationEffectView));

                else
                    throw new Exception("Unhandled AlterationEffectViewModel");
            });

            // Delete Scenario
            _eventAggregator.GetEvent<DeleteScenarioEvent>().Subscribe((scenarioInfo) =>
            {
                var result = MessageBox.Show("Are you sure you want to delete this scenario?", "Delete " + scenarioInfo.ScenarioName, MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Yes || result == MessageBoxResult.OK)
                {
                    // Delete the scenario
                    _scenarioResourceService.DeleteScenario(scenarioInfo);

                    // Notify listeners
                    _eventAggregator.GetEvent<ScenarioDeletedEvent>().Publish();
                }
            });

            RegisterViews();
        }

        private void RegisterViews()
        {
            // RogueRegionManager does not require that views are pre-registered. There are several
            // ways to load a view into a region; but those will instantiate the view when they're called.
            //
            // This can cause issues because many event aggregator subscriptions are handled in the 
            // constructors. 
            //
            // The Prism region manager handles this using pre-registration; but the design is limited to
            // singleton regions. Sharing view instances led to problems in the scenario editor - so I
            // created a region manager that uses region instances to register views.
            //
            // Leaving these (below) views un-registered caused problems when loading scenario view models
            // because most of the event-aggregator events had already been called to initialize the view.
            //
            // So, I had to create a way to pre-register views to allow the constructor to be called before
            // hand. This will only work for "singleton regions" (see RogueRegionManager)
            //

            _regionManager.PreRegisterView(RegionName.MainRegion, typeof(GameView));
            _regionManager.PreRegisterView(RegionName.GameRegion, typeof(LevelView));
            _regionManager.PreRegisterView(RegionName.GameRegion, typeof(EquipmentSelectionCtrl));
            _regionManager.PreRegisterView(RegionName.GameRegion, typeof(DungeonEncyclopedia));
        }
    }
}


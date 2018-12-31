﻿using Prism.Events;
using Prism.Mef.Modularity;
using Prism.Modularity;
using Prism.Regions;
using Rogue.NET.Common.Events.Scenario;
using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Service;
using Rogue.NET.Core.Service.Interface;
using Rogue.NET.ScenarioEditor.Controller.Interface;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.Service.Interface;
using Rogue.NET.ScenarioEditor.Utility;
using Rogue.NET.ScenarioEditor.ViewModel.Constant;
using Rogue.NET.ScenarioEditor.ViewModel.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
using Rogue.NET.ScenarioEditor.Views;
using Rogue.NET.ScenarioEditor.Views.Assets;
using Rogue.NET.ScenarioEditor.Views.Assets.AnimationControl;
using Rogue.NET.ScenarioEditor.Views.Assets.ConsumableControl;
using Rogue.NET.ScenarioEditor.Views.Assets.EnemyControl;
using Rogue.NET.ScenarioEditor.Views.Assets.EquipmentControl;
using Rogue.NET.ScenarioEditor.Views.Assets.SpellControl;
using Rogue.NET.ScenarioEditor.Views.Construction;
using Rogue.NET.ScenarioEditor.Views.Controls;
using Rogue.NET.ScenarioEditor.Views.DesignRegion;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor
{
    [ModuleExport("ScenarioEditorModule", typeof(ScenarioEditorModule))]
    public class ScenarioEditorModule : IModule
    {
        readonly IEventAggregator _eventAggregator;
        readonly IRegionManager _regionManager;
        readonly IScenarioAssetController _scenarioAssetController;
        readonly IScenarioEditorController _scenarioEditorController;
        readonly IScenarioResourceService _resourceService;
        readonly IScenarioConfigurationUndoService _undoService;
        readonly IScenarioAssetReferenceService _scenarioAssetReferenceService;

        [ImportingConstructor]
        public ScenarioEditorModule(
            IRegionManager regionManager,
            IEventAggregator eventAggregator,
            IScenarioAssetController scenarioAssetController,
            IScenarioEditorController scenarioEditorController,
            IScenarioResourceService scenarioResourceService,
            IScenarioConfigurationUndoService scenarioConfigurationUndoService,
            IScenarioAssetReferenceService scenarioAssetReferenceService)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            _scenarioAssetController = scenarioAssetController;
            _scenarioEditorController = scenarioEditorController;
            _resourceService = scenarioResourceService;
            _undoService = scenarioConfigurationUndoService;
            _scenarioAssetReferenceService = scenarioAssetReferenceService;
        }

        public void Initialize()
        {
            RegisterRegionViews();
            RegisterEvents();
        }

        private void RegisterRegionViews()
        {
            // Regions
            _regionManager.RegisterViewWithRegion("MainRegion", typeof(Editor));
            _regionManager.RegisterViewWithRegion("DesignRegion", typeof(EditorInstructions));
            _regionManager.RegisterViewWithRegion("AssetBrowserRegion", typeof(ScenarioAssetBrowser));
            _regionManager.RegisterViewWithRegion("ScenarioConstructionRegion", typeof(ScenarioConstruction));
            _regionManager.RegisterViewWithRegion("OutputRegion", typeof(Output));

            // Design Region - Asset Views
            _regionManager.RegisterViewWithRegion("DesignRegion", typeof(AssetContainerControl));
            _regionManager.RegisterViewWithRegion("AssetContainerRegion", typeof(Animation));
            _regionManager.RegisterViewWithRegion("AnimationParametersRegion", typeof(AnimationParameters));
            _regionManager.RegisterViewWithRegion("AnimationPreviewRegion", typeof(AnimationPreviewControl));
            _regionManager.RegisterViewWithRegion("AssetContainerRegion", typeof(Rogue.NET.ScenarioEditor.Views.Assets.Brush));
            _regionManager.RegisterViewWithRegion("AssetContainerRegion", typeof(Consumable));
            _regionManager.RegisterViewWithRegion("ConsumableParametersRegion", typeof(ConsumableParameters));
            _regionManager.RegisterViewWithRegion("AssetContainerRegion", typeof(Doodad));
            _regionManager.RegisterViewWithRegion("AssetContainerRegion", typeof(Enemy));
            _regionManager.RegisterViewWithRegion("EnemyItemsRegion", typeof(EnemyItems));
            _regionManager.RegisterViewWithRegion("EnemyBehaviorRegion", typeof(EnemyBehavior));
            _regionManager.RegisterViewWithRegion("EnemyParametersRegion", typeof(EnemyParameters));
            _regionManager.RegisterViewWithRegion("AssetContainerRegion", typeof(Equipment));
            _regionManager.RegisterViewWithRegion("EquipmentParametersRegion", typeof(EquipmentParameters));
            _regionManager.RegisterViewWithRegion("AssetContainerRegion", typeof(Layout));
            _regionManager.RegisterViewWithRegion("AssetContainerRegion", typeof(SkillSet));
            _regionManager.RegisterViewWithRegion("AssetContainerRegion", typeof(Spell));
            _regionManager.RegisterViewWithRegion("SpellParametersRegion", typeof(SpellParameters));


            // Design Region - Construction Views
            _regionManager.RegisterViewWithRegion("DesignRegion", typeof(General));
            _regionManager.RegisterViewWithRegion("DesignRegion", typeof(ScenarioObjectPlacement));
            _regionManager.RegisterViewWithRegion("DesignRegion", typeof(EnemyPlacement));
            _regionManager.RegisterViewWithRegion("DesignRegion", typeof(ItemPlacement));
            _regionManager.RegisterViewWithRegion("DesignRegion", typeof(LayoutDesign));
            _regionManager.RegisterViewWithRegion("DesignRegion", typeof(ObjectiveDesign));
            _regionManager.RegisterViewWithRegion("DesignRegion", typeof(PlayerDesign));

            // Design Region - Difficulty View
            _regionManager.RegisterViewWithRegion("DesignRegion", typeof(ScenarioDesignOverview));
        }
        private void RegisterEvents()
        {
            // Scenario Editor Events
            _eventAggregator.GetEvent<EditScenarioEvent>().Subscribe(() =>
            {
                _regionManager.RequestNavigate("MainRegion", "Editor");
                _regionManager.RequestNavigate("DesignRegion", "EditorInstructions");

                // Create an instance of the config so that there aren't any null refs.
                _scenarioEditorController.New();
            });
            _eventAggregator.GetEvent<LoadDifficultyChartEvent>().Subscribe(() =>
            {
                _regionManager.RequestNavigate("DesignRegion", "ScenarioDesignOverview");
            });

            // Asset Events
            _eventAggregator.GetEvent<AddAssetEvent>().Subscribe((e) =>
            {
                _scenarioAssetController.AddAsset(e.AssetType, e.AssetUniqueName);

                // Publish a special event to update source lists for specific views
                PublishScenarioUpdate();
            });
            _eventAggregator.GetEvent<LoadAssetEvent>().Subscribe((e) =>
            {
                LoadAsset(e);
            });
            _eventAggregator.GetEvent<RemoveAssetEvent>().Subscribe((e) =>
            {
                _scenarioAssetController.RemoveAsset(e.Type, e.Name);

                // Load the Editor Instructions to prevent editing removed asset
                _regionManager.RequestNavigate("DesignRegion", "EditorInstructions");

                // Publish a special event to update source lists for specific views
                PublishScenarioUpdate();
            });
            _eventAggregator.GetEvent<RenameAssetEvent>().Subscribe((e) =>
            {
                // Show rename control
                var view = new RenameControl();
                var asset = _scenarioAssetController.GetAsset(e.Name, e.Type);

                view.DataContext = asset;

                DialogWindowFactory.Show(view, "Rename Scenario Asset");

                // Update Asset Name
                e.Name = asset.Name;

                // Publish a special event to update source lists for specific views
                PublishScenarioUpdate();

                // Reload Asset
                LoadAsset(e);
            });
            _eventAggregator.GetEvent<CopyAssetEvent>().Subscribe((e) =>
            {
                // Create a copy of the asset
                _scenarioAssetController.CopyAsset(e.AssetName, e.AssetNewName, e.AssetType);

                // Publish a special event to update source lists for specific views
                PublishScenarioUpdate();
            });

            // Construction Events
            _eventAggregator.GetEvent<LoadConstructionEvent>().Subscribe((e) =>
            {
                LoadConstruction(e.ConstructionName);
            });
            _eventAggregator.GetEvent<AddCombatAttributeEvent>().Subscribe((e) =>
            {
                // NOTE*** THIS CAUSES MANY CHANGES TO THE MODEL. REQUIRES AN UNDO BLOCK AND CLEARING OF 
                //         THE STACK
                _undoService.Block();

                // Add Combat Attribute to the scenario
                _scenarioEditorController.CurrentConfig.CombatAttributes.Add(e);

                // Update Scenario object references
                _scenarioAssetReferenceService.UpdateCombatAttributes(_scenarioEditorController.CurrentConfig);

                // Allow undo changes again - and clear the stack to prevent old references to Attack Attributes
                _undoService.UnBlock();
                _undoService.Clear();

                // Reload designer
                LoadConstruction("General");
            });
            _eventAggregator.GetEvent<RemoveCombatAttributeEvent>().Subscribe((e) =>
            {
                // NOTE*** THIS CAUSES MANY CHANGES TO THE MODEL. REQUIRES AN UNDO BLOCK AND CLEARING OF 
                //         THE STACK
                _undoService.Block();

                // Remove Combat Attribute from the scenario
                _scenarioEditorController.CurrentConfig.CombatAttributes.Remove(e);

                // Update Scenario object references
                _scenarioAssetReferenceService.UpdateCombatAttributes(_scenarioEditorController.CurrentConfig);

                // Allow undo changes again - and clear the stack to prevent old references to Attack Attributes
                _undoService.UnBlock();
                _undoService.Clear();

                // Reload designer
                LoadConstruction("General");
            });
            _eventAggregator.GetEvent<UpdateCombatAttributeEvent>().Subscribe((e) =>
            {
                // NOTE*** THIS CAUSES MANY CHANGES TO THE MODEL. REQUIRES AN UNDO BLOCK AND CLEARING OF 
                //         THE STACK
                _undoService.Block();

                // Update Scenario object references
                _scenarioAssetReferenceService.UpdateCombatAttributes(_scenarioEditorController.CurrentConfig);

                // Allow undo changes again - and clear the stack to prevent old references to Attack Attributes
                _undoService.UnBlock();
                _undoService.Clear();

                // Reload designer
                LoadConstruction("General");
            });

            _eventAggregator.GetEvent<AddAlteredCharacterStateEvent>().Subscribe((e) =>
            {
                // NOTE*** THIS CAUSES MANY CHANGES TO THE MODEL. REQUIRES AN UNDO BLOCK AND CLEARING OF 
                //         THE STACK
                _undoService.Block();

                // Add Attack Attribute to the scenario
                _scenarioEditorController.CurrentConfig.AlteredCharacterStates.Add(new AlteredCharacterStateTemplateViewModel()
                {
                    Name = e.Name,
                    SymbolDetails = new SymbolDetailsTemplateViewModel()
                    {
                        Type = SymbolTypes.Image,
                        Icon = e.Icon
                    },
                    BaseType = e.BaseType
                });

                // Update Scenario object references
                _scenarioAssetReferenceService.UpdateAlteredCharacterStates(_scenarioEditorController.CurrentConfig);

                // Allow undo changes again - and clear the stack to prevent old references to Attack Attributes
                _undoService.UnBlock();
                _undoService.Clear();

                // Reload designer
                LoadConstruction("General");
            });
            _eventAggregator.GetEvent<RemoveAlteredCharacterStateEvent>().Subscribe((e) =>
            {
                // NOTE*** THIS CAUSES MANY CHANGES TO THE MODEL. REQUIRES AN UNDO BLOCK AND CLEARING OF 
                //         THE STACK
                _undoService.Block();

                // Remove Attack Attribute from the scenario
                _scenarioEditorController.CurrentConfig.AlteredCharacterStates.Remove(e);

                // Update Scenario object references
                _scenarioAssetReferenceService.UpdateAlteredCharacterStates(_scenarioEditorController.CurrentConfig);

                // Allow undo changes again - and clear the stack to prevent old references to Attack Attributes
                _undoService.UnBlock();
                _undoService.Clear();

                // Reload designer
                LoadConstruction("General");
            });
        }

        private void LoadAsset(IScenarioAssetViewModel assetViewModel)
        {
            // KLUDGE:  This block is to prevent ComboBox Binding update issues. Events were firing when
            //          the view changed that were caught by the undo service. This should prevent those.
            _undoService.Block();

            // Get the asset for loading into the design region
            var viewModel = _scenarioAssetController.GetAsset(assetViewModel.Name, assetViewModel.Type);

            // Get the view name for this asset type
            var viewName = AssetType.AssetViews[assetViewModel.Type];

            // Request navigate to load the control (These are by type string)
            _regionManager.RequestNavigate("DesignRegion", "AssetContainerControl");
            _regionManager.RequestNavigate("AssetContainerRegion", viewName);

            // Set parameters for Asset Container by hand
            var assetContainer = _regionManager.Regions["DesignRegion"]
                                               .Views.First(x => x.GetType() == typeof(AssetContainerControl)) as AssetContainerControl;

            assetContainer.AssetNameTextBlock.Text = assetViewModel.Name;
            assetContainer.AssetTypeTextRun.Text = assetViewModel.Type;

            // Resolve active control by name: NAMING CONVENTION REQUIRED
            var view = _regionManager.Regions["AssetContainerRegion"]
                                     .Views.First(v => v.GetType().Name == assetViewModel.Type) as UserControl;

            view.DataContext = viewModel;

            // Unblock the undo service
            _undoService.UnBlock();
        }
        private void LoadConstruction(string constructionName)
        {
            _regionManager.RequestNavigate("DesignRegion", constructionName);

            var view = _regionManager.Regions["DesignRegion"]
                                     .Views
                                     .First(x => x.GetType().Name == constructionName) as UserControl;

            view.DataContext = _scenarioEditorController.CurrentConfig;
        }
        private void PublishScenarioUpdate()
        {
            _eventAggregator.GetEvent<ScenarioUpdateEvent>().Publish(_scenarioEditorController.CurrentConfig);
        }
    }
}

using Prism.Mef.Modularity;
using Prism.Modularity;
using Prism.Regions;
using Rogue.NET.Common.Events.Scenario;
using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Common.Extension.Prism.RegionManager.Interface;
using Rogue.NET.ScenarioEditor.Controller.Interface;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.Service.Interface;
using Rogue.NET.ScenarioEditor.Utility;
using Rogue.NET.ScenarioEditor.ViewModel.Constant;
using Rogue.NET.ScenarioEditor.ViewModel.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
using Rogue.NET.ScenarioEditor.Views;
using Rogue.NET.ScenarioEditor.Views.Constants;
using Rogue.NET.ScenarioEditor.Views.Construction;
using Rogue.NET.ScenarioEditor.Views.Controls;
using Rogue.NET.ScenarioEditor.Views.DesignRegion;
using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;

namespace Rogue.NET.ScenarioEditor
{
    [ModuleExport("ScenarioEditorModule", typeof(ScenarioEditorModule))]
    public class ScenarioEditorModule : IModule
    {
        readonly IRogueEventAggregator _eventAggregator;
        readonly IRegionManager _regionManagerOLD;
        readonly IRogueRegionManager _regionManager;
        readonly IScenarioAssetController _scenarioAssetController;
        readonly IScenarioEditorController _scenarioEditorController;
        readonly IScenarioConfigurationUndoService _undoService;
        readonly IScenarioAssetReferenceService _scenarioAssetReferenceService;

        [ImportingConstructor]
        public ScenarioEditorModule(
            IRegionManager regionManagerOLD,
            IRogueRegionManager regionManager,
            IRogueEventAggregator eventAggregator,
            IScenarioAssetController scenarioAssetController,
            IScenarioEditorController scenarioEditorController,
            IScenarioConfigurationUndoService scenarioConfigurationUndoService,
            IScenarioAssetReferenceService scenarioAssetReferenceService)
        {
            _regionManagerOLD = regionManagerOLD;
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            _scenarioAssetController = scenarioAssetController;
            _scenarioEditorController = scenarioEditorController;
            _undoService = scenarioConfigurationUndoService;
            _scenarioAssetReferenceService = scenarioAssetReferenceService;
        }

        public void Initialize()
        {
            // Register views with OLD IRegionManager
            _regionManagerOLD.RegisterViewWithRegion("MainRegion", typeof(Editor));

            RegisterEvents();
        }

        private void RegisterEvents()
        {
            // Scenario Editor Events
            _eventAggregator.GetEvent<EditScenarioEvent>().Subscribe(() =>
            {
                _regionManagerOLD.RequestNavigate("MainRegion", "Editor");

                // Create an instance of the config so that there aren't any null refs.
                _scenarioEditorController.New();
            });
            _eventAggregator.GetEvent<LoadDifficultyChartEvent>().Subscribe((region) =>
            {
                // TODO:REGIONMANAGER Not sure whether to have a data context object passed with the event
                // _regionManager.Load(region, typeof(ScenarioDesignOverview));
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
                _regionManager.LoadSingleInstance(RegionNames.DesignRegion, typeof(EditorInstructions), null);

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
            _eventAggregator.GetEvent<LoadConstructionEvent>().Subscribe((type) =>
            {
                LoadConstruction(type);
            });

            // General Assets - (Attack Attributes, Character Classes, Altered States, and Brushes)
            //                  These have shared lists in the Scenario Configuration and have to be
            //                  managed accordingly
            _eventAggregator.GetEvent<AddGeneralAssetEvent>().Subscribe((e) =>
            {
                // NOTE*** THIS CAUSES MANY CHANGES TO THE MODEL. REQUIRES AN UNDO BLOCK AND CLEARING OF 
                //         THE STACK
                _undoService.Block();

                // Add asset to the scenario
                if (e is AttackAttributeTemplateViewModel)
                {
                    // Add Attack Attribute Asset
                    _scenarioEditorController.CurrentConfig.AttackAttributes.Add(e as AttackAttributeTemplateViewModel);

                    // Update Scenario object references
                    _scenarioAssetReferenceService.UpdateAttackAttributes(_scenarioEditorController.CurrentConfig);
                }

                else if (e is CharacterClassTemplateViewModel)
                {
                    // Add Character Class Asset
                    _scenarioEditorController.CurrentConfig.CharacterClasses.Add(e as CharacterClassTemplateViewModel);

                    // Update Scenario object references
                    _scenarioAssetReferenceService.UpdateCharacterClasses(_scenarioEditorController.CurrentConfig);
                }

                else if (e is AlteredCharacterStateTemplateViewModel)
                {
                    // Add Altered Character State
                    _scenarioEditorController.CurrentConfig.AlteredCharacterStates.Add(e as AlteredCharacterStateTemplateViewModel);

                    // Update Scenario object references
                    _scenarioAssetReferenceService.UpdateAlteredCharacterStates(_scenarioEditorController.CurrentConfig);
                }

                else if (e is BrushTemplateViewModel)
                {
                    // Add Brush
                    _scenarioEditorController.CurrentConfig.BrushTemplates.Add(e as BrushTemplateViewModel);

                    // Update Scenario object references
                    _scenarioAssetReferenceService.UpdateBrushes(_scenarioEditorController.CurrentConfig);
                }

                else
                    throw new Exception("Unhandled General Asset Type");

                // Allow undo changes again - and clear the stack to prevent old references to Attack Attributes
                _undoService.UnBlock();
                _undoService.Clear();

                // Reload designer
                LoadConstruction(typeof(General));
            });
            _eventAggregator.GetEvent<RemoveGeneralAssetEvent>().Subscribe((e) =>
            {
                // NOTE*** THIS CAUSES MANY CHANGES TO THE MODEL. REQUIRES AN UNDO BLOCK AND CLEARING OF 
                //         THE STACK
                _undoService.Block();

                // Add asset to the scenario
                if (e is AttackAttributeTemplateViewModel)
                {
                    // Add Attack Attribute Asset
                    _scenarioEditorController.CurrentConfig.AttackAttributes.Remove(e as AttackAttributeTemplateViewModel);

                    // Update Scenario object references
                    _scenarioAssetReferenceService.UpdateAttackAttributes(_scenarioEditorController.CurrentConfig);
                }

                else if (e is CharacterClassTemplateViewModel)
                {
                    // Add Character Class Asset
                    _scenarioEditorController.CurrentConfig.CharacterClasses.Remove(e as CharacterClassTemplateViewModel);

                    // Update Scenario object references
                    _scenarioAssetReferenceService.UpdateCharacterClasses(_scenarioEditorController.CurrentConfig);
                }

                else if (e is AlteredCharacterStateTemplateViewModel)
                {
                    // Add Altered Character State
                    _scenarioEditorController.CurrentConfig.AlteredCharacterStates.Remove(e as AlteredCharacterStateTemplateViewModel);

                    // Update Scenario object references
                    _scenarioAssetReferenceService.UpdateAlteredCharacterStates(_scenarioEditorController.CurrentConfig);
                }

                else if (e is BrushTemplateViewModel)
                {
                    // Add Brush
                    _scenarioEditorController.CurrentConfig.BrushTemplates.Remove(e as BrushTemplateViewModel);

                    // Update Scenario object references
                    _scenarioAssetReferenceService.UpdateBrushes(_scenarioEditorController.CurrentConfig);
                }

                else
                    throw new Exception("Unhandled General Asset Type");

                // Allow undo changes again - and clear the stack to prevent old references to Attack Attributes
                _undoService.UnBlock();
                _undoService.Clear();

                // Reload designer
                LoadConstruction(typeof(General));
            });

            // Alteration Effect Events
            _eventAggregator.GetEvent<AlterationEffectLoadRequestEvent>().Subscribe((container, e) =>
            {
                if (Xceed.Wpf
                         .Toolkit
                         .MessageBox
                         .Show("This will erase the current effect data. Are you sure?",
                               "Confirm Create Effect", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    // Construct the new alteration effect
                    var alterationEffect = e.AlterationEffectType.Construct();

                    // Load the Region
                    _regionManager.Load(container, e.AlterationEffectViewType, alterationEffect);

                    // Send response message
                    _eventAggregator.GetEvent<AlterationEffectLoadResponseEvent>()
                                    .Publish(new AlterationEffectLoadResponseEventArgs()
                                    {
                                        AlterationEffect = alterationEffect
                                    });
                }
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
            var viewType = AssetType.AssetViewTypes[assetViewModel.Type];

            // Request navigate to load the control (These are by type string)
            _regionManager.LoadSingleInstance(RegionNames.DesignRegion, typeof(AssetContainerControl), assetViewModel);
            _regionManager.LoadSingleInstance(RegionNames.AssetContainerRegion, viewType, viewModel);

            // Unblock the undo service
            _undoService.UnBlock();
        }
        private void LoadConstruction(Type constructionType)
        {
            // Load Design Region with Construction Control
            _regionManager.LoadSingleInstance(RegionNames.DesignRegion, 
                                              constructionType, 
                                              _scenarioEditorController.CurrentConfig);
        }
        private void PublishScenarioUpdate()
        {
            _eventAggregator.GetEvent<ScenarioUpdateEvent>().Publish(_scenarioEditorController.CurrentConfig);
        }
    }
}

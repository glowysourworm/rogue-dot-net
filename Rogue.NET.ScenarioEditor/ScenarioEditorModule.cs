using Prism.Mef.Modularity;
using Prism.Modularity;
using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Common.Extension.Prism.RegionManager.Interface;
using Rogue.NET.Core.Processing.Event.Scenario;
using Rogue.NET.ScenarioEditor.Controller.Interface;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.Service.Interface;
using Rogue.NET.ScenarioEditor.Utility;
using Rogue.NET.ScenarioEditor.ViewModel.Constant;
using Rogue.NET.ScenarioEditor.ViewModel.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Common;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
using Rogue.NET.ScenarioEditor.Views;
using Rogue.NET.ScenarioEditor.Views.Constants;
using Rogue.NET.ScenarioEditor.Views.Construction;
using Rogue.NET.ScenarioEditor.Views.Controls;
using Rogue.NET.ScenarioEditor.Views.DesignRegion;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;

namespace Rogue.NET.ScenarioEditor
{
    [ModuleExport("ScenarioEditorModule", typeof(ScenarioEditorModule))]
    public class ScenarioEditorModule : IModule
    {
        readonly IRogueEventAggregator _eventAggregator;
        readonly IRogueRegionManager _regionManager;
        readonly IScenarioAssetController _scenarioAssetController;
        readonly IScenarioEditorController _scenarioEditorController;
        readonly IScenarioConfigurationUndoService _undoService;
        readonly IScenarioAssetReferenceService _scenarioAssetReferenceService;
        readonly IScenarioCollectionProvider _scenarioCollectionProvider;

        [ImportingConstructor]
        public ScenarioEditorModule(
            IRogueRegionManager regionManager,
            IRogueEventAggregator eventAggregator,
            IScenarioAssetController scenarioAssetController,
            IScenarioEditorController scenarioEditorController,
            IScenarioConfigurationUndoService scenarioConfigurationUndoService,
            IScenarioAssetReferenceService scenarioAssetReferenceService,
            IScenarioCollectionProvider scenarioCollectionProvider)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            _scenarioAssetController = scenarioAssetController;
            _scenarioEditorController = scenarioEditorController;
            _undoService = scenarioConfigurationUndoService;
            _scenarioAssetReferenceService = scenarioAssetReferenceService;
            _scenarioCollectionProvider = scenarioCollectionProvider;
        }

        public void Initialize()
        {
            _regionManager.PreRegisterView(RegionNames.MainRegion, typeof(Editor));

            RegisterEvents();
        }

        private void RegisterEvents()
        {
            // Scenario Editor Events
            _eventAggregator.GetEvent<EditScenarioEvent>().Subscribe(() =>
            {
                _regionManager.LoadSingleInstance(RegionNames.MainRegion, typeof(Editor));

                // Create an instance of the config so that there aren't any null refs.
                _scenarioEditorController.New();
            });
            _eventAggregator.GetEvent<LoadDifficultyChartEvent>().Subscribe(() =>
            {
                _regionManager.LoadSingleInstance(RegionNames.DesignRegion, typeof(ScenarioDesignOverview));
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
                _regionManager.LoadSingleInstance(RegionNames.DesignRegion, typeof(EditorInstructions));

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

                else if (e is PlayerTemplateViewModel)
                {
                    // Add Character Class Asset
                    _scenarioEditorController.CurrentConfig.PlayerTemplates.Add(e as PlayerTemplateViewModel);

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

                else if (e is PlayerTemplateViewModel)
                {
                    // Add Character Class Asset
                    _scenarioEditorController.CurrentConfig.PlayerTemplates.Remove(e as PlayerTemplateViewModel);

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

                else
                    throw new Exception("Unhandled General Asset Type");

                // Allow undo changes again - and clear the stack to prevent old references to Attack Attributes
                _undoService.UnBlock();
                _undoService.Clear();

                // Reload designer
                LoadConstruction(typeof(General));
            });

            // Alteration Effect Events
            _eventAggregator.GetEvent<LoadNewAlterationEffectRequestEvent>().Subscribe((container, e) =>
            {
                if (Xceed.Wpf
                         .Toolkit
                         .MessageBox
                         .Show("This will erase the current effect data. Are you sure?",
                               "Confirm Create Effect", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    // Construct the new alteration effect
                    var alterationEffect = (IAlterationEffectTemplateViewModel)e.AlterationEffectType.Construct();

                    // Add Attack Attributes (SHOULD MOVE THIS)
                    //
                    // NOTE*** This needs to be done because of shared attack attribute assets. Design
                    //         needs to be changed to avoid shared collections
                    //
                    // TODO: CLEAN THIS UP
                    if (alterationEffect is AttackAttributeAuraAlterationEffectTemplateViewModel)
                        (alterationEffect as AttackAttributeAuraAlterationEffectTemplateViewModel).AttackAttributes.AddRange(_scenarioEditorController.CurrentConfig.AttackAttributes.Select(x => x.DeepClone()));

                    else if (alterationEffect is AttackAttributeMeleeAlterationEffectTemplateViewModel)
                        (alterationEffect as AttackAttributeMeleeAlterationEffectTemplateViewModel).AttackAttributes.AddRange(_scenarioEditorController.CurrentConfig.AttackAttributes.Select(x => x.DeepClone()));

                    else if (alterationEffect is AttackAttributePassiveAlterationEffectTemplateViewModel)
                        (alterationEffect as AttackAttributePassiveAlterationEffectTemplateViewModel).AttackAttributes.AddRange(_scenarioEditorController.CurrentConfig.AttackAttributes.Select(x => x.DeepClone()));

                    else if (alterationEffect is AttackAttributeTemporaryAlterationEffectTemplateViewModel)
                        (alterationEffect as AttackAttributeTemporaryAlterationEffectTemplateViewModel).AttackAttributes.AddRange(_scenarioEditorController.CurrentConfig.AttackAttributes.Select(x => x.DeepClone()));

                    else if (alterationEffect is EquipmentEnhanceAlterationEffectTemplateViewModel)
                        (alterationEffect as EquipmentEnhanceAlterationEffectTemplateViewModel).AttackAttributes.AddRange(_scenarioEditorController.CurrentConfig.AttackAttributes.Select(x => x.DeepClone()));

                    else if (alterationEffect is EquipmentDamageAlterationEffectTemplateViewModel)
                        (alterationEffect as EquipmentDamageAlterationEffectTemplateViewModel).AttackAttributes.AddRange(_scenarioEditorController.CurrentConfig.AttackAttributes.Select(x => x.DeepClone()));

                    // Load the Region
                    var view = _regionManager.Load(container, e.AlterationEffectViewType);

                    // Send Update Event
                    //
                    _eventAggregator.GetEvent<AlterationEffectChangedEvent>()
                                    .Publish(new AlterationEffectChangedEventArgs()
                                    {
                                        Alteration = e.Alteration,
                                        Effect = alterationEffect                                        
                                    });

                    // TODO:   Undo Service doesn't support recursive hooking of complex
                    //         property changes. These are only hooked up once when the
                    //         tree is traversed the first time.
                    //
                    //         This should be fixed so that the new alteration effect will
                    //         be hooked up when it's INotifyPropertyChanged event is raised.
                    //
                    //         This re-register call is a workaround.
                    _undoService.Register(_scenarioEditorController.CurrentConfig);
                }
            });
            _eventAggregator.GetEvent<LoadAlterationEffectRequestEvent>().Subscribe((container, e) =>
            {
                // Load the Region
                //
                // ***NOTE  No response event required because no new effect was constructed
                var view = _regionManager.Load(container, e.AlterationEffectViewType);
            });

            // Brush Events
            _eventAggregator.GetEvent<BrushAddedEvent>().Subscribe(brush =>
            {
                _scenarioCollectionProvider.AddBrush(brush);
            });
            _eventAggregator.GetEvent<BrushRemovedEvent>().Subscribe(brush =>
            {
                _scenarioCollectionProvider.RemoveBrush(brush);
            });

            // Region Events
            _eventAggregator.GetEvent<LoadDefaultRegionViewEvent>().Subscribe(region =>
            {
                // Loads the default view (instance or type) for the specified region
                _regionManager.LoadDefaultView(region);
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
            var assetContainerView = _regionManager.LoadSingleInstance(RegionNames.DesignRegion, typeof(AssetContainerControl));
            var assetView = _regionManager.LoadSingleInstance(RegionNames.AssetContainerRegion, viewType);

            assetContainerView.DataContext = assetViewModel;
            assetView.DataContext = viewModel;

            // Unblock the undo service
            _undoService.UnBlock();
        }
        private void LoadConstruction(Type constructionType)
        {
            // Load Design Region with Construction Control
            var view = _regionManager.LoadSingleInstance(RegionNames.DesignRegion, 
                                                         constructionType);

            // Set Data Context
            view.DataContext = _scenarioEditorController.CurrentConfig;
        }
        private void PublishScenarioUpdate()
        {
            // Publish update to provide service for views that have derived lists
            _eventAggregator.GetEvent<ScenarioUpdateEvent>().Publish(_scenarioCollectionProvider);
        }
    }
}

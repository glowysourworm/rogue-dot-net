using Prism.Mef.Modularity;
using Prism.Modularity;
using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Common.Extension.Prism.RegionManager.Interface;
using Rogue.NET.Core.Processing.Event.Scenario;
using Rogue.NET.ScenarioEditor.Controller.Interface;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.Events.Asset;
using Rogue.NET.ScenarioEditor.Events.Asset.Alteration;
using Rogue.NET.ScenarioEditor.Events.Browser;
using Rogue.NET.ScenarioEditor.Service.Interface;
using Rogue.NET.ScenarioEditor.Utility;
using Rogue.NET.ScenarioEditor.ViewModel.Attribute;
using Rogue.NET.ScenarioEditor.ViewModel.Browser.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Common;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Design;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Layout;
using Rogue.NET.ScenarioEditor.Views;
using Rogue.NET.ScenarioEditor.Views.Assets.SharedControl;
using Rogue.NET.ScenarioEditor.Views.Browser;
using Rogue.NET.ScenarioEditor.Views.Constants;
using Rogue.NET.ScenarioEditor.Views.Controls;
using Rogue.NET.ScenarioEditor.Views.Design;
using Rogue.NET.ScenarioEditor.Views.Design.LevelBranchDesign;
using Rogue.NET.ScenarioEditor.Views.DesignRegion;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;

using MessageBox = Xceed.Wpf.Toolkit.MessageBox;

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
            _regionManager.PreRegisterView(RegionNames.BrowserRegion, typeof(ScenarioLevelBrowser));

            RegisterEditorEvents();
            RegisterAssetEvents();
            RegisterAssetBrowserEvents();
            RegisterGeneralAssetEvents();
            RegisterLevelDesignEvents();
            RegisterAlterationEffectEvents();
        }
        private void RegisterEditorEvents()
        {
            // Scenario Editor Events
            _eventAggregator.GetEvent<EditScenarioEvent>().Subscribe(() =>
            {
                _regionManager.LoadSingleInstance(RegionNames.MainRegion, typeof(Editor));

                // Create an instance of the config so that there aren't any null refs.
                _scenarioEditorController.New();
            });

            // Design Mode Events
            _eventAggregator.GetEvent<LoadDesignEvent>().Subscribe((type) =>
            {
                ChangeDesignMode(type);
            });

            // Region Events
            _eventAggregator.GetEvent<LoadDefaultRegionViewEvent>().Subscribe(region =>
            {
                // Loads the default view (instance or type) for the specified region
                _regionManager.LoadDefaultView(region);
            });
        }
        private void RegisterAssetEvents()
        {
            // Rename event for scenario assets
            _eventAggregator.GetEvent<RenameScenarioAssetEvent>().Subscribe(asset =>
            {
                // Show rename control
                var view = new RenameControl();

                view.DataContext = asset;

                DialogWindowFactory.Show(view, "Rename Scenario Asset");

                // Publish a special event to update source lists for specific views
                PublishScenarioUpdate();
            });
        }
        private void RegisterAssetBrowserEvents()
        {
            // Asset Events
            _eventAggregator.GetEvent<AddAssetEvent>().Subscribe((e) =>
            {
                _scenarioAssetController.AddAsset(e.AssetType, e.AssetUniqueName);

                //Publish a special event to update source lists for specific views
                PublishScenarioUpdate();
            });
            _eventAggregator.GetEvent<LoadAssetEvent>().Subscribe((e) =>
            {
                LoadAsset(e);
            });
            _eventAggregator.GetEvent<RemoveAssetEvent>().Subscribe((e) =>
            {
                _scenarioAssetController.RemoveAsset(e.AssetType, e.Name);

                // Load the Editor Instructions to prevent editing removed asset
                _regionManager.LoadSingleInstance(RegionNames.DesignRegion, typeof(EditorInstructions));

                // Publish a special event to update source lists for specific views
                PublishScenarioUpdate();
            });
            _eventAggregator.GetEvent<RenameAssetEvent>().Subscribe((e) =>
            {
                // Show rename control
                var view = new RenameControl();
                var asset = _scenarioAssetController.GetAsset(e.Name, e.AssetType);

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
                IEnumerable<string> existingNames;
                var desiredAssetName = "Copy of " + e.Name;

                if (e.AssetType == typeof(PlayerTemplateViewModel))
                    existingNames = _scenarioCollectionProvider.PlayerClasses.Select(x => x.Name);

                else if (e.AssetType == typeof(EquipmentTemplateViewModel))
                    existingNames = _scenarioCollectionProvider.Equipment.Select(x => x.Name);

                else if (e.AssetType == typeof(ConsumableTemplateViewModel))
                    existingNames = _scenarioCollectionProvider.Consumables.Select(x => x.Name);

                else if (e.AssetType == typeof(EnemyTemplateViewModel))
                    existingNames = _scenarioCollectionProvider.Enemies.Select(x => x.Name);

                else if (e.AssetType == typeof(FriendlyTemplateViewModel))
                    existingNames = _scenarioCollectionProvider.Friendlies.Select(x => x.Name);

                else if (e.AssetType == typeof(DoodadTemplateViewModel))
                    existingNames = _scenarioCollectionProvider.Doodads.Select(x => x.Name);

                else if (e.AssetType == typeof(LayoutTemplateViewModel))
                    existingNames = _scenarioCollectionProvider.Layouts.Select(x => x.Name);

                else if (e.AssetType == typeof(SkillSetTemplateViewModel))
                    existingNames = _scenarioCollectionProvider.SkillSets.Select(x => x.Name);

                else
                    throw new Exception("Unhandled Asset Type");

                // Create a copy of the asset
                _scenarioAssetController.CopyAsset(e.Name, NameGenerator.Get(existingNames, desiredAssetName), e.AssetType);

                // Publish a special event to update source lists for specific views
                PublishScenarioUpdate();
            });
        }
        private void RegisterGeneralAssetEvents()
        {
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
                    _scenarioAssetReferenceService.UpdateAttackAttributes();
                }

                else if (e is PlayerTemplateViewModel)
                {
                    // Add Character Class Asset
                    _scenarioEditorController.CurrentConfig.PlayerTemplates.Add(e as PlayerTemplateViewModel);

                    // Update Scenario object references
                    _scenarioAssetReferenceService.UpdatePlayerClasses();
                }

                else if (e is AlteredCharacterStateTemplateViewModel)
                {
                    // Add Altered Character State
                    _scenarioEditorController.CurrentConfig.AlteredCharacterStates.Add(e as AlteredCharacterStateTemplateViewModel);

                    // Update Scenario object references
                    _scenarioAssetReferenceService.UpdateAlteredCharacterStates();
                }

                else
                    throw new Exception("Unhandled General Asset Type");

                // Allow undo changes again - and clear the stack to prevent old references to Attack Attributes
                _undoService.UnBlock();
                _undoService.Clear();

                // Reload designer
                ChangeDesignMode(DesignMode.General);
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
                    _scenarioAssetReferenceService.UpdateAttackAttributes();
                }

                else if (e is PlayerTemplateViewModel)
                {
                    // Add Character Class Asset
                    _scenarioEditorController.CurrentConfig.PlayerTemplates.Remove(e as PlayerTemplateViewModel);

                    // Update Scenario object references
                    _scenarioAssetReferenceService.UpdatePlayerClasses();
                }

                else if (e is AlteredCharacterStateTemplateViewModel)
                {
                    // Add Altered Character State
                    _scenarioEditorController.CurrentConfig.AlteredCharacterStates.Remove(e as AlteredCharacterStateTemplateViewModel);

                    // Update Scenario object references
                    _scenarioAssetReferenceService.UpdateAlteredCharacterStates();
                }

                else
                    throw new Exception("Unhandled General Asset Type");

                // Allow undo changes again - and clear the stack to prevent old references to Attack Attributes
                _undoService.UnBlock();
                _undoService.Clear();

                // Reload designer
                ChangeDesignMode(DesignMode.General);
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
        }
        private void RegisterLevelDesignEvents()
        {
            // Level Events
            _eventAggregator.GetEvent<AddLevelEvent>().Subscribe(() =>
            {
                var levelNumber = _scenarioEditorController.CurrentConfig
                                                           .ScenarioDesign
                                                           .LevelDesigns
                                                           .Count + 1;

                _scenarioEditorController.CurrentConfig
                                         .ScenarioDesign
                                         .LevelDesigns
                                         .Add(new LevelTemplateViewModel()
                                         {
                                             Name = "Level " + levelNumber.ToString()
                                         });

                PublishScenarioUpdate();
            });

            _eventAggregator.GetEvent<RemoveLevelEvent>().Subscribe(levelViewModel =>
            {
                var template = _scenarioEditorController.CurrentConfig
                                                        .ScenarioDesign
                                                        .LevelDesigns
                                                        .FirstOrDefault(x => x.Name == levelViewModel.Name);

                _scenarioEditorController.CurrentConfig
                                         .ScenarioDesign
                                         .LevelDesigns
                                         .Remove(template);


                // Rename levels
                var counter = 1;
                foreach (var levelTemplate in _scenarioEditorController.CurrentConfig
                                                                       .ScenarioDesign
                                                                       .LevelDesigns)
                {
                    levelTemplate.Name = "Level " + counter++;
                }

                PublishScenarioUpdate();
            });

            _eventAggregator.GetEvent<LoadLevelEvent>().Subscribe(levelViewModel =>
            {
                var template = _scenarioEditorController.CurrentConfig
                                                        .ScenarioDesign
                                                        .LevelDesigns
                                                        .FirstOrDefault(x => x.Name == levelViewModel.Name);

                // Load Design Container
                _regionManager.LoadSingleInstance(RegionNames.DesignRegion, typeof(DesignContainer)).DataContext = template;
                _regionManager.LoadSingleInstance(RegionNames.DesignContainerRegion, typeof(LevelDesign));
            });

            // Level Branch Events
            _eventAggregator.GetEvent<AddLevelBranchEvent>().Subscribe(eventData =>
            {
                // Get the level for this branch
                var template = _scenarioEditorController.CurrentConfig
                                                        .ScenarioDesign
                                                        .LevelDesigns
                                                        .First(x => x.Name == eventData.LevelName);

                // Add new branch to the level
                var branchTemplate = new LevelBranchGenerationTemplateViewModel()
                {
                    Name = eventData.LevelBranchUniqueName
                };

                // Copy branch name to the actual branch
                branchTemplate.LevelBranch.Name = eventData.LevelBranchUniqueName;

                template.LevelBranches.Add(branchTemplate);

                PublishScenarioUpdate();
            });

            _eventAggregator.GetEvent<CopyLevelBranchEvent>().Subscribe(eventData =>
            {
                // Get the level template
                var levelTemplate = _scenarioEditorController.CurrentConfig
                                                             .ScenarioDesign
                                                             .LevelDesigns
                                                             .FirstOrDefault(x => x.Name == eventData.LevelName);

                // Get the level branch template
                var branchTemplate = levelTemplate.LevelBranches.First(x => x.Name == eventData.LevelBranchName);

                // Create unique name for the branch
                var uniqueName = NameGenerator.Get(levelTemplate.LevelBranches.Select(x => x.Name), levelTemplate.Name + " Branch");

                // Copy the branch data references
                var branchTemplateCopy = CopyLevelBranch(branchTemplate, uniqueName);

                // Add level branch copy to level
                levelTemplate.LevelBranches.Add(branchTemplateCopy);

                PublishScenarioUpdate();
            });

            _eventAggregator.GetEvent<LoadLevelBranchAssetsEvent>().Subscribe(eventData =>
            {
                // Get the level template
                var levelTemplate = _scenarioEditorController.CurrentConfig
                                                             .ScenarioDesign
                                                             .LevelDesigns
                                                             .First(x => x.LevelBranches.Any(z => z.LevelBranch.Name == eventData.LevelBranchName));

                // Get the level branch template
                var branchTemplate = levelTemplate.LevelBranches.First(x => x.LevelBranch.Name == eventData.LevelBranchName);

                // ***NOTE  Setting data context for the design views as the level branch
                //

                // Load Design Container
                _regionManager.LoadSingleInstance(RegionNames.DesignRegion, typeof(DesignContainer)).DataContext = branchTemplate.LevelBranch;

                // Load the level branch view
                if (eventData.AssetType == typeof(ConsumableTemplateViewModel))
                    _regionManager.LoadSingleInstance(RegionNames.DesignContainerRegion, typeof(ConsumableDesign));

                else if (eventData.AssetType == typeof(DoodadTemplateViewModel))
                    _regionManager.LoadSingleInstance(RegionNames.DesignContainerRegion, typeof(DoodadDesign)).DataContext = branchTemplate.LevelBranch;

                else if (eventData.AssetType == typeof(EnemyTemplateViewModel))
                    _regionManager.LoadSingleInstance(RegionNames.DesignContainerRegion, typeof(EnemyDesign)).DataContext = branchTemplate.LevelBranch;

                else if (eventData.AssetType == typeof(EquipmentTemplateViewModel))
                    _regionManager.LoadSingleInstance(RegionNames.DesignContainerRegion, typeof(EquipmentDesign)).DataContext = branchTemplate.LevelBranch;

                else if (eventData.AssetType == typeof(FriendlyTemplateViewModel))
                    _regionManager.LoadSingleInstance(RegionNames.DesignContainerRegion, typeof(FriendlyDesign)).DataContext = branchTemplate.LevelBranch;

                else if (eventData.AssetType == typeof(LayoutTemplateViewModel))
                    _regionManager.LoadSingleInstance(RegionNames.DesignContainerRegion, typeof(LayoutDesign)).DataContext = branchTemplate.LevelBranch;

                else
                    throw new Exception("Unknown Level Branch Asset Type ScenarioEditorModule");
            });

            _eventAggregator.GetEvent<RenameLevelBranchEvent>().Subscribe(levelBranchViewModel =>
            {
                // Get the level template
                var levelTemplate = _scenarioEditorController.CurrentConfig
                                                             .ScenarioDesign
                                                             .LevelDesigns
                                                             .First(x => x.LevelBranches.Any(z => z.LevelBranch.Name == levelBranchViewModel.Name));

                // Get the level branch template
                var branchTemplate = levelTemplate.LevelBranches.First(x => x.LevelBranch.Name == levelBranchViewModel.Name);

                // Show rename control
                var view = new RenameControl();

                view.DataContext = branchTemplate;

                DialogWindowFactory.Show(view, "Rename Level Branch");

                // Update level branch Name (from the generation view model name)
                branchTemplate.LevelBranch.Name = branchTemplate.Name;

                // Update view model level branch Name
                levelBranchViewModel.Name = branchTemplate.Name;
            });

            _eventAggregator.GetEvent<RemoveLevelBranchEvent>().Subscribe(eventData =>
            {
                // Get the level template
                var levelTemplate = _scenarioEditorController.CurrentConfig
                                                             .ScenarioDesign
                                                             .LevelDesigns
                                                             .First(x => x.LevelBranches.Any(z => z.LevelBranch.Name == eventData.LevelBranchName));

                // Get the level branch template
                var branchTemplate = levelTemplate.LevelBranches.First(x => x.LevelBranch.Name == eventData.LevelBranchName);

                if (levelTemplate.LevelBranches.Count <= 1)
                    MessageBox.Show("Levels must have at least one branch", "Cannot Delete Level Branch", MessageBoxButton.OK);

                // Remove the level Branch -> INotifyCollectionChanged (picks up the change)
                else
                    levelTemplate.LevelBranches.Remove(branchTemplate);

                PublishScenarioUpdate();
            });
        }
        private void RegisterAlterationEffectEvents()
        {
            // Alteration Effect Events
            _eventAggregator.GetEvent<LoadNewAlterationEffectRequestEvent>().Subscribe((container, e) =>
            {
                if (MessageBox.Show("This will erase the current effect data. Are you sure?",
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
        }

        /// <summary>
        /// Copies level branch asset references into a new level branch instance. Level branches are not deep-copied because
        /// asset references are shared.
        /// </summary>
        private LevelBranchGenerationTemplateViewModel CopyLevelBranch(
                    LevelBranchGenerationTemplateViewModel source, 
                    string copyName)
        {
            // Create copy of the level branch
            var copy = new LevelBranchGenerationTemplateViewModel();

            // Create new name for the branch
            copy.Name = copyName;

            // Copy new name to the actual branch
            copy.LevelBranch.Name = copy.Name;

            // Copy over asset references
            foreach (var consumable in source.LevelBranch.Consumables)
                copy.LevelBranch.Consumables.Add(consumable);

            foreach (var doodad in source.LevelBranch.Doodads)
                copy.LevelBranch.Doodads.Add(doodad);

            foreach (var enemy in source.LevelBranch.Enemies)
                copy.LevelBranch.Enemies.Add(enemy);

            foreach (var equipment in source.LevelBranch.Equipment)
                copy.LevelBranch.Equipment.Add(equipment);

            foreach (var friendly in source.LevelBranch.Friendlies)
                copy.LevelBranch.Friendlies.Add(friendly);

            foreach (var layout in source.LevelBranch.Layouts)
                copy.LevelBranch.Layouts.Add(layout);

            // Copy over other parameters
            copy.LevelBranch.ConsumableGenerationRange = new RangeViewModel<int>(source.LevelBranch.ConsumableGenerationRange);
            copy.LevelBranch.DoodadGenerationRange = new RangeViewModel<int>(source.LevelBranch.DoodadGenerationRange);
            copy.LevelBranch.EnemyGenerationRange = new RangeViewModel<int>(source.LevelBranch.EnemyGenerationRange);
            copy.LevelBranch.EquipmentGenerationRange = new RangeViewModel<int>(source.LevelBranch.EquipmentGenerationRange);
            copy.LevelBranch.FriendlyGenerationRange = new RangeViewModel<int>(source.LevelBranch.FriendlyGenerationRange);
            copy.LevelBranch.MonsterGenerationPerStep = source.LevelBranch.MonsterGenerationPerStep;

            return copy;
        }
        private void LoadAsset(IScenarioAssetReadonlyViewModel assetViewModel)
        {
            // KLUDGE:  This block is to prevent ComboBox Binding update issues. Events were firing when
            //          the view changed that were caught by the undo service. This should prevent those.
            _undoService.Block();

            // Get the asset for loading into the design region
            var viewModel = _scenarioAssetController.GetAsset(assetViewModel.Name, assetViewModel.AssetType);

            // Get the view name for this asset type
            var viewType = assetViewModel.AssetType.GetAttribute<UITypeAttribute>().ViewType;

            // Region load AssetContainerControl -> Load Sub-Region with Asset View
            var assetContainerView = _regionManager.LoadSingleInstance(RegionNames.DesignRegion, typeof(AssetContainerControl));
            var assetView = _regionManager.LoadSingleInstance(RegionNames.AssetContainerRegion, viewType);

            assetContainerView.DataContext = assetViewModel;
            assetView.DataContext = viewModel;

            // Unblock the undo service
            _undoService.UnBlock();
        }
        private void ChangeDesignMode(DesignMode mode)
        {
            // Load Design Region / Browser Region with appropriate view(s)
            switch (mode)
            {
                case DesignMode.General:
                    _regionManager.LoadSingleInstance(RegionNames.BrowserRegion, typeof(ScenarioAssetBrowser));
                    _regionManager.LoadSingleInstance(RegionNames.DesignRegion, typeof(General))
                                  .DataContext = _scenarioEditorController.CurrentConfig;
                    break;
                case DesignMode.Level:
                    _regionManager.LoadSingleInstance(RegionNames.BrowserRegion, typeof(ScenarioLevelBrowser));
                    _regionManager.LoadSingleInstance(RegionNames.DesignRegion, typeof(EditorInstructions));
                    break;
                case DesignMode.Overview:
                    _regionManager.LoadSingleInstance(RegionNames.BrowserRegion, typeof(ScenarioLevelBrowser));
                    _regionManager.LoadSingleInstance(RegionNames.DesignRegion, typeof(EditorInstructions));
                    break;
                case DesignMode.Validation:
                    _regionManager.LoadSingleInstance(RegionNames.BrowserRegion, typeof(ScenarioAssetBrowser));
                    _regionManager.LoadSingleInstance(RegionNames.DesignRegion, typeof(Validation));
                    break;
                default:
                    break;
            }
        }
        private void PublishScenarioUpdate()
        {
            // Publish update to provide service for views that have derived lists
            _eventAggregator.GetEvent<ScenarioUpdateEvent>().Publish(_scenarioCollectionProvider);
        }
    }
}

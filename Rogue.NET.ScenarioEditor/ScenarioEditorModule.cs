using Prism.Events;
using Prism.Mef.Modularity;
using Prism.Modularity;
using Prism.Regions;
using Rogue.NET.Common.Events.Scenario;
using Rogue.NET.Core.Service;
using Rogue.NET.Core.Service.Interface;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.Interface;
using Rogue.NET.ScenarioEditor.Utility;
using Rogue.NET.ScenarioEditor.ViewModel.Constant;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Layout;
using Rogue.NET.ScenarioEditor.Views;
using Rogue.NET.ScenarioEditor.Views.Assets;
using Rogue.NET.ScenarioEditor.Views.Construction;
using Rogue.NET.ScenarioEditor.Views.DesignRegion;
using System;
using System.Collections;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor
{
    [ModuleExport("ScenarioEditor", typeof(ScenarioEditorModule))]
    public class ScenarioEditorModule : IModule
    {
        readonly IEventAggregator _eventAggregator;
        readonly IRegionManager _regionManager;
        readonly IScenarioEditorController _controller;
        readonly IScenarioResourceService _resourceService;

        [ImportingConstructor]
        public ScenarioEditorModule(
            IRegionManager regionManager,
            IEventAggregator eventAggregator,
            IScenarioEditorController scenarioEditorController,
            IScenarioResourceService scenarioResourceService)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            _controller = scenarioEditorController;
            _resourceService = scenarioResourceService;
        }

        public void Initialize()
        {
            _resourceService.SetCacheMode(ResourceCacheMode.NoCache);

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
            _regionManager.RegisterViewWithRegion("AssetContainerRegion", typeof(Rogue.NET.ScenarioEditor.Views.Assets.Brush));
            _regionManager.RegisterViewWithRegion("AssetContainerRegion", typeof(Consumable));
            _regionManager.RegisterViewWithRegion("AssetContainerRegion", typeof(Doodad));
            _regionManager.RegisterViewWithRegion("AssetContainerRegion", typeof(Enemy));
            _regionManager.RegisterViewWithRegion("AssetContainerRegion", typeof(Equipment));
            _regionManager.RegisterViewWithRegion("AssetContainerRegion", typeof(Layout));
            _regionManager.RegisterViewWithRegion("AssetContainerRegion", typeof(SkillSet));
            _regionManager.RegisterViewWithRegion("AssetContainerRegion", typeof(Spell));


            // Design Region - Construction Views
            _regionManager.RegisterViewWithRegion("DesignRegion", typeof(General));
            _regionManager.RegisterViewWithRegion("DesignRegion", typeof(DungeonObjectPlacement));
            _regionManager.RegisterViewWithRegion("DesignRegion", typeof(EnemyPlacement));
            _regionManager.RegisterViewWithRegion("DesignRegion", typeof(ItemPlacement));
            _regionManager.RegisterViewWithRegion("DesignRegion", typeof(LayoutDesign));
            _regionManager.RegisterViewWithRegion("DesignRegion", typeof(ObjectiveDesign));
            _regionManager.RegisterViewWithRegion("DesignRegion", typeof(PlayerDesign));

            // Design Region - Difficulty View
            _regionManager.RegisterViewWithRegion("DesignRegion", typeof(ScenarioDifficultyChart));
        }
        private void RegisterEvents()
        {
            _eventAggregator.GetEvent<EditScenarioEvent>().Subscribe(() =>
            {
                _regionManager.RequestNavigate("MainRegion", "Editor");
                _regionManager.RequestNavigate("DesignRegion", "EditorInstructions");

                // Create an instance of the config so that there aren't any null refs.
                _controller.New();
            });

            _eventAggregator.GetEvent<LoadConstructionEvent>().Subscribe((e) =>
            {
                _regionManager.RequestNavigate("DesignRegion", e.ConstructionName);

                var view = _regionManager.Regions["DesignRegion"]
                                         .Views
                                         .First(x => x.GetType().Name == e.ConstructionName) as UserControl;

                view.DataContext = _controller.CurrentConfig;
            });
            _eventAggregator.GetEvent<ScoreScenarioEvent>().Subscribe(() =>
            {
                _regionManager.RequestNavigate("DesignRegion", "ScenarioDifficultyChart");
            });
            _eventAggregator.GetEvent<AddAssetEvent>().Subscribe((e) =>
            {
                AddAsset(e.AssetType, e.AssetUniqueName, e.SymbolDetails);
            });
            _eventAggregator.GetEvent<LoadAssetEvent>().Subscribe((e) =>
            {
                LoadAsset(e.Type, e.Name);
            });
            _eventAggregator.GetEvent<RemoveAssetEvent>().Subscribe((e) =>
            {
                RemoveAsset(e.Type, e.Name);
            });
        }

        /// <summary>
        /// Adds an asset with a pre-calculated name
        /// </summary>
        private void AddAsset(string assetType, string uniqueName, SymbolDetailsTemplateViewModel symbolDetails)
        {
            switch (assetType)
            {
                case AssetType.Layout:
                    _controller.CurrentConfig.DungeonTemplate.LayoutTemplates.Add(new LayoutTemplateViewModel() { Name = uniqueName });
                    break;
                case AssetType.Enemy:
                    _controller.CurrentConfig.EnemyTemplates.Add(new EnemyTemplateViewModel() { Name = uniqueName, SymbolDetails = symbolDetails });
                    break;
                case AssetType.Equipment:
                    _controller.CurrentConfig.EquipmentTemplates.Add(new EquipmentTemplateViewModel() { Name = uniqueName, SymbolDetails = symbolDetails });
                    break;
                case AssetType.Consumable:
                    _controller.CurrentConfig.ConsumableTemplates.Add(new ConsumableTemplateViewModel() { Name = uniqueName, SymbolDetails = symbolDetails });
                    break;
                case AssetType.Doodad:
                    _controller.CurrentConfig.DoodadTemplates.Add(new DoodadTemplateViewModel() { Name = uniqueName, SymbolDetails = symbolDetails });
                    break;
                case AssetType.Spell:
                    _controller.CurrentConfig.MagicSpells.Add(new SpellTemplateViewModel() { Name = uniqueName });
                    break;
                case AssetType.SkillSet:
                    _controller.CurrentConfig.SkillTemplates.Add(new SkillSetTemplateViewModel() { Name = uniqueName, SymbolDetails = symbolDetails });
                    break;
                case AssetType.Animation:
                    _controller.CurrentConfig.AnimationTemplates.Add(new AnimationTemplateViewModel() { Name = uniqueName });
                    break;
                case AssetType.Brush:
                    _controller.CurrentConfig.BrushTemplates.Add(new BrushTemplateViewModel() { Name = uniqueName });
                    break;
                default:
                    throw new Exception("Unidentified new asset type");
            }
        }
        private void RemoveAsset(string assetType, string name)
        {
            var collection = (IList)ConfigurationCollectionResolver.GetAssetCollection(_controller.CurrentConfig, assetType);

            var item = collection.Cast<TemplateViewModel>().First(x => x.Name == name);
            collection.Remove(item);
        }
        private void LoadAsset(string assetType, string name)
        {
            // Get the asset for loading into the design region
            var viewModel = GetAsset(name, assetType);

            // Get the view name for this asset type
            var viewName = AssetType.AssetViews[assetType];

            // Request navigate to load the control (These are by type string)
            _regionManager.RequestNavigate("DesignRegion", "AssetContainerControl");
            _regionManager.RequestNavigate("AssetContainerRegion", viewName);

            // Set parameters for Asset Container by hand
            var assetContainer = _regionManager.Regions["DesignRegion"]
                                               .Views.First(x => x.GetType() == typeof(AssetContainerControl)) as AssetContainerControl;

            assetContainer.AssetNameTextBlock.Text = name;
            assetContainer.AssetTypeTextRun.Text = assetType;

            // Resolve active control by name: NAMING CONVENTION REQUIRED
            var view = _regionManager.Regions["AssetContainerRegion"]
                                     .Views.First(v => v.GetType().Name == assetType) as UserControl;

            view.DataContext = viewModel;
        }
        private TemplateViewModel GetAsset(string name, string assetType)
        {
            var collection = (IList)ConfigurationCollectionResolver.GetAssetCollection(_controller.CurrentConfig, assetType);

            return collection.Cast<TemplateViewModel>().First(x => x.Name == name);
        }
    }
}

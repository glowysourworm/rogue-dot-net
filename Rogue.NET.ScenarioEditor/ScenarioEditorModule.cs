using Prism.Events;
using Prism.Mef.Modularity;
using Prism.Modularity;
using Prism.Regions;
using Rogue.NET.Common.Events.Scenario;
using Rogue.NET.Core.Service.Interface;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Layout;
using Rogue.NET.ScenarioEditor.Views;
using Rogue.NET.ScenarioEditor.Views.Assets;
using Rogue.NET.ScenarioEditor.Views.Construction;
using System;
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
            _regionManager.RegisterViewWithRegion("DesignRegion", typeof(Animation));
            _regionManager.RegisterViewWithRegion("DesignRegion", typeof(AttackAttribute));
            _regionManager.RegisterViewWithRegion("DesignRegion", typeof(Rogue.NET.ScenarioEditor.Views.Assets.Brush));
            _regionManager.RegisterViewWithRegion("DesignRegion", typeof(Consumable));
            _regionManager.RegisterViewWithRegion("DesignRegion", typeof(Doodad));
            _regionManager.RegisterViewWithRegion("DesignRegion", typeof(Enemy));
            _regionManager.RegisterViewWithRegion("DesignRegion", typeof(Equipment));
            _regionManager.RegisterViewWithRegion("DesignRegion", typeof(Layout));
            _regionManager.RegisterViewWithRegion("DesignRegion", typeof(SkillSet));
            _regionManager.RegisterViewWithRegion("DesignRegion", typeof(Spell));


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
                case "Layout":
                    _controller.CurrentConfig.DungeonTemplate.LayoutTemplates.Add(new LayoutTemplateViewModel() { Name = uniqueName });
                    break;
                case "AttackAttribute":
                    _controller.CurrentConfig.AttackAttributes.Add(new AttackAttributeTemplateViewModel() { Name = uniqueName, SymbolDetails = symbolDetails });
                    break;
                case "Enemy":
                    _controller.CurrentConfig.EnemyTemplates.Add(new EnemyTemplateViewModel() { Name = uniqueName, SymbolDetails = symbolDetails });
                    break;
                case "Equipment":
                    _controller.CurrentConfig.EquipmentTemplates.Add(new EquipmentTemplateViewModel() { Name = uniqueName, SymbolDetails = symbolDetails });
                    break;
                case "Consumable":
                    _controller.CurrentConfig.ConsumableTemplates.Add(new ConsumableTemplateViewModel() { Name = uniqueName, SymbolDetails = symbolDetails });
                    break;
                case "Doodad":
                    _controller.CurrentConfig.DoodadTemplates.Add(new DoodadTemplateViewModel() { Name = uniqueName, SymbolDetails = symbolDetails });
                    break;
                case "Spell":
                    _controller.CurrentConfig.MagicSpells.Add(new SpellTemplateViewModel() { Name = uniqueName });
                    break;
                case "SkillSet":
                    _controller.CurrentConfig.SkillTemplates.Add(new SkillSetTemplateViewModel() { Name = uniqueName, SymbolDetails = symbolDetails });
                    break;
                case "Animation":
                    _controller.CurrentConfig.AnimationTemplates.Add(new AnimationTemplateViewModel() { Name = uniqueName });
                    break;
                case "Brush":
                    _controller.CurrentConfig.BrushTemplates.Add(new BrushTemplateViewModel() { Name = uniqueName });
                    break;
                case "Pen":
                    _controller.CurrentConfig.PenTemplates.Add(new PenTemplateViewModel() { Name = uniqueName });
                    break;
                default:
                    throw new Exception("Unidentified new asset type");
            }
        }
        private bool RemoveAsset(string type, string name)
        {
            switch (type)
            {
                case "Layout":
                    {
                        var item = _controller.CurrentConfig.DungeonTemplate.LayoutTemplates.FirstOrDefault(x => x.Name == name);
                        return _controller.CurrentConfig.DungeonTemplate.LayoutTemplates.Remove(item);
                    }
                case "AttackAttribute":
                    {
                        var item = _controller.CurrentConfig.AttackAttributes.FirstOrDefault(x => x.Name == name);
                        return _controller.CurrentConfig.AttackAttributes.Remove(item);
                    }
                case "Enemy":
                    {
                        var item = _controller.CurrentConfig.EnemyTemplates.FirstOrDefault(x => x.Name == name);
                        return _controller.CurrentConfig.EnemyTemplates.Remove(item);
                    }
                case "Equipment":
                    {
                        var item = _controller.CurrentConfig.EquipmentTemplates.FirstOrDefault(x => x.Name == name);
                        return _controller.CurrentConfig.EquipmentTemplates.Remove(item);
                    }
                case "Consumable":
                    {
                        var item = _controller.CurrentConfig.ConsumableTemplates.FirstOrDefault(x => x.Name == name);
                        return _controller.CurrentConfig.ConsumableTemplates.Remove(item);
                    }
                case "Doodad":
                    {
                        var item = _controller.CurrentConfig.DoodadTemplates.FirstOrDefault(x => x.Name == name);
                        return _controller.CurrentConfig.DoodadTemplates.Remove(item);
                    }
                case "Spell":
                    {
                        var item = _controller.CurrentConfig.MagicSpells.FirstOrDefault(x => x.Name == name);
                        return _controller.CurrentConfig.MagicSpells.Remove(item);
                    }
                case "SkillSet":
                    {
                        var item = _controller.CurrentConfig.SkillTemplates.FirstOrDefault(x => x.Name == name);
                        return _controller.CurrentConfig.SkillTemplates.Remove(item);
                    }
                case "Animation":
                    {
                        var item = _controller.CurrentConfig.AnimationTemplates.FirstOrDefault(x => x.Name == name);
                        return _controller.CurrentConfig.AnimationTemplates.Remove(item);
                    }
                case "Brush":
                    {
                        var item = _controller.CurrentConfig.BrushTemplates.FirstOrDefault(x => x.Name == name);
                        return _controller.CurrentConfig.BrushTemplates.Remove(item);
                    }
                case "Pen":
                    {
                        var item = _controller.CurrentConfig.PenTemplates.FirstOrDefault(x => x.Name == name);
                        return _controller.CurrentConfig.PenTemplates.Remove(item);
                    }
                default:
                    break;
            }

            throw new Exception("Unidentified new asset type");
        }
        private void LoadAsset(string type, string name)
        {
            // Get the asset for loading into the design region
            var viewModel = GetAsset(name, type);

            // Request navigate to load the control (These are by type string)
            _regionManager.RequestNavigate("DesignRegion", type);

            // Resolve active control by name: NAMING CONVENTION REQUIRED
            var view = _regionManager.Regions["DesignRegion"]
                                     .Views.First(v => v.GetType().Name == type) as UserControl;

            view.DataContext = viewModel;
        }
        private bool UpdateAssetName(string oldName, string newName, string type)
        {
            if (string.IsNullOrEmpty(newName))
                return false;

            var asset = GetAsset(oldName, type);
            var existingAsset = GetAsset(newName, type);

            // can't allow duplicate name / type combinations
            if (existingAsset != null)
                return false;

            asset.Name = newName;
            return true;
        }
        private TemplateViewModel GetAsset(string name, string type)
        {
            switch (type)
            {
                case "Layout":
                    return _controller.CurrentConfig.DungeonTemplate.LayoutTemplates.FirstOrDefault(x => x.Name == name);
                case "SkillSet":
                    return _controller.CurrentConfig.SkillTemplates.FirstOrDefault(x => x.Name == name);
                case "Brush":
                    return _controller.CurrentConfig.BrushTemplates.FirstOrDefault(x => x.Name == name);
                case "Enemy":
                    return _controller.CurrentConfig.EnemyTemplates.FirstOrDefault(x => x.Name == name);
                case "Equipment":
                    return _controller.CurrentConfig.EquipmentTemplates.FirstOrDefault(x => x.Name == name);
                case "Consumable":
                    return _controller.CurrentConfig.ConsumableTemplates.FirstOrDefault(x => x.Name == name);
                case "Doodad":
                    return _controller.CurrentConfig.DoodadTemplates.FirstOrDefault(x => x.Name == name);
                case "Spell":
                    return _controller.CurrentConfig.MagicSpells.FirstOrDefault(x => x.Name == name);
                case "Animation":
                    return _controller.CurrentConfig.AnimationTemplates.FirstOrDefault(x => x.Name == name);
                default:
                    throw new ApplicationException("Unhandled Asset Type");
            }
        }
    }
}

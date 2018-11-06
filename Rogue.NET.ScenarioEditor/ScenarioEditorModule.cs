using Prism.Events;
using Prism.Mef.Modularity;
using Prism.Modularity;
using Prism.Regions;
using Rogue.NET.Common.Events.Scenario;
using Rogue.NET.Core.Service.Interface;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.Interface;
using Rogue.NET.ScenarioEditor.Utility;
using Rogue.NET.ScenarioEditor.ViewModel;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Layout;
using Rogue.NET.ScenarioEditor.Views;
using Rogue.NET.ScenarioEditor.Views.Assets;
using Rogue.NET.ScenarioEditor.Views.Assets.Animation;
using Rogue.NET.ScenarioEditor.Views.Assets.Consumable;
using Rogue.NET.ScenarioEditor.Views.Assets.Enemy;
using Rogue.NET.ScenarioEditor.Views.Assets.Equipment;
using Rogue.NET.ScenarioEditor.Views.Assets.Spell;
using Rogue.NET.ScenarioEditor.Views.Construction;
using Rogue.NET.ScenarioEditor.Views.Controls;
using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Controls;
using ConsumableSubType = Rogue.NET.ScenarioEditor.Views.Assets.Consumable.ConsumableSubType;

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
            _regionManager.RegisterViewWithRegion("DesignRegion", typeof(AnimationWizard));
            _regionManager.RegisterViewWithRegion("DesignRegion", typeof(ConsumableWizard));
            _regionManager.RegisterViewWithRegion("DesignRegion", typeof(EnemyWizard));
            _regionManager.RegisterViewWithRegion("DesignRegion", typeof(EquipmentWizard));
            _regionManager.RegisterViewWithRegion("DesignRegion", typeof(SpellWizard));
            _regionManager.RegisterViewWithRegion("AnimationWizardRegion", () =>
            {
                return new Wizard(new WizardViewModel()
                {
                    FirstPageType = typeof(AnimationBasicType),
                    Title = "Animation Creation Wizard",
                    WizardSteps = new string[] {
                            "Select Basic Type",
                            "Select Focus Type",
                            "Set Animation Parameters",
                            "Preview"
                    }
                });
            });
            _regionManager.RegisterViewWithRegion("ConsumableWizardRegion", () =>
            {
                return new Wizard(new WizardViewModel()
                {
                    FirstPageType = typeof(ConsumableSubType),
                    Title = "Consumable Wizard",
                    WizardSteps = new string[]{
                            "Select Type",
                            "Select Use Type",
                            "Select Spell Usage",
                            "Set Parameters",
                            "Set Rogue Encyclopedia Data",
                            "Edit Symbol"
                    }
                });
            });
            _regionManager.RegisterViewWithRegion("EnemyWizardRegion", () =>
            {
                return new Wizard(new WizardViewModel()
                {
                    FirstPageType = typeof(EnemyParameters),
                    Title = "Enemy Wizard",
                    WizardSteps = new string[]{
                            "Set Melee Parameters",
                            "Set Behavior",
                            "Set Attack Attributes",
                            "Set Items",
                            "Set Rogue Encyclopedia Data",
                            "Edit Symbol"
                    }
                });
            });
            _regionManager.RegisterViewWithRegion("EquipmentWizardRegion", () =>
            {
                return new Wizard(new WizardViewModel()
                {
                    FirstPageType = typeof(EquipmentSpellSelection),
                    Title = "Equipment Wizard",
                    WizardSteps = new string[]{
                            "Select Attached Spells",
                            "Set Parameters",
                            "Set Attack Attributes",
                            "Set Rogue Encyclopedia Data",
                            "Edit Symbol"
                    }
                });
            });
            _regionManager.RegisterViewWithRegion("SpellWizardRegion", () =>
            {
                return new Wizard(new WizardViewModel()
                {
                    FirstPageType = typeof(SpellType),
                    Title = "Spell Wizard",
                    WizardSteps = new string[]{
                            "Select Base Type",
                            "Select Sub Type",
                            "Set Parameters"
                    }
                });
            });

            _regionManager.RegisterViewWithRegion("DesignRegion", typeof(Layout));
            _regionManager.RegisterViewWithRegion("DesignRegion", typeof(Doodad));
            _regionManager.RegisterViewWithRegion("DesignRegion", typeof(SkillSet));
            _regionManager.RegisterViewWithRegion("DesignRegion", typeof(Rogue.NET.ScenarioEditor.Views.Assets.Brush));

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
        }

        private string AddAsset(string assetType)
        {
            switch (assetType)
            {
                case "Layout":
                    {
                        var name = NameGenerator.Get(_controller.CurrentConfig.DungeonTemplate.LayoutTemplates.Select(z => z.Name), "New Layout");
                        _controller.CurrentConfig.DungeonTemplate.LayoutTemplates.Add(new LayoutTemplateViewModel() { Name = name });
                        return name;
                    }
                case "AttackAttribute":
                    {
                        var name = NameGenerator.Get(_controller.CurrentConfig.AttackAttributes.Select(z => z.Name), "New Attack Attribute");
                        _controller.CurrentConfig.AttackAttributes.Add(new AttackAttributeTemplateViewModel() { Name = name });
                        return name;
                    }
                case "Enemy":
                    {
                        var name = NameGenerator.Get(_controller.CurrentConfig.EnemyTemplates.Select(z => z.Name), "New Enemy");
                        _controller.CurrentConfig.EnemyTemplates.Add(new EnemyTemplateViewModel() { Name = name });
                        return name;
                    }
                case "Equipment":
                    {
                        var name = NameGenerator.Get(_controller.CurrentConfig.EquipmentTemplates.Select(z => z.Name), "New Equipment");
                        _controller.CurrentConfig.EquipmentTemplates.Add(new EquipmentTemplateViewModel() { Name = name });
                        return name;
                    }
                case "Consumable":
                    {
                        var name = NameGenerator.Get(_controller.CurrentConfig.ConsumableTemplates.Select(z => z.Name), "New Consumable");
                        _controller.CurrentConfig.ConsumableTemplates.Add(new ConsumableTemplateViewModel() { Name = name });
                        return name;
                    }
                case "Doodad":
                    {
                        var name = NameGenerator.Get(_controller.CurrentConfig.DoodadTemplates.Select(z => z.Name), "New Doodad");
                        _controller.CurrentConfig.DoodadTemplates.Add(new DoodadTemplateViewModel() { Name = name });
                        return name;
                    }
                case "Spell":
                    {
                        var name = NameGenerator.Get(_controller.CurrentConfig.MagicSpells.Select(z => z.Name), "New Spell");
                        _controller.CurrentConfig.MagicSpells.Add(new SpellTemplateViewModel() { Name = name });
                        return name;
                    }
                case "SkillSet":
                    {
                        var name = NameGenerator.Get(_controller.CurrentConfig.SkillTemplates.Select(z => z.Name), "New Skill Set");
                        _controller.CurrentConfig.SkillTemplates.Add(new SkillSetTemplateViewModel() { Name = name });
                        return name;
                    }
                case "Animation":
                    {
                        var name = NameGenerator.Get(_controller.CurrentConfig.AnimationTemplates.Select(z => z.Name), "New Animation");
                        _controller.CurrentConfig.AnimationTemplates.Add(new AnimationTemplateViewModel() { Name = name });
                        return name;
                    }
                case "Brush":
                    {
                        var name = NameGenerator.Get(_controller.CurrentConfig.BrushTemplates.Select(z => z.Name), "New Brush");
                        _controller.CurrentConfig.BrushTemplates.Add(new BrushTemplateViewModel() { Name = name });
                        return name;
                    }
                case "Pen":
                    {
                        var name = NameGenerator.Get(_controller.CurrentConfig.PenTemplates.Select(z => z.Name), "New Pen");
                        _controller.CurrentConfig.PenTemplates.Add(new PenTemplateViewModel() { Name = name });
                        return name;
                    }
                default:
                    break;
            }

            throw new Exception("Unidentified new asset type");
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

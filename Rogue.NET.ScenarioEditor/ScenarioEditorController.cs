using MonitoredUndo;
using Prism.Events;
using Prism.Regions;
using Rogue.NET.Common.Events.Scenario;
using Rogue.NET.Core.Event.Splash;
using Rogue.NET.Core.Logic.Processing;
using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Service.Interface;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.Interface;
using Rogue.NET.ScenarioEditor.Service.Interface;
using Rogue.NET.ScenarioEditor.ViewModel;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Layout;
using Rogue.NET.ScenarioEditor.Views.Assets;
using Rogue.NET.ScenarioEditor.Views.Construction;
using Rogue.NET.ScenarioEditor.Views.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IScenarioEditorController))]
    public class ScenarioEditorController : IScenarioEditorController
    {
        readonly IEventAggregator _eventAggregator;
        readonly IRegionManager _regionManager;
        readonly IRogueUndoService _rogueUndoService;

        readonly IScenarioResourceService _resourceService;

        ScenarioConfigurationContainerViewModel _config;

        [ImportingConstructor]
        public ScenarioEditorController(
            IRegionManager regionManager,
            IEventAggregator eventAggregator,
            IRogueUndoService rogueUndoService,
            IScenarioResourceService scenarioResourceService)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            _rogueUndoService = rogueUndoService;
            _resourceService = scenarioResourceService;

            Initialize();
        }

        private void Initialize()
        {
            _eventAggregator.GetEvent<LoadBuiltInScenarioEvent>().Subscribe((e) =>
            {
                Open(e.ScenarioName, true);
            });

            _eventAggregator.GetEvent<Rogue.NET.ScenarioEditor.Events.SaveScenarioEvent>().Subscribe((e) =>
            {
                Save();
            });

            _eventAggregator.GetEvent<NewScenarioConfigEvent>().Subscribe(() =>
            {
                New();
            });

            _eventAggregator.GetEvent<LoadConstructionEvent>().Subscribe((e) =>
            {
                LoadConstruction(e.ConstructionName);
            });

            _eventAggregator.GetEvent<ScoreScenarioEvent>().Subscribe(() =>
            {
                ShowDifficulty();
            });
        }

        public string AddAsset(string assetType)
        {
            switch (assetType)
            {
                case "Layout":
                    {
                        var name = GenerateName(_config.DungeonTemplate.LayoutTemplates.Select(z => z.Name), "New Layout");
                        _config.DungeonTemplate.LayoutTemplates.Add(new LayoutTemplateViewModel() { Name = name });
                        return name;
                    }
                case "AttackAttribute":
                    {
                        var name = GenerateName(_config.AttackAttributes.Select(z => z.Name), "New Attack Attribute");
                        _config.AttackAttributes.Add(new AttackAttributeTemplateViewModel() { Name = name });
                        return name;
                    }
                case "Enemy":
                    {
                        var name = GenerateName(_config.EnemyTemplates.Select(z => z.Name), "New Enemy");
                        _config.EnemyTemplates.Add(new EnemyTemplateViewModel() { Name = name });
                        return name;
                    }
                case "Equipment":
                    {
                        var name = GenerateName(_config.EquipmentTemplates.Select(z => z.Name), "New Equipment");
                        _config.EquipmentTemplates.Add(new EquipmentTemplateViewModel() { Name = name });
                        return name;
                    }
                case "Consumable":
                    {
                        var name = GenerateName(_config.ConsumableTemplates.Select(z => z.Name), "New Consumable");
                        _config.ConsumableTemplates.Add(new ConsumableTemplateViewModel() { Name = name });
                        return name;
                    }
                case "Doodad":
                    {
                        var name = GenerateName(_config.DoodadTemplates.Select(z => z.Name), "New Doodad");
                        _config.DoodadTemplates.Add(new DoodadTemplateViewModel() { Name = name });
                        return name;
                    }
                case "Spell":
                    {
                        var name = GenerateName(_config.MagicSpells.Select(z => z.Name), "New Spell");
                        _config.MagicSpells.Add(new SpellTemplateViewModel() { Name = name });
                        return name;
                    }
                case "SkillSet":
                    {
                        var name = GenerateName(_config.SkillTemplates.Select(z => z.Name), "New Skill Set");
                        _config.SkillTemplates.Add(new SkillSetTemplateViewModel() { Name = name });
                        return name;
                    }
                case "Animation":
                    {
                        var name = GenerateName(_config.AnimationTemplates.Select(z => z.Name), "New Animation");
                        _config.AnimationTemplates.Add(new AnimationTemplateViewModel() { Name = name });
                        return name;
                    }
                case "Brush":
                    {
                        var name = GenerateName(_config.BrushTemplates.Select(z => z.Name), "New Brush");
                        _config.BrushTemplates.Add(new BrushTemplateViewModel() { Name = name });
                        return name;
                    }
                case "Pen":
                    {
                        var name = GenerateName(_config.PenTemplates.Select(z => z.Name), "New Pen");
                        _config.PenTemplates.Add(new PenTemplateViewModel() { Name = name });
                        return name;
                    }
                default:
                    break;
            }

            throw new Exception("Unidentified new asset type");
        }
        public void RemoveAsset(string type, string name)
        {
            //TODO: remove asset
        }
        public void LoadAsset(string type, string name)
        {
            switch (type)
            {
                case "Layout":
                    {
                        _regionManager.RequestNavigate("DesignRegion", type);
                        var ctrl = _regionManager.Regions["DesignRegion"].Views.First((v => v.GetType() == typeof(Layout))) as UserControl;
                        var model = _config.DungeonTemplate.LayoutTemplates.First(x => x.Name == name);

                        ctrl.DataContext = model;
                    }
                    break;
                case "SkillSet":
                    {
                        _regionManager.RequestNavigate("DesignRegion", type);
                        var ctrl = _regionManager.Regions["DesignRegion"].Views.First((v => v.GetType() == typeof(SkillSet))) as SkillSet;
                        var model = _config.SkillTemplates.First(x => x.Name == name);

                        ctrl.DataContext = model;
                        ctrl.SetConfigurationData(_config);
                    }
                    break;
                case "Brush":
                    {
                        _regionManager.RequestNavigate("DesignRegion", type);
                        var ctrl = _regionManager.Regions["DesignRegion"].Views.First((v => v.GetType() == typeof(Brush))) as UserControl;
                        var model = _config.BrushTemplates.First(x => x.Name == name);

                        ctrl.DataContext = model;
                    }
                    break;
                case "Enemy":
                    {
                        _regionManager.RequestNavigate("DesignRegion", "EnemyWizard");
                        var wizard = _regionManager.Regions["EnemyWizardRegion"].Views.First() as Wizard;
                        var viewModel = wizard.DataContext as IWizardViewModel;
                        var model = _config.EnemyTemplates.First(x => x.Name == name);

                        // manually set the wizard's payload. no simple way to inject the model using the constructor
                        viewModel.Payload = model;
                        viewModel.PayloadTitle = model.Name;
                        viewModel.SecondaryPayload = _config;

                        //wizard.Reset();
                    }
                    break;
                case "Equipment":
                    {
                        _regionManager.RequestNavigate("DesignRegion", "EquipmentWizard");
                        var wizard = _regionManager.Regions["EquipmentWizardRegion"].Views.First() as Wizard;
                        var viewModel = wizard.DataContext as IWizardViewModel;
                        var model = _config.EquipmentTemplates.First(x => x.Name == name);

                        // manually set the wizard's payload. no simple way to inject the model using the constructor
                        viewModel.Payload = model;
                        viewModel.PayloadTitle = model.Name;
                        viewModel.SecondaryPayload = _config;

                        //wizard.Reset();
                    }
                    break;
                case "Consumable":
                    {
                        _regionManager.RequestNavigate("DesignRegion", "ConsumableWizard");
                        var wizard = _regionManager.Regions["ConsumableWizardRegion"].Views.First() as Wizard;
                        var viewModel = wizard.DataContext as IWizardViewModel;
                        var model = _config.ConsumableTemplates.First(x => x.Name == name);

                        // manually set the wizard's payload. no simple way to inject the model using the constructor
                        viewModel.Payload = model;
                        viewModel.PayloadTitle = model.Name;
                        viewModel.SecondaryPayload = _config;

                        //wizard.Reset();
                    }
                    break;
                case "Doodad":
                    {
                        _regionManager.RequestNavigate("DesignRegion", type);
                        var ctrl = _regionManager.Regions["DesignRegion"].Views.First((v => v.GetType() == typeof(Doodad))) as Doodad;
                        var model = _config.DoodadTemplates.First(x => x.Name == name);

                        ctrl.SetConfigurationData(_config);
                        ctrl.DataContext = model;
                    }
                    break;
                case "Spell":
                    {
                        _regionManager.RequestNavigate("DesignRegion", "SpellWizard");
                        var wizard = _regionManager.Regions["SpellWizardRegion"].Views.First() as Wizard;
                        var viewModel = wizard.DataContext as IWizardViewModel;
                        var model = _config.MagicSpells.First(x => x.Name == name);

                        // manually set the wizard's payload. no simple way to inject the model using the constructor
                        viewModel.Payload = model;
                        viewModel.PayloadTitle = model.Name;
                        viewModel.SecondaryPayload = _config;

                        ///wizard.Reset();
                    }
                    break;
                case "Animation":
                    {
                        _regionManager.RequestNavigate("DesignRegion", "AnimationWizard");
                        var wizard = _regionManager.Regions["AnimationWizardRegion"].Views.First() as Wizard;
                        var viewModel = wizard.DataContext as IWizardViewModel;
                        var model = _config.AnimationTemplates.First(x => x.Name == name);

                        // manually set the wizard's payload. no simple way to inject the model using the constructor
                        viewModel.Payload = model;
                        viewModel.PayloadTitle = model.Name;
                        viewModel.SecondaryPayload = _config;

                        //wizard.Reset();
                    }
                    break;
                default:
                    throw new ApplicationException("Unhandled Design Region Type");
            }
        }
        public bool UpdateAssetName(string oldName, string newName, string type)
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
                    return _config.DungeonTemplate.LayoutTemplates.FirstOrDefault(x => x.Name == name);
                case "SkillSet":
                    return _config.SkillTemplates.FirstOrDefault(x => x.Name == name);
                case "Brush":
                    return _config.BrushTemplates.FirstOrDefault(x => x.Name == name);
                case "Enemy":
                    return _config.EnemyTemplates.FirstOrDefault(x => x.Name == name);
                case "Equipment":
                    return _config.EquipmentTemplates.FirstOrDefault(x => x.Name == name);
                case "Consumable":
                    return _config.ConsumableTemplates.FirstOrDefault(x => x.Name == name);
                case "Doodad":
                    return _config.DoodadTemplates.FirstOrDefault(x => x.Name == name);
                case "Spell":
                    return _config.MagicSpells.FirstOrDefault(x => x.Name == name);
                case "Animation":
                    return _config.AnimationTemplates.FirstOrDefault(x => x.Name == name);
                default:
                    throw new ApplicationException("Unhandled Asset Type");
            }
        }

        public void LoadConstruction(string constructionName)
        {
            switch (constructionName)
            {
                case "General":
                    {
                        _regionManager.RequestNavigate("DesignRegion", "General");
                        var ctrl = _regionManager.Regions["DesignRegion"].Views.First((v => v.GetType() == typeof(General))) as General;

                        ctrl.DataContext = _config.DungeonTemplate;
                        ctrl.SetConfigurationParameters(_config);
                    }
                    break;
                case "ItemPlacement":
                    {
                        _regionManager.RequestNavigate("DesignRegion", "ItemPlacement");
                        var ctrl = _regionManager.Regions["DesignRegion"].Views.First(v => v.GetType() == typeof(ItemPlacement)) as UserControl;

                        ctrl.DataContext = new PlacementGroupViewModel(_config.ConsumableTemplates.
                            Cast<DungeonObjectTemplateViewModel>().
                            Union(_config.EquipmentTemplates.
                                Cast<DungeonObjectTemplateViewModel>()).
                                Select(z => new PlacementViewModel()
                                {
                                    // TODO
                                    //ImageSource = TemplateGenerator.GenerateSymbol(z.SymbolDetails).SymbolImageSource,
                                    Template = z
                                }));
                    }
                    break;
                case "EnemyPlacement":
                    {
                        _regionManager.RequestNavigate("DesignRegion", "EnemyPlacement");
                        var ctrl = _regionManager.Regions["DesignRegion"].Views.First(v => v.GetType() == typeof(EnemyPlacement)) as UserControl;

                        ctrl.DataContext = new PlacementGroupViewModel(_config.EnemyTemplates.
                            Select(z => new PlacementViewModel()
                            {
                                // TODO
                                // ImageSource = TemplateGenerator.GenerateSymbol(z.SymbolDetails).SymbolImageSource,
                                Template = z
                            }));
                    }
                    break;
                case "DoodadPlacement":
                    {
                        _regionManager.RequestNavigate("DesignRegion", "DungeonObjectPlacement");
                        var ctrl = _regionManager.Regions["DesignRegion"].Views.First(v => v.GetType() == typeof(DungeonObjectPlacement)) as UserControl;

                        ctrl.DataContext = new PlacementGroupViewModel(_config.DoodadTemplates.
                            Select(z => new PlacementViewModel()
                            {
                                // TODO
                                // ImageSource = TemplateGenerator.GenerateSymbol(z.SymbolDetails).SymbolImageSource,
                                Template = z
                            }));
                    }
                    break;
                case "LayoutDesign":
                    {
                        _regionManager.RequestNavigate("DesignRegion", "LayoutDesign");
                        var ctrl = _regionManager.Regions["DesignRegion"].Views.First(v => v.GetType() == typeof(LayoutDesign)) as UserControl;

                        ctrl.DataContext = new PlacementGroupViewModel(_config.DungeonTemplate.LayoutTemplates.
                            Select(z => new PlacementViewModel()
                            {
                                ImageSource = null,
                                Template = z
                            }));
                    }
                    break;
                case "PlayerDesign":
                    {
                        _regionManager.RequestNavigate("DesignRegion", "PlayerDesign");
                        var ctrl = _regionManager.Regions["DesignRegion"].Views.First(v => v.GetType() == typeof(PlayerDesign)) as PlayerDesign;

                        ctrl.DataContext = _config.PlayerTemplate;
                        ctrl.SetConfigurationParameters(_config);
                    }
                    break;
                case "ObjectiveDesign":
                    {
                        _regionManager.RequestNavigate("DesignRegion", "ObjectiveDesign");
                        var ctrl = _regionManager.Regions["DesignRegion"].Views.First(v => v.GetType() == typeof(ObjectiveDesign)) as ObjectiveDesign;

                        ctrl.DataContext = _config.DungeonTemplate;
                        ctrl.SetConfigurationParameters(_config);
                    }
                    break;
            }
        }

        public void New()
        {
            // Have to keep Undo Service in sync with the configuration
            if (_config != null)
                _rogueUndoService.Clear();

            // Create new Scenario Configuration
            _config = new ScenarioConfigurationContainerViewModel();

            // Register with the Undo Service
            _rogueUndoService.Register(_config);

            // Publish the Scenario Configuration
            _eventAggregator.GetEvent<ScenarioLoadedEvent>().Publish(_config);
        }

        public void Open(string name, bool builtIn)
        {
            // Have to keep Undo Service in sync with the configuration
            if (_config != null)
                _rogueUndoService.Clear();

            // Open the Scenario Configuration from file
            ScenarioConfigurationContainer config;
            if (builtIn)
                config = _resourceService.GetEmbeddedScenarioConfiguration((ConfigResources)Enum.Parse(typeof(ConfigResources), name));
            else
                config = _resourceService.OpenScenarioConfigurationFile(name);

            // Map to the view model
            _config = ExpressMapper.Mapper.Map<ScenarioConfigurationContainer, ScenarioConfigurationContainerViewModel>(config);

            // Register with the Undo Service
            _rogueUndoService.Register(_config);

            // Publish configuration
            _eventAggregator.GetEvent<ScenarioLoadedEvent>().Publish(_config);
        }

        public void Save()
        {
            _eventAggregator.GetEvent<SplashEvent>().Publish(new SplashUpdate()
            {
                SplashAction = SplashAction.Show,
                SplashType = SplashEventType.Save
            });

            PublishOutputMessage("Saving " + _config.DungeonTemplate.Name + " Scenario File...");

            // resolve references: for example, if a consumable spell was altered - update the player's items.
            // This should be done by the serializer; but off hand I don't know how to specify this behavior.
            ResolveConfigurationReferences();

            // Map back to the model namespace
            var config = ExpressMapper.Mapper.Map<ScenarioConfigurationContainerViewModel, ScenarioConfigurationContainer>(_config);

            // Save the configuration
            _resourceService.SaveConfig(_config.DungeonTemplate.Name, config);

            // Clear the Undo stack
            _rogueUndoService.Clear();

            PublishOutputMessage("Save complete");

            _eventAggregator.GetEvent<SplashEvent>().Publish(new SplashUpdate()
            {
                SplashAction = SplashAction.Hide,
                SplashType = SplashEventType.Save
            });

            _eventAggregator.GetEvent<ScenarioLoadedEvent>().Publish(_config);
        }

        public void Validate()
        {
            //TODO
        }

        public void ShowDifficulty()
        {
            _regionManager.RequestNavigate("DesignRegion", "ScenarioDifficultyChart");
            //var ctrl = _regionManager.Regions["DesignRegion"].Views.First((v => v.GetType() == typeof(ScenarioDifficultyChart))) as ScenarioDifficultyChart;
        }

        private string GenerateName(IEnumerable<string> names, string prefix)
        {
            var ctr = 1;
            var name = prefix;
            while (names.Contains(prefix))
                prefix = name + " (" + ctr++.ToString() + ")";

            return prefix;
        }

        private void PublishOutputMessage(string msg)
        {
            _eventAggregator.GetEvent<ScenarioEditorMessageEvent>().Publish(new ScenarioEditorMessageEventArgs()
            {
                Message = msg
            });
        }

        private void ResolveConfigurationReferences()
        {
            /* References to spells should be held through serialization. I don't know how to do this using the 
             * .NET serializer; but it's not that much code to ensure they're consistent... 
             */

            // animation brushes
            foreach (var animation in _config.AnimationTemplates)
            {
                if (animation.FillTemplate != null && animation.FillTemplate.Name != "New Template")
                    animation.FillTemplate = _config.BrushTemplates.First(b => b.Name == animation.FillTemplate.Name);

                if (animation.StrokeTemplate != null && animation.StrokeTemplate.Name != "New Template")
                    animation.StrokeTemplate = _config.PenTemplates.First(b => b.Name == animation.StrokeTemplate.Name);
            }

            // spells
            foreach (var spell in _config.MagicSpells)
            {
                for (int i=0;i<spell.Animations.Count;i++)
                {
                    var animation = spell.Animations[i];
                    if (animation != null)
                        animation = _config.AnimationTemplates.First(a => a.Name == animation.Name);

                    SyncAttackAttributes(_config.AttackAttributes, spell.Effect.AttackAttributes.Cast<DungeonObjectTemplateViewModel>().ToList());
                    SyncAttackAttributes(_config.AttackAttributes, spell.AuraEffect.AttackAttributes.Cast<DungeonObjectTemplateViewModel>().ToList());
                }
            }

            // skill set spells
            foreach (var skillSet in _config.SkillTemplates)
            {
                for (int i = 0; i < skillSet.Spells.Count; i++)
                {
                    var spell = skillSet.Spells[i];
                    if (spell != null)
                        spell = _config.MagicSpells.First(s => s.Name == spell.Name);
                }
            }

            // consumable spells and skill sets
            foreach (var consumable in _config.ConsumableTemplates)
            {
                if (consumable.AmmoSpellTemplate != null && consumable.SubType == ConsumableSubType.Ammo)
                    consumable.AmmoSpellTemplate = _config.MagicSpells.First(s => s.Name == consumable.AmmoSpellTemplate.Name);

                if (consumable.LearnedSkill!= null && consumable.HasLearnedSkill)
                    consumable.LearnedSkill = _config.SkillTemplates.First(s => s.Name == consumable.LearnedSkill.Name);

                if (consumable.ProjectileSpellTemplate != null && consumable.IsProjectile)
                    consumable.ProjectileSpellTemplate = _config.MagicSpells.First(s => s.Name == consumable.ProjectileSpellTemplate.Name);

                if (consumable.SpellTemplate != null && consumable.HasSpell)
                    consumable.SpellTemplate = _config.MagicSpells.First(s => s.Name == consumable.SpellTemplate.Name);
            }

            // equipment spells
            foreach (var equipment in _config.EquipmentTemplates)
            {
                if (equipment.AmmoTemplate != null && equipment.Type == EquipmentType.RangeWeapon)
                    equipment.AmmoTemplate = _config.ConsumableTemplates.First(s => s.Name == equipment.AmmoTemplate.Name);

                if (equipment.AttackSpell != null && equipment.HasAttackSpell)
                    equipment.AttackSpell = _config.MagicSpells.First(s => s.Name == equipment.AttackSpell.Name);

                if (equipment.EquipSpell != null && equipment.HasEquipSpell)
                    equipment.EquipSpell = _config.MagicSpells.First(s => s.Name == equipment.EquipSpell.Name);

                if (equipment.CurseSpell != null && equipment.HasCurseSpell)
                    equipment.CurseSpell = _config.MagicSpells.First(s => s.Name == equipment.CurseSpell.Name);

                SyncAttackAttributes(_config.AttackAttributes, equipment.AttackAttributes.Cast<DungeonObjectTemplateViewModel>().ToList());
            }

            // doodad spells
            foreach (var doodad in _config.DoodadTemplates)
            {
                if (doodad.AutomaticMagicSpellTemplate != null && doodad.IsAutomatic)
                    doodad.AutomaticMagicSpellTemplate = _config.MagicSpells.First(s => s.Name == doodad.AutomaticMagicSpellTemplate.Name);

                if (doodad.InvokedMagicSpellTemplate != null && doodad.IsInvoked)
                    doodad.InvokedMagicSpellTemplate = _config.MagicSpells.First(s => s.Name == doodad.InvokedMagicSpellTemplate.Name);
            }

            // player consumables
            for (int i = 0; i < _config.PlayerTemplate.StartingConsumables.Count; i++)
            {
                var consumable = _config.PlayerTemplate.StartingConsumables[i];
                if (consumable != null)
                    consumable.TheTemplate = _config.ConsumableTemplates.First(c => c.Name == consumable.TheTemplate.Name);
            }

            // player equipment
            for (int i = 0; i < _config.PlayerTemplate.StartingEquipment.Count; i++)
            {
                var equipment = _config.PlayerTemplate.StartingEquipment[i];
                if (equipment != null)
                    equipment.TheTemplate = _config.EquipmentTemplates.First(c => c.Name == equipment.TheTemplate.Name);
            }

            // player skills
            for (int i = 0; i < _config.PlayerTemplate.Skills.Count; i++)
            {
                var skill = _config.PlayerTemplate.Skills[i];
                if (skill != null)
                    _config.PlayerTemplate.Skills[i] = _config.SkillTemplates.First(s => s.Name == skill.Name);
            }

            // Enemies
            foreach (var enemy in _config.EnemyTemplates)
            {
                if (enemy.BehaviorDetails.PrimaryBehavior.EnemySpell != null && 
                    enemy.BehaviorDetails.PrimaryBehavior.AttackType == CharacterAttackType.Skill)
                    enemy.BehaviorDetails.PrimaryBehavior.EnemySpell = _config.MagicSpells.First(s => s.Name == enemy.BehaviorDetails.PrimaryBehavior.EnemySpell.Name);

                if (enemy.BehaviorDetails.SecondaryBehavior.EnemySpell != null &&
                    enemy.BehaviorDetails.SecondaryBehavior.AttackType == CharacterAttackType.Skill)
                    enemy.BehaviorDetails.SecondaryBehavior.EnemySpell = _config.MagicSpells.First(s => s.Name == enemy.BehaviorDetails.SecondaryBehavior.EnemySpell.Name);

                // enemy consumables
                for (int i = 0; i < enemy.StartingConsumables.Count; i++)
                {
                    var consumable = enemy.StartingConsumables[i];
                    if (consumable != null)
                        consumable.TheTemplate = _config.ConsumableTemplates.First(c => c.Name == consumable.TheTemplate.Name);
                }

                // enemy equipment
                for (int i = 0; i < enemy.StartingEquipment.Count; i++)
                {
                    var equipment = enemy.StartingEquipment[i];
                    if (equipment != null)
                        equipment.TheTemplate = _config.EquipmentTemplates.First(c => c.Name == equipment.TheTemplate.Name);
                }

                // enemy attack attributes
                SyncAttackAttributes(_config.AttackAttributes, enemy.AttackAttributes.Cast<DungeonObjectTemplateViewModel>().ToList());
            }
        }

        private void SyncAttackAttributes(IList<DungeonObjectTemplateViewModel> source, IList<DungeonObjectTemplateViewModel> dest)
        {
            // Create
            foreach (var attrib in source)
            {
                if (!dest.Any(a => a.Name == attrib.Name))
                    dest.Add(attrib);

                // Update
                else
                {
                    var existing = dest.First(a => a.Name == attrib.Name);
                    existing.SymbolDetails.Icon = attrib.SymbolDetails.Icon;
                }
            }

            // Delete
            for (int i = dest.Count - 1; i >= 0; i--)
            {
                if (!source.Any(a => a.Name == dest[i].Name))
                    dest.RemoveAt(i);
            }
        }
    }
}

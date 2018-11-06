using Prism.Events;
using Prism.Regions;
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
        readonly IRogueUndoService _rogueUndoService;
        readonly IScenarioResourceService _resourceService;

        ScenarioConfigurationContainerViewModel _config;

        [ImportingConstructor]
        public ScenarioEditorController(
            IEventAggregator eventAggregator,
            IRogueUndoService rogueUndoService,
            IScenarioResourceService scenarioResourceService)
        {
            _eventAggregator = eventAggregator;
            _rogueUndoService = rogueUndoService;
            _resourceService = scenarioResourceService;

            Initialize();
        }

        public ScenarioConfigurationContainerViewModel CurrentConfig { get { return _config; } }

        private void Initialize()
        {
            _eventAggregator.GetEvent<LoadBuiltInScenarioEvent>().Subscribe((scenarioName) =>
            {
                Open(scenarioName, true);
            });

            _eventAggregator.GetEvent<Rogue.NET.ScenarioEditor.Events.SaveScenarioEvent>().Subscribe(() =>
            {
                Save();
            });

            _eventAggregator.GetEvent<SaveBuiltInScenarioEvent>().Subscribe((configResource) =>
            {
                Save(true, configResource);
            });

            _eventAggregator.GetEvent<NewScenarioConfigEvent>().Subscribe(() =>
            {
                New();
            });
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
            Save(false);
        }
        public void Save(bool builtInScenario = false, ConfigResources builtInScenarioType = ConfigResources.Fighter)
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
            if (builtInScenario)
                _resourceService.EmbedConfig(builtInScenarioType, config);
            else
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

        private void PublishOutputMessage(string msg)
        {
            _eventAggregator.GetEvent<ScenarioEditorMessageEvent>().Publish(new ScenarioEditorMessageEventArgs()
            {
                Message = msg
            });
        }

        #region TODO REMOVE THESE!!!
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
        #endregion
    }
}

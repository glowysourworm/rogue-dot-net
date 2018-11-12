using Rogue.NET.Core.Logic.Content.Interface;
using Rogue.NET.Core.Logic.Interface;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Generator.Interface;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Content.Doodad;
using Rogue.NET.Core.Service.Interface;

using System.Linq;
using System.ComponentModel.Composition;
using System;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Core.Logic.Processing.Interface;
using Rogue.NET.Core.Logic.Processing;
using Rogue.NET.Core.Logic.Processing.Enum;

namespace Rogue.NET.Core.Logic
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IScenarioEngine))]
    public class ScenarioEngine : IScenarioEngine
    {
        readonly ILayoutEngine _layoutEngine;
        readonly IContentEngine _contentEngine;
        readonly ISpellEngine _spellEngine;
        readonly IModelService _modelService;
        readonly IScenarioMessageService _scenarioMessageService;
        readonly IInteractionProcessor _interactionProcessor;
        readonly ICharacterProcessor _characterProcessor;
        readonly IPlayerProcessor _playerProcessor;
        readonly IAlterationProcessor _alterationProcessor;
        readonly IRandomSequenceGenerator _randomSequenceGenerator;

        public event EventHandler<IScenarioUpdate> ScenarioUpdateEvent;
        public event EventHandler<ISplashUpdate> SplashUpdateEvent;
        public event EventHandler<ILevelUpdate> LevelUpdateEvent;
        public event EventHandler<IAnimationUpdate> AnimationUpdateEvent;
        public event EventHandler<ILevelProcessingAction> LevelProcessingActionEvent;

        [ImportingConstructor]
        public ScenarioEngine(
            ILayoutEngine layoutEngine,
            IContentEngine contentEngine,
            ISpellEngine spellEngine,
            IModelService modelService,
            IScenarioMessageService scenarioMessageService,
            IInteractionProcessor interactionProcessor,
            ICharacterProcessor characterProcessor,
            IPlayerProcessor playerProcessor,
            IAlterationProcessor alterationProcessor,
            IRandomSequenceGenerator randomSequenceGenerator)
        {
            _layoutEngine = layoutEngine;
            _contentEngine = contentEngine;
            _spellEngine = spellEngine;
            _modelService = modelService;
            _scenarioMessageService = scenarioMessageService;
            _interactionProcessor = interactionProcessor;
            _characterProcessor = characterProcessor;
            _playerProcessor = playerProcessor;
            _alterationProcessor = alterationProcessor;
            _randomSequenceGenerator = randomSequenceGenerator;
        }

        // Pre-check player state before applying desired command
        public LevelContinuationAction ProcessAlteredPlayerState()
        {
            var player = _modelService.Player;
            var states = player.Alteration.GetStates();

            if (states.Any(z => z != CharacterStateType.Normal))
            {
                //Sleeping
                if (states.Any(z => z == CharacterStateType.Sleeping))
                {
                    _scenarioMessageService.Publish(player.RogueName + " is asleep!");
                    return LevelContinuationAction.ProcessTurn;
                }

                //Paralyzed
                else if (states.Any(z => z == CharacterStateType.Paralyzed))
                {
                    _scenarioMessageService.Publish(player.RogueName + " is paralyzed!");
                    return LevelContinuationAction.ProcessTurn;
                }

                //Confused
                else if (states.Any(z => z == CharacterStateType.Confused))
                {
                    _scenarioMessageService.Publish(player.RogueName + " is confused!");
                    var obj = MoveRandom();

                    if (obj is Consumable || obj is Equipment)
                        _contentEngine.StepOnItem(player, (ItemBase)obj);
                    else if (obj is DoodadBase)
                        _contentEngine.StepOnDoodad(player, (DoodadBase)obj);

                    return LevelContinuationAction.ProcessTurn;
                }
            }

            return LevelContinuationAction.DoNothing;
        }

        public ScenarioObject Move(Compass direction)
        {
            //Desired Location
            var desiredLocation = _layoutEngine.GetPointInDirection(_modelService.CurrentLevel.Grid, _modelService.Player.Location, direction);

            //Look for road blocks - move player
            if (!_layoutEngine.IsPathToAdjacentCellBlocked(_modelService.CurrentLevel, _modelService.Player.Location, desiredLocation))
            {
                // Update player location
                _modelService.Player.Location = desiredLocation;

                // Notify Listener queue
                LevelUpdateEvent(this, new LevelUpdate() { LevelUpdateType = LevelUpdateType.Player });
            }

            //Increment counter
            _modelService.CurrentLevel.StepsTaken++;

            //See what the player stepped on...
            return _modelService.CurrentLevel.GetAtPoint<ScenarioObject>(_modelService.Player.Location);
        }
        public ScenarioObject MoveRandom()
        {
            // Get random adjacent location
            var desiredLocation = _layoutEngine.GetRandomAdjacentLocation(_modelService.CurrentLevel,_modelService.Player, _modelService.Player.Location, true);

            // Get direction for random move -> Move()
            return Move(_layoutEngine.GetDirectionBetweenAdjacentPoints(_modelService.Player.Location, desiredLocation));
        }

        /// <summary>
        /// Runs Ray-Tracing for end-of-turn; Applies Player end-of-turn, advancement, and death; and
        /// applies Level end-of-turn (create new monsters). Updates IModelService { visible locations, visible contents, end targeting }
        /// </summary>
        /// <param name="regenerate">Set to false to prevent Player regeneration</param>
        public void ProcessEndOfTurn(bool regenerate)
        {
            var level = _modelService.CurrentLevel;
            var player = _modelService.Player;

            // Block Scenario Message Processing to prevent sending UI messages
            _scenarioMessageService.Block();

            // Want to now update model service from the model. This will process new visibility of
            // the Level Grid.
            _modelService.UpdateVisibleLocations();

            // Player: End-Of-Turn
            _playerProcessor.ApplyEndOfTurn(player, regenerate);

            //I'm Not DEEEAD! (TODO: Publish event for player death)
            if (player.Hunger >= 100 || player.Hp <= 0.1)
                ScenarioUpdateEvent(this, new ScenarioUpdate()
                {
                    ScenarioUpdateType = ScenarioUpdateType.PlayerDeath,
                    PlayerDeathMessage = "Had a rough day"
                });

            // Apply End-Of-Turn for the Level content
            _contentEngine.ApplyEndOfTurn();

            // Update Model Content: 0) End Targeting
            //                       1) Update visible contents
            //                       2) Calculate model delta to prepare for UI
            _modelService.ClearTargetedEnemies();
            _modelService.UpdateContents();

            // Allow passing of messages back to the UI
            _scenarioMessageService.UnBlock(false);

            // Check Scenario Objective - TODO - Create different message event
            if (CheckObjective())
                _scenarioMessageService.Publish("YOU WON!");
        }

        private bool CheckObjective()
        {
            return _modelService.ScenarioEncyclopedia
                                .Values
                                .Where(z => z.IsObjective)
                                .All((metaData) =>
            {
                switch (metaData.ObjectType)
                {
                    case DungeonMetaDataObjectTypes.Skill:
                        return _modelService.Player.SkillSets.Any(skill => skill.RogueName == metaData.RogueName);
                    case DungeonMetaDataObjectTypes.Item:
                        return (_modelService.Player.Consumables.Values.Any(item => item.RogueName == metaData.RogueName)
                             || _modelService.Player.Equipment.Values.Any(item => item.RogueName == metaData.RogueName));
                    case DungeonMetaDataObjectTypes.Enemy:
                        return metaData.IsIdentified;
                    case DungeonMetaDataObjectTypes.Doodad:
                        return metaData.IsIdentified;
                    default:
                        throw new Exception("Unknown Scenario Meta Data Object Type");
                }
            });
        }

        public void Attack(Compass direction)
        {
            var player = _modelService.Player;

            // Get points involved with the attack
            var location = _modelService.Player.Location;
            var attackLocation = _layoutEngine.GetPointInDirection(_modelService.CurrentLevel.Grid, location, direction);

            // Check to see whether path is clear to attack
            var blocked = _layoutEngine.IsCellThroughWall(_modelService.CurrentLevel.Grid, location, attackLocation);

            // Get target for attack
            var enemy = _modelService.CurrentLevel.Enemies.FirstOrDefault(x => x.Location == attackLocation);

            if (enemy != null && !blocked)
            {
                //Engage enemy if they're attacked
                enemy.IsEngaged = true;

                // Enemy dodges
                var hit = _interactionProcessor.CalculatePlayerHit(_modelService.Player, enemy);
                if (hit <= 0 || _randomSequenceGenerator.Get() < _characterProcessor.GetDodge(enemy))
                    _scenarioMessageService.Publish(player.RogueName + " Misses");

                else
                {
                    //Critical hit
                    if (_randomSequenceGenerator.Get() < _playerProcessor.GetCriticalHitProbability(player))
                    {
                        hit *= 2;
                        _scenarioMessageService.Publish(player.RogueName + " scores a critical hit!");
                    }
                    else
                        _scenarioMessageService.Publish(player.RogueName + " attacks");

                    enemy.Hp -= hit;

                    // TODO - process alterations attached to the player's equipment
                    //IEnumerable<Equipment> magicWeapons = this.Player.EquipmentInventory.Where(eq => eq.HasAttackSpell && eq.IsEquiped);
                    //foreach (Equipment eq in magicWeapons)
                    //{
                    //    //Set target so the enemy is processed with the spell
                    //    //e.IsTargeted = true;

                    //    ProcessPlayerMagicSpell(eq.AttackSpell);
                    //}
                }

                // Enemy counter-attacks
                if (_randomSequenceGenerator.Get() < enemy.BehaviorDetails.CurrentBehavior.CounterAttackProbability)
                {
                    player.Hp -= _interactionProcessor.CalculateEnemyHit(player, enemy);
                    _scenarioMessageService.Publish(enemy.RogueName + " counter attacks");
                }
            }
        }
        public LevelContinuationAction Throw(string itemId)
        {
            if (!_modelService.GetTargetedEnemies().Any())
                return LevelContinuationAction.DoNothing;

            var nextAction = LevelContinuationAction.DoNothing;
            var player = _modelService.Player;
            var enemy = _modelService.GetTargetedEnemies()
                                     .First();

            var thrownItem = player.Consumables[itemId];

            // Remove item from inventory
            player.Consumables.Remove(itemId);

            if (thrownItem.HasProjectileSpell)
            {
                //Get the spell and null the item
                var spell = thrownItem.ProjectileSpell;

                //Process the spell TODO
                //TODO - Change this to enemy invoke
                //nextAction = ProcessPlayerMagicSpell(s);
            }

            return nextAction;
        }
        public LevelContinuationAction Consume(string itemId)
        {
            var player = _modelService.Player;
            var consumable = player.Consumables[itemId];
            var meetsAlterationCost = _alterationProcessor.CalculatePlayerMeetsAlterationCost(player, consumable.Spell.Cost);
            var displayName = _modelService.GetDisplayName(consumable.RogueName);

            switch (consumable.Type)
            {
                case ConsumableType.OneUse:
                    if (meetsAlterationCost)
                        player.Consumables.Remove(itemId);
                    break;
                case ConsumableType.MultipleUses:
                    {
                        if (meetsAlterationCost)
                            consumable.Uses--;

                        if (consumable.Uses <= 0)
                            player.Consumables.Remove(itemId);
                    }
                    break;
                case ConsumableType.UnlimitedUses:
                default:
                    break;
            }

            if (consumable.HasLearnedSkillSet)
            {
                if (!player.SkillSets.Any(z => z.RogueName == consumable.LearnedSkill.RogueName))
                {
                    //Message plays on dungeon turn
                    player.SkillSets.Add(consumable.LearnedSkill);

                    _scenarioMessageService.Publish(player.RogueName + " has been granted a new skill!  \"" + consumable.LearnedSkill.RogueName + "\"");
                }
            }
            if (consumable.HasSpell || consumable.SubType == ConsumableSubType.Ammo)
            {
                var targetedEnemy = _modelService.GetTargetedEnemies()
                                                 .FirstOrDefault();

                if (_alterationProcessor.CalculateSpellRequiresTarget(consumable.Spell) && targetedEnemy == null)
                    _scenarioMessageService.Publish("Must first target an enemy");

                else
                {
                    _scenarioMessageService.Publish("Consuming " + displayName);
                    // TODO
                    //return ProcessPlayerMagicSpell((c.SubType == ConsumableSubType.Ammo) ? c.AmmoSpell : c.Spell);
                }
            }

            return LevelContinuationAction.ProcessTurn;
        }
        public void Identify(string itemId)
        {
            var item = _modelService.Player.Inventory[itemId];
            var metaData = _modelService.ScenarioEncyclopedia[item.RogueName];

            metaData.IsIdentified = true;
            metaData.IsCurseIdentified = true;
            item.IsIdentified = true;

            _scenarioMessageService.Publish(item.RogueName + " Identified");
        }
        public void Enchant(string id)
        {
            var equipment = _modelService.Player.Equipment[id];
            equipment.Class++;

            _scenarioMessageService.Publish("Your " + _modelService.GetDisplayName(equipment.RogueName) + " starts to glow!");
        }
        public void Uncurse(string itemId)
        {
            var equipment = _modelService.Player.Equipment[itemId];
            equipment.IsCursed = false;

            _scenarioMessageService.Publish(_modelService.GetDisplayName(equipment.RogueName) + " Uncursed");

            if (equipment.HasCurseSpell)
            {
                // TODO
                //if (equipment.EquipSpell.Type == AlterationType.PassiveSource)
                //    this.Player.DeactivatePassiveEffect(equipment.CurseSpell.Id);

                //else
                //    this.Player.DeactivatePassiveAura(equipment.CurseSpell.Id);

                _scenarioMessageService.Publish("Your " + equipment.RogueName + " is now safe to use (with caution...)");
            }
        }
        public void Drop(string itemId)
        {
            _contentEngine.DropPlayerItem(itemId);
        }
        public LevelContinuationAction Fire()
        {
            var rangeWeapon = _modelService.Player.Equipment.Values.FirstOrDefault(x => x.IsEquipped && x.Type == EquipmentType.RangeWeapon);
            var targetedEnemy = _modelService.GetTargetedEnemies().FirstOrDefault();

            if (rangeWeapon == null)
                _scenarioMessageService.Publish("No Range Weapons are equipped");

            else if (targetedEnemy == null)
                _scenarioMessageService.Publish("Must first target an enemy");

            else if (_layoutEngine.RoguianDistance(_modelService.Player.Location, targetedEnemy.Location) <= 2)
                _scenarioMessageService.Publish("Too close to fire your weapon");

            else
            {
                //Should find a better way to identify ammo
                var ammo = _modelService.Player.Consumables.Values.FirstOrDefault(z => z.RogueName == rangeWeapon.AmmoName);
                if (ammo == null)
                {
                    _scenarioMessageService.Publish("No Ammunition for your weapon");
                    return LevelContinuationAction.ProcessTurn;
                }

                // Remove ammo from inventory
                _modelService.Player.Consumables.Remove(ammo.Id);

                // Player Misses
                var hit = _interactionProcessor.CalculatePlayerHit(_modelService.Player, targetedEnemy);
                if (hit <= 0 || _randomSequenceGenerator.Get() < _characterProcessor.GetDodge(targetedEnemy))
                    _scenarioMessageService.Publish(_modelService.Player.RogueName + " Misses");

                // Player Hits Targeted Enemy
                else
                {
                    if (_randomSequenceGenerator.Get() < _playerProcessor.GetCriticalHitProbability(_modelService.Player))
                    {
                        _scenarioMessageService.Publish("You fire your " + rangeWeapon.RogueName + " for a critical hit!");
                        targetedEnemy.Hp -= hit * 2;
                    }
                    else
                    {
                        _scenarioMessageService.Publish("You hit the " + targetedEnemy.RogueName + " using your " + rangeWeapon.RogueName);
                        targetedEnemy.Hp -= hit;
                    }

                    // TODO
                    //return ProcessPlayerMagicSpell((c.SubType == ConsumableSubType.Ammo) ? c.AmmoSpell : c.Spell);
                }
            }
            return LevelContinuationAction.ProcessTurn;
        }
        public void Target(Compass direction)
        {
            var targetedEnemy = _modelService.GetTargetedEnemies().FirstOrDefault();
            var enemiesInRange = _modelService.GetVisibleEnemies().ToList();

            if (targetedEnemy != null)
            {
                int targetedEnemyIndex = enemiesInRange.IndexOf(targetedEnemy);
                switch (direction)
                {
                    case Compass.E:
                        {
                            if (targetedEnemyIndex + 1 == enemiesInRange.Count)
                                _modelService.SetTargetedEnemy(enemiesInRange[0]);
                            else
                                _modelService.SetTargetedEnemy(enemiesInRange[targetedEnemyIndex + 1]);
                        }
                        break;
                    case Compass.W:
                        {
                            if (targetedEnemyIndex - 1 == -1)
                                _modelService.SetTargetedEnemy(enemiesInRange[enemiesInRange.Count - 1]);
                            else
                                _modelService.SetTargetedEnemy(enemiesInRange[targetedEnemyIndex - 1]);
                        }
                        break;
                    default:
                        _modelService.SetTargetedEnemy(enemiesInRange[0]);
                        break;

                }
            }
            else
            {
                if (enemiesInRange.Count > 0)
                    _modelService.SetTargetedEnemy(enemiesInRange[0]);
            }
        }
        public void ToggleActiveSkill(string skillSetId, bool activate)
        {
            var skillSet = _modelService.Player.SkillSets.FirstOrDefault(z => z.Id == skillSetId);
            bool isActive = skillSet.IsActive;

            if (isActive && activate)
                return;

            if (!isActive && !activate)
                return;

            //Good measure - set non active for all skill sets
            foreach (var playerSkillSet in _modelService.Player.SkillSets)
                playerSkillSet.IsActive = false;

            //Shut off aura effects
            foreach (var s in _modelService.Player.SkillSets)
            {
                //Maintain aura effects
                if (s.IsTurnedOn)
                {

                    s.IsTurnedOn = false;
                    _scenarioMessageService.Publish("Deactivating " + s.RogueName);

                    // TODO
                    //if (s.CurrentSkill.Type == AlterationType.PassiveAura)
                    //    this.Player.DeactivatePassiveAura(s.CurrentSkill.Id);

                    //else
                    //    this.Player.DeactivatePassiveEffect(s.CurrentSkill.Id);
                }
            }

            if (skillSet != null)
                skillSet.IsActive = !isActive;
        }
        public void EmphasizeSkillUp(string skillSetId)
        {
            var skillSet = _modelService.Player.SkillSets.FirstOrDefault(z => z.Id == skillSetId);
            if (skillSet != null)
            {
                if (skillSet.Emphasis < 3)
                    skillSet.Emphasis++;
            }
        }
        public void EmphasizeSkillDown(string skillSetId)
        {
            var skillSet = _modelService.Player.SkillSets.FirstOrDefault(z => z.Id == skillSetId);
            if (skillSet != null)
            {
                if (skillSet.Emphasis > 0)
                    skillSet.Emphasis--;
            }
        }
        public LevelContinuationAction InvokePlayerSkill()
        {
            var activeSkillSet = _modelService.Player.SkillSets.FirstOrDefault(z => z.IsActive == true);

            // No Active Skill Set
            if (activeSkillSet == null)
            {
                _scenarioMessageService.Publish("No Active Skill");
                return LevelContinuationAction.ProcessTurn;
            }

            // No Current Skill (Could throw Exception)
            var currentSkill = activeSkillSet.GetCurrentSkill();
            if (currentSkill == null)
            {
                _scenarioMessageService.Publish("No Active Skill");
                return LevelContinuationAction.ProcessTurn;
            }

            var enemyTargeted = _modelService.GetTargetedEnemies().FirstOrDefault();

            // Requires Target
            if (_alterationProcessor.CalculateSpellRequiresTarget(activeSkillSet.GetCurrentSkill()) && enemyTargeted == null)
                _scenarioMessageService.Publish(activeSkillSet.RogueName + " requires a targeted enemy");

            // Meets Alteration Cost?
            if (!_alterationProcessor.CalculatePlayerMeetsAlterationCost(_modelService.Player, currentSkill.Cost))
                return LevelContinuationAction.ProcessTurn;

            // Deactivate passive if it's turned on
            if (activeSkillSet.GetCurrentSkill().Type == AlterationType.PassiveAura ||
                activeSkillSet.GetCurrentSkill().Type == AlterationType.PassiveSource)
            {
                if (activeSkillSet.IsTurnedOn)
                {
                    _scenarioMessageService.Publish("Deactivating - " + activeSkillSet.RogueName);
                    // TODO
                    //this.Player.DeactivatePassiveEffect(activeSkillSet.CurrentSkill.Id);
                    activeSkillSet.IsTurnedOn = false;
                }
                else
                    activeSkillSet.IsTurnedOn = true;
            }


            _scenarioMessageService.Publish("Invoking - " + activeSkillSet.RogueName);

            // TODO - Finally process the spell
            //return ProcessPlayerMagicSpell(activeSkillSet.CurrentSkill);

            return LevelContinuationAction.ProcessTurn;
        }
        public LevelContinuationAction InvokeDoodad()
        {
            var player = _modelService.Player;
            var doodad = _modelService.CurrentLevel.GetAtPoint<DoodadBase>(player.Location);
            if (doodad == null)
                return LevelContinuationAction.DoNothing;

            //Sets identified
            _modelService.ScenarioEncyclopedia[doodad.RogueName].IsIdentified = true;

            switch (doodad.Type)
            {
                case DoodadType.Magic:
                    {
                        if (doodad.IsOneUse && doodad.HasBeenUsed || !((DoodadMagic)doodad).IsInvoked)
                            _scenarioMessageService.Publish("Nothing Happens");
                        else
                        {
                            _scenarioMessageService.Publish("Using " + doodad.RogueName);

                            var doodadMagic = (DoodadMagic)doodad;
                            doodadMagic.HasBeenUsed = true;

                            return _spellEngine.InvokePlayerMagicSpell(doodadMagic.InvokedSpell);
                        }
                    }
                    break;
                case DoodadType.Normal:
                    {
                        switch (((DoodadNormal)doodad).NormalType)
                        {
                            // TODO
                            case DoodadNormalType.SavePoint:
                                //PublishSaveEvent();
                                break;
                            case DoodadNormalType.StairsDown:
                                //PublishLoadLevelRequest(this.Level.Number + 1, PlayerStartLocation.StairsUp);
                                break;
                            case DoodadNormalType.StairsUp:
                                //PublishLoadLevelRequest(this.Level.Number - 1, PlayerStartLocation.StairsDown);
                                break;
                        }
                    }
                    break;
            }

            return LevelContinuationAction.ProcessTurn;
        }

        public void ApplyEndOfTurn()
        {
            throw new NotImplementedException();
        }
    }
}

using Rogue.NET.Core.Logic.Algorithm.Interface;
using Rogue.NET.Core.Logic.Content.Interface;
using Rogue.NET.Core.Logic.Interface;
using Rogue.NET.Core.Logic.Processing;
using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Core.Logic.Processing.Interface;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Generator.Interface;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content.Doodad;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Service.Interface;
using Rogue.NET.Core.Utility;
using System;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rogue.NET.Core.Logic
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IContentEngine))]
    public class ContentEngine : IContentEngine
    {
        readonly IModelService _modelService;
        readonly IPathFinder _pathFinder;
        readonly ILayoutEngine _layoutEngine;
        readonly ISpellEngine _spellEngine;
        readonly IEnemyProcessor _enemyProcessor;
        readonly IPlayerProcessor _playerProcessor;        
        readonly ICharacterProcessor _characterProcessor;
        readonly IInteractionProcessor _interactionProcessor;
        readonly IScenarioMessageService _scenarioMessageService;
        readonly IRandomSequenceGenerator _randomSequenceGenerator;
        readonly ICharacterGenerator _characterGenerator;

        public event EventHandler<IScenarioUpdate> ScenarioUpdateEvent;
        public event EventHandler<ISplashUpdate> SplashUpdateEvent;
        public event EventHandler<ILevelUpdate> LevelUpdateEvent;
        public event EventHandler<IAnimationUpdate> AnimationUpdateEvent;
        public event EventHandler<ILevelProcessingAction> LevelProcessingActionEvent;

        [ImportingConstructor]
        public ContentEngine(
            IModelService modelService, 
            IPathFinder pathFinder,
            ILayoutEngine layoutEngine, 
            ISpellEngine spellEngine,
            IEnemyProcessor enemyProcessor,
            IPlayerProcessor playerProcessor,
            ICharacterProcessor characterProcessor,
            IInteractionProcessor interactionProcessor,
            IScenarioMessageService scenarioMessageService,
            IRandomSequenceGenerator randomSequenceGenerator,
            ICharacterGenerator characterGenerator)
        {
            _modelService = modelService;
            _pathFinder = pathFinder;
            _layoutEngine = layoutEngine;
            _spellEngine = spellEngine;
            _enemyProcessor = enemyProcessor;
            _playerProcessor = playerProcessor;
            _characterProcessor = characterProcessor;
            _interactionProcessor = interactionProcessor;
            _scenarioMessageService = scenarioMessageService;
            _randomSequenceGenerator = randomSequenceGenerator;
            _characterGenerator = characterGenerator;
        }

        #region (public) Methods
        public void StepOnItem(Character character, ItemBase item)
        {
            var level = _modelService.Level;
            var haulMax = _characterProcessor.GetHaulMax(character);
            var projectedHaul = item.Weight + _characterProcessor.GetHaul(character);

            if (haulMax >= projectedHaul)
            {
                // Add to Character's inventory
                if (item is Consumable)
                    character.Consumables.Add(item.Id, item as Consumable);

                else if (item is Equipment)
                    character.Equipment.Add(item.Id, item as Equipment);

                // Remove from the level
                level.RemoveContent(item);

                // Queue level update event for removed item
                LevelUpdateEvent(this, new LevelUpdate()
                {
                    LevelUpdateType = LevelUpdateType.ContentRemove,
                    ContentIds = new string[] { item.Id }
                });

                if (character is Player)
                {
                    // Publish message
                    _scenarioMessageService.Publish("Found " + _modelService.GetDisplayName(item.RogueName));

                    if (item is Consumable)
                        LevelUpdateEvent(this, new LevelUpdate()
                        {
                            LevelUpdateType = LevelUpdateType.PlayerConsumableAddOrUpdate,
                            ContentIds = new string[] { item.Id }
                        });
                    else if (item is Equipment)
                        LevelUpdateEvent(this, new LevelUpdate()
                        {
                            LevelUpdateType = LevelUpdateType.PlayerEquipmentAddOrUpdate,
                            ContentIds = new string[] { item.Id }
                        });

                    //Update level statistics
                    QueueScenarioStatisticsUpdate(ScenarioUpdateType.StatisticsItemFound, item.RogueName);
                }
            }
            else if (character is Player)
                _scenarioMessageService.Publish("Too much weight in your inventory");
        }
        public void StepOnDoodad(Character character, DoodadBase doodad)
        {
            // Show the Doodad (could be hidden)
            doodad.IsHidden = false;

            switch (doodad.Type)
            {
                case DoodadType.Normal:
                    StepOnDoodadNormal(character, doodad as DoodadNormal);
                    break;
                case DoodadType.Magic:
                    StepOnDoodadMagic(character, doodad as DoodadMagic);
                    break;
            }
        }
        public bool Equip(string equipId)
        {
            // Fetch reference to equipment
            var equipment = _modelService.Player.Equipment[equipId];
            var result = false;

            // Process the update
            if (equipment.IsEquipped)
                result = UnEquip(equipment);

            else
                result = Equip(equipment);

            // Queue player update for this item
            QueuePlayerEquipmentAddOrUpdate(equipId);

            return result;
        }
        public void DropPlayerItem(string itemId)
        {
            var item = _modelService.Player.Inventory[itemId];
            var adjacentFreeLocations = _layoutEngine.GetFreeAdjacentLocations(_modelService.Level, _modelService.Player, _modelService.Player.Location);
            var dropLocation = adjacentFreeLocations.FirstOrDefault();

            if (dropLocation == null)
            {
                _scenarioMessageService.Publish("Cannot drop item here");
                return;
            }

            var displayName = _modelService.GetDisplayName(item.RogueName);

            if (item is Equipment)
            {
                var equipment = item as Equipment;
                if (equipment.IsEquipped)
                {
                    if (!Equip(equipment.Id))
                        return;
                }

                // Set item location
                equipment.Location = dropLocation;

                // Remove from inventory
                _modelService.Player.Equipment.Remove(equipment.Id);

                // Add level content
                _modelService.Level.AddContent(equipment);

                // Queue updates
                QueuePlayerEquipmentRemove(equipment.Id);
                QueueLevelUpdate(LevelUpdateType.ContentAll, string.Empty);
            }
            if (item is Consumable)
            {
                var consumable = item as Consumable;

                // Set item location
                consumable.Location = dropLocation;

                // Remove from inventory
                _modelService.Player.Consumables.Remove(consumable.Id);

                // Add level content
                _modelService.Level.AddContent(consumable);

                // Queue updates
                QueuePlayerConsumableRemove(consumable.Id);
                QueueLevelUpdate(LevelUpdateType.ContentAll, string.Empty);
            }

            // Publish message
            _scenarioMessageService.Publish(displayName + " Dropped");
        }
        public void EnemyDeath(Enemy enemy)
        {
            for (int i = enemy.Equipment.Count - 1; i >= 0; i--)
            {
                DropEnemyItem(enemy, enemy.Equipment.ElementAt(i).Value);
            }
            for (int i = enemy.Consumables.Count - 1; i >= 0; i--)
            {
                DropEnemyItem(enemy, enemy.Consumables.ElementAt(i).Value);
            }

            //Update level object
            var level = _modelService.Level;

            level.RemoveContent(enemy);

            // Update statistics
            QueueScenarioStatisticsUpdate(ScenarioUpdateType.StatisticsEnemyDeath, enemy.RogueName);

            // TODO: Consider where to put this
            _modelService.Player.Experience += enemy.ExperienceGiven;

            //Skill Progress - Player gets boost on enemy death
            _playerProcessor.ProcessSkillLearning(_modelService.Player);

            _scenarioMessageService.Publish(enemy.RogueName + " Slayed");

            //Set enemy identified
            _modelService.ScenarioEncyclopedia[enemy.RogueName].IsIdentified = true;

            // Publish Level update
            QueueLevelUpdate(LevelUpdateType.ContentRemove, enemy.Id);
        }
        public void CalculateEnemyReactions()
        {
            var level = _modelService.Level;

            // Enemy Reactions: 0) Check whether enemy is still alive 
            //                  1) Process Enemy Reaction (Applies End-Of-Turn)
            //                  2) Check for Enemy Death (After Enemy Reaction)
            for (int i = level.Enemies.Count() - 1; i >= 0; i--)
            {
                var enemy = level.Enemies.ElementAt(i);

                if (enemy.Hp <= 0)
                    EnemyDeath(enemy);
                else
                    LevelProcessingActionEvent(this, new LevelProcessingAction()
                    {
                        CharacterId = enemy.Id,
                        Type = LevelProcessingActionType.EnemyReaction
                    });
            }
        }
        public void ProcessEnemyReaction(Enemy enemy)
        {
            // Don't let Enemy get the last word. Check this here to prevent Enemy Death checks every where else.
            if (enemy.Hp <= 0)
            {
                EnemyDeath(enemy);
                return;
            }

            var level = _modelService.Level;
            var player = _modelService.Player;

            double dist = _layoutEngine.EuclideanDistance(enemy.Location, player.Location);

            //Check for engaged
            if (dist < enemy.BehaviorDetails.CurrentBehavior.EngageRadius)
                enemy.IsEngaged = true;

            if (dist > enemy.BehaviorDetails.CurrentBehavior.DisengageRadius)
                enemy.IsEngaged = false;

            if (!enemy.IsEngaged)
                return;

            enemy.TurnCounter = _interactionProcessor.CalculateEnemyTurn(player, enemy);

            if (enemy.TurnCounter >= 1)
                OnEnemyReaction(enemy);

            if (enemy.Hp <= 0)
                EnemyDeath(enemy);
        }
        public void ApplyEndOfTurn()
        {
            ProcessMonsterGeneration();
        }
        #endregion

        #region (private) Sub-Methods
        private void StepOnDoodadNormal(Character character, DoodadNormal doodad)
        {
            var level = _modelService.Level;
            var metaData = _modelService.ScenarioEncyclopedia[doodad.RogueName];

            if (character is Player)
            {
                metaData.IsIdentified = true;
                doodad.IsHidden = false;
            }

            switch (doodad.NormalType)
            {
                case DoodadNormalType.SavePoint:
                case DoodadNormalType.StairsDown:
                case DoodadNormalType.StairsUp:
                    if (character is Player)
                    {
                        var doodadTitle = TextUtility.CamelCaseToTitleCase(doodad.NormalType.ToString());
                        _scenarioMessageService.Publish(doodadTitle + " (Press \"D\" to Use)");
                    }
                    break;
                case DoodadNormalType.TeleportRandom:
                    {
                        character.Location = _layoutEngine.GetRandomLocation(_modelService.Level, true);
                        if (character is Player)
                            _scenarioMessageService.Publish("Teleport!");

                        // Queue update event for character location
                        LevelUpdateEvent(this, new LevelUpdate()
                        {
                            LevelUpdateType  = LevelUpdateType.ContentMove,
                            ContentIds = new string[] {character.Id}
                        });
                    }
                    break;
                case DoodadNormalType.Teleport1:
                case DoodadNormalType.Teleport2:
                    {
                        var otherTeleporter = level.DoodadsNormal.First(x => x.PairId == doodad.Id);

                        // Show the other teleporter also if it's hidden
                        otherTeleporter.IsHidden = false;

                        // Have to boot enemy if it's sitting on other teleporter
                        if (character is Player && level.IsCellOccupiedByEnemy(otherTeleporter.Location))
                        {
                            var enemy = level.GetAtPoint<Enemy>(otherTeleporter.Location);

                            // Remove from the level
                            level.RemoveContent(enemy);

                            // Queue update to the level
                            LevelUpdateEvent(this, new LevelUpdate()
                            {
                                LevelUpdateType = LevelUpdateType.ContentRemove,
                                ContentIds = new string[] {enemy.Id}
                            });
                        }
                        // Enemy is trying to teleport in where Player is
                        else if (character is Enemy && character.Location == _modelService.Player.Location)
                        {
                            _scenarioMessageService.Publish("Enemy is trying to use your teleporter!");
                            break;
                        }

                        // Set character location to other teleporter
                        character.Location = otherTeleporter.Location;

                        // Queue update to level 
                        LevelUpdateEvent(this, new LevelUpdate()
                        {
                            LevelUpdateType = LevelUpdateType.ContentMove,
                            ContentIds = new string[] { character.Id }
                        });

                        if (character is Player)
                            _scenarioMessageService.Publish("Teleport!");

                        break;
                    }
            }
        }
        private void StepOnDoodadMagic(Character character, DoodadMagic doodad)
        {
            var level = _modelService.Level;
            var metaData = _modelService.ScenarioEncyclopedia[doodad.RogueName];

            if (character is Player)
            {
                var displayName = _modelService.GetDisplayName(doodad.RogueName);

                if (!(doodad.IsOneUse && doodad.HasBeenUsed))
                {
                    if (doodad.IsAutomatic)
                    {
                        // Mark that the doodad has been used
                        doodad.HasBeenUsed = true;

                        // Queue magic spell with animation
                        _spellEngine.QueuePlayerMagicSpell(doodad.AutomaticSpell);
                    }
                    else
                        _scenarioMessageService.Publish(displayName + " Press \"D\" to Use");
                }
                else
                    _scenarioMessageService.Publish(displayName + " seems to be inactive");
            }
        }
        private bool UnEquip(Equipment equipment)
        {
            var metaData = _modelService.ScenarioEncyclopedia[equipment.RogueName];

            if (equipment.IsCursed)
            {
                // Publish message
                _scenarioMessageService.Publish(_modelService.GetDisplayName(equipment.RogueName) + " is Cursed!!!");

                // Set Curse Identified
                metaData.IsCurseIdentified = true;

                return false;
            }
            else
            {
                // Unequip
                equipment.IsEquipped = false;

                // Publish Message
                _scenarioMessageService.Publish("Un-Equipped " + (metaData.IsIdentified ? equipment.RogueName : "???"));
                
                // TODO
                // If equip spell is present - make sure it's deactivated and removed
                //if (equipment.HasEquipSpell)
                //{
                //    if (e.EquipSpell.Type == AlterationType.PassiveSource)
                //        _modelService.Player.DeactivatePassiveEffect(e.EquipSpell.Id);

                //    else
                //        _modelService.Player.DeactivatePassiveAura(e.EquipSpell.Id);
                //}

                return true;
            }

        }
        private bool Equip(Equipment equipment)
        {
            switch (equipment.Type)
            {
                case EquipmentType.Armor:
                case EquipmentType.Boots:
                case EquipmentType.Gauntlets:
                case EquipmentType.Helmet:
                case EquipmentType.Amulet:
                case EquipmentType.Orb:
                case EquipmentType.Belt:
                case EquipmentType.Shoulder:
                    {
                        var equippedItem = _playerProcessor.GetEquippedType(_modelService.Player, equipment.Type);
                        if (equippedItem != null)
                        {
                            _scenarioMessageService.Publish("Must first un-equip " + _modelService.GetDisplayName(equippedItem.RogueName));

                            return false;
                        }
                    }
                    break;
                case EquipmentType.TwoHandedMeleeWeapon:
                case EquipmentType.OneHandedMeleeWeapon:
                case EquipmentType.Shield:
                case EquipmentType.RangeWeapon:
                    {
                        var handsFree = _playerProcessor.GetNumberOfFreeHands(_modelService.Player);

                        if (((equipment.Type == EquipmentType.TwoHandedMeleeWeapon || equipment.Type == EquipmentType.RangeWeapon) && handsFree < 2) || handsFree < 1)
                        {
                            _scenarioMessageService.Publish("Must first free up a hand");
                            return false;
                        }
                    }
                    break;
                case EquipmentType.Ring:
                    {
                        var ring = _playerProcessor.GetEquippedType(_modelService.Player, EquipmentType.Ring);
                        var numberEquipped = _playerProcessor.GetNumberEquipped(_modelService.Player, EquipmentType.Ring);

                        if (ring != null && numberEquipped >= 2)
                        {
                            _scenarioMessageService.Publish("Must first un-equip " + _modelService.GetDisplayName(equipment.RogueName));

                            return false;
                        }
                    }
                    break;
                default:
                    break;
            }

            equipment.IsEquipped = true;

            _scenarioMessageService.Publish("Equipped " + _modelService.GetDisplayName(equipment.RogueName));

            // Fire equip spell
            if (equipment.HasEquipSpell)
                _spellEngine.QueuePlayerMagicSpell(equipment.EquipSpell);

            if (equipment.HasCurseSpell && equipment.IsCursed)
                _spellEngine.QueuePlayerMagicSpell(equipment.CurseSpell);

            return true;
        }
        private void DropEnemyItem(Enemy enemy, ItemBase item)
        {
            var adjacentFreeLocations = _layoutEngine.GetFreeAdjacentLocations(_modelService.Level, _modelService.Player, enemy.Location);
            var location = adjacentFreeLocations.FirstOrDefault();

            if (location == null)
                return;

            // Provide new location for item
            item.Location = location;

            // Remove from enemy inventory
            if (item is Equipment)
                enemy.Equipment.Remove(item.Id);

            if (item is Consumable)
                enemy.Consumables.Remove(item.Id);

            // Add to level
            _modelService.Level.AddContent(item);
        }
        #endregion

        #region (private) Enemy Reactions
        private void OnEnemyReaction(Enemy enemy)
        {
            // Sets turn counter 
            int turns = (int)enemy.TurnCounter;
            enemy.TurnCounter = enemy.TurnCounter % 1;

            for (int j = 0; j < turns; j++)
            {
                //Check altered states
                var alteredStates = enemy.Alteration.GetStates();

                //Sleeping
                if (alteredStates.Any(z => z == CharacterStateType.Sleeping))
                    continue;

                //Paralyzed
                else if (alteredStates.Any(z => z == CharacterStateType.Paralyzed))
                    continue;

                //Confused - check during calculate character move

                var actionTaken = false;

                switch (enemy.BehaviorDetails.CurrentBehavior.AttackType)
                {
                    case CharacterAttackType.Melee:
                        {
                            var adjacentCells = _layoutEngine.GetAdjacentLocations(_modelService.Level.Grid, enemy.Location);
                            var attackLocation = adjacentCells.FirstOrDefault(z => z == _modelService.Player.Location);

                            if (attackLocation != null) // TODO && 
                                                        //!e.States.Any(z => z == CharacterStateType.Confused))
                            {
                                if (!_layoutEngine.IsPathToCellThroughWall(_modelService.Level.Grid, enemy.Location, attackLocation))
                                {
                                    OnEnemyMeleeAttack(enemy);
                                    actionTaken = true;
                                }
                            }
                        }
                        break;
                    case CharacterAttackType.Skill:
                        // TODO
                        //else if (e.BehaviorDetails.CurrentBehavior.AttackType == CharacterAttackType.Skill
                        //    && dist < e.BehaviorDetails.CurrentBehavior.EngageRadius
                        //    && this.Level.Grid.GetVisibleCells().Any(z => z.Location.Equals(e.Location))
                        //    && !e.States.Any(z => z == CharacterStateType.Confused))
                        //{
                        //    ProcessEnemyInvokeSkill(e);
                        //    e.BehaviorDetails.SetPrimaryInvoked();
                        //    actionTaken = true;
                        //}
                        break;
                    case CharacterAttackType.None:
                    default:
                        break;
                }

                if (!actionTaken)
                {
                    var moveLocation = CalculateEnemyMoveLocation(enemy, _modelService.Player.Location);
                    if (moveLocation != null)
                        ProcessEnemyMove(enemy, moveLocation);
                }

                // Apply end-of-turn behavior for enemy
                _enemyProcessor.ApplyEndOfTurn(enemy);
            }
        }
        private void OnEnemyMeleeAttack(Enemy enemy)
        {
            var player = _modelService.Player;
            var hit = _interactionProcessor.CalculateEnemyHit(player, enemy);
            var dodge = _randomSequenceGenerator.Get() <= _characterProcessor.GetDodge(player);
            var criticalHit = _randomSequenceGenerator.Get() <= enemy.BehaviorDetails.CurrentBehavior.CriticalRatio;

            if (hit > 0 && !dodge)
            {
                if (criticalHit)
                {
                    hit *= 2;
                    _scenarioMessageService.Publish(enemy.RogueName + " attacks for a critical hit!");
                }
                else
                    _scenarioMessageService.Publish(enemy.RogueName + " attacks");

                player.Hp -= hit;
            }
            else
                _scenarioMessageService.Publish(enemy.RogueName + " Misses");
        }
        private CellPoint CalculateEnemyMoveLocation(Enemy enemy, CellPoint desiredLocation)
        {
            // TODO
            //if (states.Any(z => z == CharacterStateType.Confused))
            //{
            //    //Return random if confused
            //    List<Cell> adjCells = Helper.GetAdjacentCells(location.Row, location.Column, this.Level.Grid);
            //    return adjCells[this.Random.Next(0, adjCells.Count)];
            //}

            switch (enemy.BehaviorDetails.CurrentBehavior.MovementType)
            {
                case CharacterMovementType.Random:
                    return _layoutEngine.GetRandomAdjacentLocation(_modelService.Level, _modelService.Player, enemy.Location, true);
                case CharacterMovementType.HeatSeeker:
                    return _layoutEngine.GetFreeAdjacentLocationsForMovement(_modelService.Level, _modelService.Player, enemy.Location)
                                        .OrderBy(x => _layoutEngine.RoguianDistance(x, desiredLocation))
                                        .FirstOrDefault();
                case CharacterMovementType.StandOffIsh:
                    return _layoutEngine.GetFreeAdjacentLocationsForMovement(_modelService.Level, _modelService.Player, enemy.Location)
                                        .OrderBy(x => _layoutEngine.RoguianDistance(x, desiredLocation))
                                        .LastOrDefault();
                case CharacterMovementType.PathFinder:
                    var nextLocation = _pathFinder.FindPath(enemy.Location, enemy.Location, enemy.BehaviorDetails.CurrentBehavior.DisengageRadius);
                    return nextLocation ?? _layoutEngine.GetFreeAdjacentLocationsForMovement(_modelService.Level,  _modelService.Player, enemy.Location)
                                                        .OrderBy(x => _layoutEngine.RoguianDistance(x, desiredLocation))
                                                        .FirstOrDefault();
                default:
                    throw new Exception("Unknown Enemy Movement Type");
            }
        }

        // Processes logic for found path point. This includes anything required for Enemy to
        // relocate to point "moveLocation". This point has been calculated as the next point
        // towards Player
        private void ProcessEnemyMove(Enemy enemy, CellPoint moveLocation)
        {
            // Case where path finding algorithm returns null; or heat seeking algorithm
            // returns null.
            if (moveLocation == null)
                return;

            // Check for opening of doors
            var openingPosition1 = CellPoint.Empty;
            var openingPosition2 = CellPoint.Empty;
            var openingDirection2 = Compass.Null;
            var shouldMoveToOpeningPosition1 = false;

            var moveDirection = _layoutEngine.GetDirectionBetweenAdjacentPoints(enemy.Location, moveLocation);

            var throughDoor = moveDirection == Compass.Null ? false : _layoutEngine.IsPathToCellThroughDoor(
                                _modelService.Level.Grid, 
                                enemy.Location, 
                                moveDirection, 
                                out openingPosition1, 
                                out openingPosition2, 
                                out openingDirection2,
                                out shouldMoveToOpeningPosition1);

            // Behavior allows opening of doors
            if (enemy.BehaviorDetails.CurrentBehavior.CanOpenDoors && throughDoor)
            {
                // If have to move into position first then move
                if (shouldMoveToOpeningPosition1)
                {
                    // Have to move to opening position 1 first - which means the door is on one of the off-diagonal locations
                    if (_layoutEngine.IsPathToAdjacentCellBlocked(_modelService.Level, enemy.Location, openingPosition1, true))
                    {
                        // Update enemy location
                        enemy.Location = openingPosition1;

                        // Notify listener queue
                        LevelUpdateEvent(this, new LevelUpdate()
                        {
                            LevelUpdateType = LevelUpdateType.ContentMove,
                            ContentIds = new string[] { enemy.Id }
                        });
                    }
                }

                // Can open the door where the enemy is at
                else
                {
                    // Open the door
                    _layoutEngine.ToggleDoor(_modelService.Level.Grid, moveDirection, enemy.Location);

                    // Notify listener queue
                    LevelUpdateEvent(this, new LevelUpdate() { LevelUpdateType = LevelUpdateType.LayoutTopology });
                }
            }
            else if (!throughDoor &&
                     !_layoutEngine.IsPathToAdjacentCellBlocked(_modelService.Level, enemy.Location, moveLocation, true))
            {
                // Update enemy location
                enemy.Location = moveLocation;

                // Notify listener queue
                LevelUpdateEvent(this, new LevelUpdate()
                {
                    LevelUpdateType = LevelUpdateType.ContentMove,
                    ContentIds = new string[] { enemy.Id }
                });
            }

            // Check for items
            var item = _modelService.Level.GetAtPoint<ItemBase>(enemy.Location);
            if (item != null)
                StepOnItem(enemy, item);

            // Check for doodad
            var doodad = _modelService.Level.GetAtPoint<DoodadBase>(enemy.Location);
            if (doodad != null)
                StepOnDoodad(enemy, doodad);
        }
        #endregion

        #region (private) End-Of-Turn Methods
        private void ProcessMonsterGeneration()
        {
            // Create monster on generation rate roll
            var createMonster = _modelService.ScenarioConfiguration.DungeonTemplate.MonsterGenerationBase > _randomSequenceGenerator.Get();

            if (!createMonster)
                return;

            // Select enemy templates with: 0) this level range, 2) non-objective, 3) already generated unique enemies
            var enemyTemplates = _modelService.ScenarioConfiguration
                                              .EnemyTemplates
                                              .Where(x =>
                                              {
                                                  return !(x.IsUnique && x.HasBeenGenerated) &&
                                                          !x.IsObjectiveItem &&
                                                           x.Level.Contains(_modelService.Level.Number);
                                              })
                                              .ToList();

            if (enemyTemplates.Count <= 0)
                return;

            // Create enemy from template
            var template = enemyTemplates[_randomSequenceGenerator.Get(0, enemyTemplates.Count)];
            var enemy = _characterGenerator.GenerateEnemy(template);
            
            // Map enemy location to level
            enemy.Location = _layoutEngine.GetRandomLocation(_modelService.Level, true);

            // Add content to level
            _modelService.Level.AddContent(enemy);
        }
        #endregion

        #region (private) Queue Methods
        private void QueueLevelUpdate(LevelUpdateType type, string contentId)
        {
            LevelUpdateEvent(this, new LevelUpdate()
            {
                LevelUpdateType = type,
                ContentIds = new string[] {contentId}
            });
        }
        private void QueueScenarioStatisticsUpdate(ScenarioUpdateType type, string rogueName)
        {
            ScenarioUpdateEvent(this, new ScenarioUpdate()
            {
                ScenarioUpdateType  = type,
                ContentRogueName = rogueName
            });
        }
        private void QueuePlayerEquipmentRemove(string equipmentId)
        {
            LevelUpdateEvent(this, new LevelUpdate()
            {
                LevelUpdateType = LevelUpdateType.PlayerEquipmentAddOrUpdate,
                ContentIds = new string[] { equipmentId }
            });
        }
        private void QueuePlayerEquipmentAddOrUpdate(string equipmentId)
        {
            LevelUpdateEvent(this, new LevelUpdate()
            {
                LevelUpdateType = LevelUpdateType.PlayerEquipmentAddOrUpdate,
                ContentIds = new string[] { equipmentId }
            });
        }
        private void QueuePlayerConsumableAddOrUpdate(string consumableId)
        {
            LevelUpdateEvent(this, new LevelUpdate()
            {
                LevelUpdateType = LevelUpdateType.PlayerConsumableAddOrUpdate,
                ContentIds = new string[] { consumableId }
            });
        }
        private void QueuePlayerConsumableRemove(string consumableId)
        {
            LevelUpdateEvent(this, new LevelUpdate()
            {
                LevelUpdateType = LevelUpdateType.PlayerConsumableRemove,
                ContentIds = new string[] { consumableId }
            });
        }
        #endregion
    }
}

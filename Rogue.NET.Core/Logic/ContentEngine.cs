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
            var level = _modelService.CurrentLevel;
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

                if (character is Player)
                {
                    // Publish message
                    _scenarioMessageService.Publish("Found " + _modelService.GetDisplayName(item.RogueName));
                    
                    //Update level statistics
                    if (!level.ItemsFound.ContainsKey(item.RogueName))
                        level.ItemsFound.Add(item.RogueName, 1);
                    else
                        level.ItemsFound[item.RogueName]++;
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
            var equipment = _modelService.Player.Equipment[equipId];

            if (equipment.IsEquipped)
                return UnEquip(equipment);

            else
                return Equip(equipment);
        }
        public void DropPlayerItem(string itemId)
        {
            var item = _modelService.Player.Inventory[itemId];
            var adjacentFreeLocations = _layoutEngine.GetFreeAdjacentLocations(_modelService.CurrentLevel, _modelService.Player.Location);
            var location = adjacentFreeLocations.FirstOrDefault();

            if (location == null)
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
                equipment.Location = location;

                // Remove from inventory
                _modelService.Player.Equipment.Remove(equipment.Id);

                // Add level content
                _modelService.CurrentLevel.AddContent(equipment);

                // TODO
                //Deactivate passives
                //if (equipment.HasEquipSpell)
                //{
                //    if (equipment.EquipSpell.Type == AlterationType.PassiveSource)
                //        this.Player.DeactivatePassiveEffect(equipment.EquipSpell.Id);

                //    else
                //        this.Player.DeactivatePassiveAura(equipment.EquipSpell.Id);
                //}

                // Publish message
                _scenarioMessageService.Publish(displayName + " Dropped");
            }
            if (item is Consumable)
            {
                var consumable = item as Consumable;

                // Set item location
                consumable.Location = location;

                // Remove from inventory
                _modelService.Player.Consumables.Remove(consumable.Id);

                // Add level content
                _modelService.CurrentLevel.AddContent(consumable);

                // Publish message
                _scenarioMessageService.Publish(displayName + " Dropped");
            }
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
            var level = _modelService.CurrentLevel;

            level.RemoveContent(enemy);
            level.MonsterScore += (int)enemy.ExperienceGiven;

            if (!level.MonstersKilled.ContainsKey(enemy.RogueName))
                level.MonstersKilled.Add(enemy.RogueName, 1);
            else
                level.MonstersKilled[enemy.RogueName]++;

            // TODO: Consider where to put this
            _modelService.Player.Experience += enemy.ExperienceGiven;

            //Skill Progress - Player gets boost on enemy death
            _playerProcessor.ProcessSkillLearning(_modelService.Player);

            _scenarioMessageService.Publish(enemy.RogueName + " Slayed");

            //Set enemy identified
            _modelService.ScenarioEncyclopedia[enemy.RogueName].IsIdentified = true;
        }
        public void CalculateEnemyReactions()
        {
            var level = _modelService.CurrentLevel;

            // Enemy Reactions: 0) Check whether enemy is still alive 
            //                  1) Process Enemy Reaction (Applies End-Of-Turn)
            //                  2) Check for Enemy Death
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

                if (enemy.Hp <= 0)
                    EnemyDeath(enemy);
            }
        }
        public void ProcessEnemyReaction(Enemy enemy)
        {
            var level = _modelService.CurrentLevel;
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
        }
        public void ApplyEndOfTurn()
        {
            ProcessMonsterGeneration();
        }
        #endregion

        #region (private) Sub-Methods
        private void StepOnDoodadNormal(Character character, DoodadNormal doodad)
        {
            var level = _modelService.CurrentLevel;
            var metaData = _modelService.ScenarioEncyclopedia[doodad.RogueName];

            if (character is Player)
                metaData.IsIdentified = true;

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
                        character.Location = _layoutEngine.GetRandomLocation(_modelService.CurrentLevel, true);
                        if (character is Player)
                            _scenarioMessageService.Publish("Teleport!");
                    }
                    break;
                case DoodadNormalType.Teleport1:
                case DoodadNormalType.Teleport2:
                    {
                        //Identify regardless of character
                        metaData.IsIdentified = true;

                        var otherTeleporter = level.DoodadsNormal.First(x => x.PairId == doodad.Id);

                        otherTeleporter.IsHidden = false;

                        // Have to boot enemy if it's sitting on other teleporter
                        if (level.IsCellOccupiedByEnemy(otherTeleporter.Location))
                        {
                            var enemy = level.GetAtPoint<Enemy>(otherTeleporter.Location);

                            level.RemoveContent(enemy);
                        }

                        // Set character location to other teleporter
                        character.Location = otherTeleporter.Location;

                        if (character is Player)
                            _scenarioMessageService.Publish("Teleport!");

                        break;
                    }
            }
        }
        private void StepOnDoodadMagic(Character c, DoodadMagic d)
        {
            var level = _modelService.CurrentLevel;
            var metaData = _modelService.ScenarioEncyclopedia[d.RogueName];

            if (c is Player)
            {
                var displayName = _modelService.GetDisplayName(d.RogueName);

                if (!(d.IsOneUse && d.HasBeenUsed))
                {
                    if (d.IsAutomatic)
                    {
                        // TODO
                        //nextAction = ProcessPlayerMagicSpell(dm.AutomaticSpell);
                        //dm.HasBeenUsed = true;
                    }
                    else
                        _scenarioMessageService.Publish(displayName + " Press \"D\" to Use");
                }
                else
                    _scenarioMessageService.Publish(displayName);
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
                //if (e.HasEquipSpell)
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

            // TODO
            //Fire equip spell
            //if (equipment.HasEquipSpell)
            //    ProcessPlayerMagicSpell(equipment.EquipSpell);

            //if (equipment.HasCurseSpell && equipment.IsCursed)
            //    ProcessPlayerMagicSpell(equipment.CurseSpell);

            return true;
        }
        private void DropEnemyItem(Enemy enemy, ItemBase item)
        {
            var adjacentFreeLocations = _layoutEngine.GetFreeAdjacentLocations(_modelService.CurrentLevel, enemy.Location);
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
            _modelService.CurrentLevel.AddContent(item);
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
                            var adjacentCells = _layoutEngine.GetAdjacentLocations(_modelService.CurrentLevel.Grid, enemy.Location);
                            var attackLocation = adjacentCells.FirstOrDefault(z => z == _modelService.Player.Location);

                            if (attackLocation != null) // TODO && 
                                                        //!e.States.Any(z => z == CharacterStateType.Confused))
                            {
                                if (!_layoutEngine.IsPathToAdjacentCellBlocked(_modelService.CurrentLevel, enemy.Location, attackLocation))
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
        private void OnEnemyMeleeAttack(Enemy e)
        {
            var player = _modelService.Player;
            if (!_layoutEngine.IsPathToAdjacentCellBlocked(_modelService.CurrentLevel, e.Location, player.Location))
            {
                var hit = _interactionProcessor.CalculateEnemyHit(player, e);
                var dodge = _randomSequenceGenerator.Get() <= _characterProcessor.GetDodge(player);
                var criticalHit = _randomSequenceGenerator.Get() <= e.BehaviorDetails.CurrentBehavior.CriticalRatio;

                if (hit > 0 && !dodge)
                {
                    if (criticalHit)
                    {
                        hit *= 2;
                        _scenarioMessageService.Publish(e.RogueName + " attacks for a critical hit!");
                    }
                    else
                        _scenarioMessageService.Publish(e.RogueName + " attacks");

                    player.Hp -= hit;
                }
                else
                    _scenarioMessageService.Publish(e.RogueName + " Misses");
            }
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
                    return _layoutEngine.GetRandomAdjacentLocation(_modelService.CurrentLevel, enemy.Location, true);
                case CharacterMovementType.HeatSeeker:
                    return _layoutEngine.GetFreeAdjacentLocationsForMovement(_modelService.CurrentLevel, enemy.Location)
                                        .OrderBy(x => _layoutEngine.RoguianDistance(x, desiredLocation))
                                        .First();
                case CharacterMovementType.StandOffIsh:
                    return _layoutEngine.GetFreeAdjacentLocationsForMovement(_modelService.CurrentLevel, enemy.Location)
                                        .OrderBy(x => _layoutEngine.RoguianDistance(x, desiredLocation))
                                        .Last();
                case CharacterMovementType.PathFinder:
                    var nextLocation = _pathFinder.FindPath(enemy.Location, enemy.Location, enemy.BehaviorDetails.CurrentBehavior.DisengageRadius);
                    return nextLocation ?? _layoutEngine.GetFreeAdjacentLocationsForMovement(_modelService.CurrentLevel, enemy.Location)
                                                        .OrderBy(x => _layoutEngine.RoguianDistance(x, desiredLocation))
                                                        .First();

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
            var openingPosition = CellPoint.Empty;
            var openingDirection = Compass.Null;

            var throughDoor = _layoutEngine.IsCellThroughDoor(_modelService.CurrentLevel.Grid, enemy.Location, moveLocation, out openingPosition, out openingDirection);

            // Behavior allows opening of doors
            if (enemy.BehaviorDetails.CurrentBehavior.CanOpenDoors && throughDoor)
            {
                var desiredDirection = _layoutEngine.GetDirectionBetweenAdjacentPoints(enemy.Location, moveLocation);

                // If not in a cardinally adjacent position then move into that position before opening.
                if (enemy.Location == openingPosition)
                    _layoutEngine.ToggleDoor(_modelService.CurrentLevel.Grid, openingDirection, enemy.Location);

                else
                {
                    if (_layoutEngine.IsPathToAdjacentCellBlocked(_modelService.CurrentLevel, enemy.Location, openingPosition))
                        enemy.Location = openingPosition;
                }
            }
            else if (!throughDoor && 
                     !_layoutEngine.IsPathToAdjacentCellBlocked(_modelService.CurrentLevel, enemy.Location, moveLocation))
                enemy.Location = moveLocation;

            // Check for items
            var item = _modelService.CurrentLevel.GetAtPoint<ItemBase>(enemy.Location);
            if (item != null)
                StepOnItem(enemy, item);

            // Check for doodad
            var doodad = _modelService.CurrentLevel.GetAtPoint<DoodadBase>(enemy.Location);
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
                                                           x.Level.Contains(_modelService.CurrentLevel.Number);
                                              })
                                              .ToList();

            if (enemyTemplates.Count <= 0)
                return;

            // Create enemy from template
            var template = enemyTemplates[_randomSequenceGenerator.Get(0, enemyTemplates.Count)];
            var enemy = _characterGenerator.GenerateEnemy(template);
            
            // Map enemy location to level
            enemy.Location = _layoutEngine.GetRandomLocation(_modelService.CurrentLevel, true);

            // Add content to level
            _modelService.CurrentLevel.AddContent(enemy);
        }
        #endregion
    }
}

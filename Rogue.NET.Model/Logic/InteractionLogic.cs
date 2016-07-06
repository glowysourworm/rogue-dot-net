using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;
using System.Threading;
using System.Reflection;
using Rogue.NET.Common.Collections;
using Rogue.NET.Common;
using Rogue.NET.Model.Scenario;
using Rogue.NET.Model;
using Rogue.NET.Scenario.Model;
using Rogue.NET.Model.Generation;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Unity;
using Rogue.NET.Model.Events;
using Rogue.NET.Common.Events.Splash;

namespace Rogue.NET.Model.Logic
{
    public class InteractionLogic : LogicBase
    {
        public InteractionLogic(IEventAggregator eventAggregator, IUnityContainer unityContainer)
            : base(eventAggregator, unityContainer)
        {
        }

        public void ToggleDoor(Compass c, CellPoint characterLocation)
        {
            CellPoint p = Helper.GetPointInDirection(characterLocation, c);
            CellPoint pt = CellPoint.Empty;
            Compass cs = Compass.Null;
            if (Helper.IsCellThroughDoor(characterLocation, p, this.Level, out pt, out cs))
            {
                Cell c1 = this.Level.Grid.GetCell(characterLocation);
                Cell c2 = this.Level.Grid.GetCell(p);
                c1.ToggleDoor(c);
                c2.ToggleDoor(Helper.GetOppositeDirection(c));
            }
        }
        public void Attack(Compass c)
        {
            CellPoint p = Helper.GetPointInDirection(this.Player.Location, c);
            bool throughWall = Helper.IsPathToAdjacentCellBlocked(this.Player.Location, p, this.Level, this.Player, false, true);
            Enemy e = this.Level.GetEnemies().FirstOrDefault(z => z.Location.Equals(p));
            if (e != null && !throughWall)
            {
                //Engage enemy if they're attacked
                e.IsEngaged = true;

                double hit = Calculator.CalculatePlayerHit(this.Player, e, this.Random);
                if (hit <= 0 || this.Random.NextDouble() < e.Dodge)
                    PublishScenarioMessage(this.Player.RogueName + " Misses");

                else
                {
                    //Critical hit
                    if (this.Random.NextDouble() < e.BehaviorDetails.CurrentBehavior.CriticalRatio)
                    {
                        hit *= 2;
                        PublishScenarioMessage(this.Player.RogueName + " scores a critical hit!");
                    }
                    else
                        PublishScenarioMessage(this.Player.RogueName + " attacks");

                    e.Hp -= hit;

                    //Target enemy to process spells
                    //e.IsTargeted = true;

                    IEnumerable<Equipment> magicWeapons = this.Player.EquipmentInventory.Where(eq => eq.HasAttackSpell && eq.IsEquiped);
                    foreach (Equipment eq in magicWeapons)
                    {
                        //Set target so the enemy is processed with the spell
                        //e.IsTargeted = true;

                        ProcessPlayerMagicSpell(eq.AttackSpell);
                    }
                }

                if (this.Random.NextDouble() < e.BehaviorDetails.CurrentBehavior.CounterAttackProbability)
                {
                    this.Player.Hp -= Calculator.CalculateEnemyHit(this.Player, e,  this.Random);
                    PublishScenarioMessage(e.RogueName + " counter attacks");
                }
            }
        }
        public LevelContinuationAction Throw(string itemId)
        {
            Enemy e = this.TargetedEnemies.FirstOrDefault();
            LevelContinuationAction nextAction = LevelContinuationAction.DoNothing;
            if (e != null)
            {
                Consumable theItem = this.Player.ConsumableInventory.FirstOrDefault(z => z.Id == itemId);
                if (theItem != null)
                {
                    if (theItem.HasProjectileSpell)
                    {
                        //Get the spell and null the item
                        Spell s = theItem.ProjectileSpell;
                        this.Player.ConsumableInventory.Remove(theItem);

                        //Process the spell
                        //TODO - Change this to enemy invoke
                        nextAction = ProcessPlayerMagicSpell(s);
                    }

                    if (this.Encyclopedia[theItem.RogueName].IsObjective)
                        CheckObjective();
                }
                theItem = null;
            }
            return nextAction;
        }
        public LevelContinuationAction Consume(string id)
        {
            Consumable c = this.Player.ConsumableInventory.FirstOrDefault(z => z.Id == id);
            LevelContinuationAction nextAction = LevelContinuationAction.ProcessTurn;
            if (c != null)
            {
                LevelMessageEventArgs args = null;
                if (c.Type == ConsumableType.OneUse)
                {
                    if (Calculator.CalculatePlayerMeetsAlterationCost(c.Spell.Cost, this.Player, out args))
                        this.Player.ConsumableInventory.Remove(c);

                    else
                    {
                        PublishScenarioMessage(args.Message);
                        return nextAction;
                    }
                }
                else if (c.Type == ConsumableType.MultipleUses)
                {
                    if (Calculator.CalculatePlayerMeetsAlterationCost(c.Spell.Cost, this.Player, out args))
                        c.Uses--;

                    else
                    {
                        PublishScenarioMessage(args.Message);
                        return nextAction;
                    }

                    if (c.Uses <= 0)
                        this.Player.ConsumableInventory.Remove(c);
                }
                if (c.HasLearnedSkillSet)
                {
                    if (!this.Player.Skills.Any(z => z.RogueName == c.LearnedSkill.RogueName))
                    {
                        //Message plays on dungeon turn
                        this.Player.Skills.Add(c.LearnedSkill);
                        //OnPlayerAdvancementEvent(this, new PlayerAdvancementEventArgs(this.Player.Rogue2Name + " has been granted a new skill!", new string[] { c.LearnedSkill.Rogue2Name }));
                    }
                }
                if (c.HasSpell || c.SubType == ConsumableSubType.Ammo)
                {
                    Enemy targetedEnemy = this.TargetedEnemies.FirstOrDefault();
                    if (Calculator.CalculateSpellRequiresTarget(c.Spell) && targetedEnemy == null)
                    {
                        PublishScenarioMessage("Must first target an enemy");
                        return nextAction;
                    }
                    PublishScenarioMessage("Consuming " + (this.Encyclopedia[c.RogueName].IsIdentified ? c.RogueName : "???"));
                    return ProcessPlayerMagicSpell((c.SubType == ConsumableSubType.Ammo) ? c.AmmoSpell : c.Spell);
                }
            }
            return nextAction;
        }
        public void EnemyDeath(Enemy e)
        {
            Character c = (Character)e;
            for (int i = e.EquipmentInventory.Count - 1; i >= 0; i--)
            {
                Item item = e.EquipmentInventory[i];
                DropItem( c, item);
            }
            for (int i = e.ConsumableInventory.Count - 1; i >= 0; i--)
            {
                Item item = e.ConsumableInventory[i];
                DropItem(c, item);
            }

            //Update level object
            this.Level.RemoveEnemy(e);
            this.Level.MonsterScore += (int)e.ExperienceGiven;
            if (!this.Level.MonstersKilled.ContainsKey(e.RogueName))
                this.Level.MonstersKilled.Add(e.RogueName, 1);
            else
                this.Level.MonstersKilled[e.RogueName]++;


            this.Player.Experience += e.ExperienceGiven;

            //Skill Progress - Player gets boost on enemy death
            foreach (SkillSet s in this.Player.Skills)
            {
                if (s.Level >= this.Player.Skills.Count)
                    continue;

                switch (s.Emphasis)
                {
                    case 1:
                        s.SkillProgress += (0.001);
                        this.Player.Hunger += 0.1;
                        break;
                    case 2:
                        s.SkillProgress += (0.005);
                        this.Player.Hunger += 0.75;
                        break;
                    case 3:
                        s.SkillProgress += (0.01);
                        this.Player.Hunger += 1.5;
                        break;
                }
                if (s.SkillProgress >= 1)
                {
                    if (s.Level < this.Player.Skills.Count)
                    {
                        //Deactivate if currently turned on
                        if (s.IsTurnedOn && (s.CurrentSkill.Type == AlterationType.PassiveAura
                                         || s.CurrentSkill.Type == AlterationType.PassiveSource))
                        {
                            PublishScenarioMessage("Deactivating - " + s.RogueName);
                            this.Player.DeactivatePassiveEffect(s.CurrentSkill.Id);
                            s.IsTurnedOn = false;
                        }

                        s.Level++;
                        s.SkillProgress = 0;
                        //OnPlayerAdvancementEvent(this, new PlayerAdvancementEventArgs("Player Skill Has Reached a New Level!", new string[]{s.Rogue2Name + " has reached Level " + (s.Level + 1).ToString()}));
                    }
                    else
                        s.SkillProgress = 1;
                }
            }
            PublishScenarioMessage(e.RogueName + " Slayed");

            //Set enemy identified
            this.Encyclopedia[e.RogueName].IsIdentified = true;

            if (this.Encyclopedia[e.RogueName].IsObjective)
                CheckObjective();
        }
        public bool Equip(string equipId)
        {
            Equipment e = this.Player.EquipmentInventory.FirstOrDefault(z => z.Id == equipId);
            string msg = "";
            if (e != null)
            {
                if (e.IsEquiped)
                {
                    if (e.IsCursed)
                    {
                        this.Encyclopedia[e.RogueName].IsCurseIdentified = true;
                        msg = (this.Encyclopedia[e.RogueName].IsIdentified?e.RogueName:"???") + " is Cursed!!!";
                        PublishScenarioMessage(msg);
                        return false;
                    }
                    else
                    {
                        e.IsEquiped = false;
                        msg = "Un-Equipped " + (this.Encyclopedia[e.RogueName].IsIdentified ? e.RogueName : "???");
                        PublishScenarioMessage(msg);

                        if (e.HasEquipSpell)
                        {
                            if (e.EquipSpell.Type == AlterationType.PassiveSource)
                                this.Player.DeactivatePassiveEffect(e.EquipSpell.Id);

                            else
                                this.Player.DeactivatePassiveAura(e.EquipSpell.Id);
                        }
                        return true;
                    }
                }
                else
                {
                    switch (e.Type)
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
                                Equipment item = this.Player.EquipmentInventory.FirstOrDefault(z => z.Type == e.Type && z.IsEquiped);
                                if (item != null)
                                {
                                    msg = "Must first un-equip " + (this.Encyclopedia[item.RogueName].IsIdentified ? item.RogueName : "???");
                                    PublishScenarioMessage(msg);
                                    return false;
                                }
                            }
                            break;
                        case EquipmentType.TwoHandedMeleeWeapon:
                        case EquipmentType.OneHandedMeleeWeapon:
                        case EquipmentType.Shield:
                        case EquipmentType.RangeWeapon:
                            {
                                int handsFree = 2 - this.Player.EquipmentInventory.Count(z => (z.Type == EquipmentType.OneHandedMeleeWeapon || z.Type == EquipmentType.Shield) && z.IsEquiped);
                                handsFree -= 2 * this.Player.EquipmentInventory.Count(z => (z.Type == EquipmentType.TwoHandedMeleeWeapon || z.Type == EquipmentType.RangeWeapon) && z.IsEquiped);
                                if (((e.Type == EquipmentType.TwoHandedMeleeWeapon || e.Type == EquipmentType.RangeWeapon) && handsFree < 2) || handsFree < 1)
                                {
                                    msg = "Must first free up a hand";
                                    PublishScenarioMessage(msg);
                                    return false;
                                }
                            }
                            break;
                        case EquipmentType.Ring:
                            {
                                Equipment item = this.Player.EquipmentInventory.FirstOrDefault(z => z.Type == e.Type && z.IsEquiped);
                                if (item != null && this.Player.EquipmentInventory.Count(z => z.Type == EquipmentType.Ring && z.IsEquiped) >= 2)
                                {
                                    msg = "Must first un-equip " + (this.Encyclopedia[item.RogueName].IsIdentified ? item.RogueName : "???");
                                    PublishScenarioMessage(msg);
                                    return false;
                                }
                            }
                            break;
                        default:
                            break;
                    }
                    e.IsEquiped = true;
                    msg = "Equipped " + (this.Encyclopedia[e.RogueName].IsIdentified ? e.RogueName : "???");
                    PublishScenarioMessage(msg);

                    //Fire equip spell
                    if (e.HasEquipSpell)
                        ProcessPlayerMagicSpell(e.EquipSpell);

                    if (e.HasCurseSpell && e.IsCursed)
                        ProcessPlayerMagicSpell(e.CurseSpell);

                    return true;
                }
            }
            return false;
        }
        public void Identify(string id)
        {
            Item i = this.Player.ConsumableInventory.FirstOrDefault(z => z.Id == id);
            if (i == null)
                i = this.Player.EquipmentInventory.FirstOrDefault(z => z.Id == id);

            if (i == null)
                i = this.ShopConsumables.FirstOrDefault(z => z.Id == id);

            if (i == null)
                i = this.ShopEquipment.FirstOrDefault(z => z.Id == id);

            if (i != null)
            {
                this.Encyclopedia[i.RogueName].IsIdentified = true;
                this.Encyclopedia[i.RogueName].IsCurseIdentified = true;
                i.IsIdentified = true;
                PublishScenarioMessage(i.RogueName + " Identified");
            }
        }
        public void Enchant(string id)
        {
            Equipment equipment = this.Player.EquipmentInventory.FirstOrDefault(z => z.Id == id);
            if (equipment != null)
                equipment.Class++;

            PublishScenarioMessage("Your " + (this.Encyclopedia[equipment.RogueName].IsIdentified ? equipment.RogueName : "???") + " starts to glow!");
        }
        public void Uncurse(string id)
        {
            Equipment e = this.Player.EquipmentInventory.FirstOrDefault(z => z.Id == id);
            if (e != null)
            {
                e.IsCursed = false;
                PublishScenarioMessage((this.Encyclopedia[e.RogueName].IsIdentified?e.RogueName : "???") + " Uncursed");

                if (e.HasCurseSpell)
                {
                    if (e.EquipSpell.Type == AlterationType.PassiveSource)
                        this.Player.DeactivatePassiveEffect(e.CurseSpell.Id);

                    else
                        this.Player.DeactivatePassiveAura(e.CurseSpell.Id);

                    PublishScenarioMessage("Your " + e.RogueName + " is now safe to use");
                }
            }
        }
        public void Drop(string itemId)
        {
            Item i = this.Player.EquipmentInventory.FirstOrDefault(z => z.Id == itemId);
            if (i == null)
                i = this.Player.ConsumableInventory.FirstOrDefault(z => z.Id == itemId);

            if (i != null)
            {
                Character p = this.Player;
                DropItem(p, i);

                if (this.Encyclopedia[i.RogueName].IsObjective)
                    CheckObjective();
            }
        }
        public LevelContinuationAction Fire()
        {
            Equipment rangeWeapon = this.Player.EquipmentInventory.FirstOrDefault(z => z.Type == EquipmentType.RangeWeapon && z.IsEquiped);
            Enemy targetedEnemy = this.TargetedEnemies.FirstOrDefault();

            if (rangeWeapon == null)
                PublishScenarioMessage("No Range Weapons are equipped");

            else if (targetedEnemy == null)
                PublishScenarioMessage("Must first target an enemy");

            else if (Helper.RoguianDistance(this.Player.Location, targetedEnemy.Location) <= 2)
                PublishScenarioMessage("Too close to fire your weapon");

            else
            {
                //Should find a better way to identify ammo
                Consumable ammo = this.Player.ConsumableInventory.FirstOrDefault(z => z.RogueName == rangeWeapon.AmmoName);
                if (ammo == null)
                {
                    PublishScenarioMessage("No Ammunition for your weapon");
                    return LevelContinuationAction.ProcessTurn;
                }

                //Calculte hit
                double hit = Calculator.CalculatePlayerHit(this.Player, targetedEnemy,  this.Random);
                if (hit <= 0 || this.Random.NextDouble() < targetedEnemy.Dodge)
                    PublishScenarioMessage(this.Player.RogueName + " Misses");

                else
                {
                    if (this.Random.NextDouble() < this.Player.CriticalHitProbability)
                    {
                        PublishScenarioMessage("You fire your " + rangeWeapon.RogueName + " for a critical hit!");
                        targetedEnemy.Hp -= hit * 2;
                    }
                    else
                    {
                        PublishScenarioMessage("You hit the " + targetedEnemy.RogueName + " using your " + rangeWeapon.RogueName);
                        targetedEnemy.Hp -= hit;
                    }
                }

                //Run consumable as normal
                return Consume(ammo.Id);
            }
            return LevelContinuationAction.ProcessTurn;
        }
        public void StepOnItem(Character c, Item i)
        {
            if (c.HaulMax >= i.Weight + c.Haul)
            {
                if (i.GetType() == typeof(Consumable))
                {
                    c.ConsumableInventory.Add(i as Consumable);
                    this.Level.RemoveConsumable(i as Consumable);
                }
                else if (i.GetType() == typeof(Equipment))
                {
                    c.EquipmentInventory.Add(i as Equipment);
                    this.Level.RemoveEquipment(i as Equipment);
                }

                if (c is Player)
                {
                    string name = this.Encyclopedia[i.RogueName].IsIdentified ? i.RogueName : "???";
                    PublishScenarioMessage("Found " + name);

                    //Update level statistics
                    this.Level.ItemScore += i.ShopValue;
                    if (!this.Level.ItemsFound.ContainsKey(i.RogueName))
                        this.Level.ItemsFound.Add(i.RogueName, 1);
                    else
                        this.Level.ItemsFound[i.RogueName]++;

                    if (this.Encyclopedia[i.RogueName].IsObjective)
                        CheckObjective();
                }
            }
            else if (c is Player)
                PublishScenarioMessage("Too much weight in your inventory");
        }
        public LevelContinuationAction StepOnDoodad(Character c, Doodad d)
        {
            bool isPlayer = c is Player;

            if (isPlayer)
                d.IsHidden = false;

            LevelContinuationAction nextAction = LevelContinuationAction.ProcessTurn;
            ScenarioMetaData m = null;
            switch (d.Type)
            {
                case DoodadType.Normal:
                    {
                        DoodadNormal n = (DoodadNormal)d;
                        switch (n.NormalType)
                        {
                            case DoodadNormalType.SavePoint:
                                if (c is Player)
                                    PublishScenarioMessage("Save Point");
                                break;
                            case DoodadNormalType.Shop:
                                if (c is Player)
                                    PublishScenarioMessage("Mysterious Shop.....");
                                break;
                            case DoodadNormalType.StairsDown:
                                if (c is Player)
                                    PublishScenarioMessage("Stairs To Level " + (this.Level.Number + 1).ToString());
                                break;
                            case DoodadNormalType.StairsUp:
                                if (c is Player)
                                PublishScenarioMessage("Stairs To Level " + (this.Level.Number - 1).ToString());
                                break;
                            case DoodadNormalType.TeleportRandom:

                                //Identify
                                this.Encyclopedia[d.RogueName].IsIdentified = true;

                                CellPoint cp = Helper.GetRandomCellPoint(this.Level,  this.Random);
                                if (!Helper.IsCellOccupied(cp, this.Level, this.Player, true))
                                {
                                    this.Player.Location = cp;
                                    if (c is Player)
                                        PublishScenarioMessage("Teleport!");
                                    n.IsHidden = false;
                                }
                                break;
                            case DoodadNormalType.Teleport1:

                                //Identify
                                this.Encyclopedia[d.RogueName].IsIdentified = true;

                                DoodadNormal d2 = this.Level.GetNormalDoodads().FirstOrDefault(z => z.Id == n.PairId);
                                n.IsHidden = false;
                                d2.IsHidden = false;
                                if (d2 != null)
                                {
                                    if (!Helper.IsCellOccupied(d2.Location, this.Level, this.Player, true))
                                        c.Location = d2.Location;

                                    else
                                    {
                                        Enemy enemyToBoot = null;
                                        if (Helper.DoesCellContainEnemy(d2.Location, this.Level, out enemyToBoot))
                                        {
                                            this.Level.RemoveEnemy(enemyToBoot);
                                            c.Location = d2.Location;
                                        }
                                    }
                                }
                                break;
                            case DoodadNormalType.Teleport2:

                                //Identify
                                this.Encyclopedia[d.RogueName].IsIdentified = true;

                                DoodadNormal d1 = this.Level.GetNormalDoodads().FirstOrDefault(z => z.Id == n.PairId);
                                n.IsHidden = false;
                                d1.IsHidden = false;
                                if (d1 != null)
                                {
                                    if (!Helper.IsCellOccupied(d1.Location, this.Level, this.Player, true))
                                        c.Location = d1.Location;

                                    else
                                    {
                                        Enemy enemyToBoot = null;
                                        if (Helper.DoesCellContainEnemy(d1.Location, this.Level, out enemyToBoot))
                                        {
                                            this.Level.RemoveEnemy(enemyToBoot);
                                            c.Location = d1.Location;
                                        }
                                    }
                                }
                                break;
                        }
                    }
                    break;
                case DoodadType.Magic:
                    {
                        if (c is Player)
                        {
                            DoodadMagic dm = (DoodadMagic)d;
                            m = this.Encyclopedia[dm.RogueName];

                            if (!(dm.IsOneUse && dm.HasBeenUsed))
                            {
                                if (dm.IsAutomatic)
                                {
                                    nextAction = ProcessPlayerMagicSpell(dm.AutomaticSpell);
                                    dm.HasBeenUsed = true;
                                }
                            }
                        }
                    }
                    break;
            }
            return nextAction;
        }
        public void Target(Compass c)
        {
            Enemy targeted = this.TargetedEnemies.FirstOrDefault();
            List<Enemy> inRange = this.Level.GetEnemies().Where(z => this.Level.Grid.GetVisibleCells().Any(y => y.Location.Equals(z.Location))).ToList();
            if (inRange.Count <= 0)
                return;

            if (targeted != null)
            {
                int idx = inRange.IndexOf(targeted);
                this.TargetedEnemies.Remove(targeted);
                switch (c)
                {
                    case Compass.E:
                        {
                            if (idx + 1 == inRange.Count)
                                this.TargetedEnemies.Add(inRange[0]);
                            else
                                this.TargetedEnemies.Add(inRange[idx + 1]);
                        }
                        break;
                    case Compass.W:
                        {
                            if (idx - 1 == -1)
                                this.TargetedEnemies.Add(inRange[inRange.Count - 1]);
                            else
                                this.TargetedEnemies.Add(inRange[idx - 1]);
                        }
                        break;
                    default:
                        this.TargetedEnemies.Add(inRange[0]);
                        break;
                }
            }
            else
            {
                if (inRange.Count > 0)
                    this.TargetedEnemies.Add(inRange[0]);
            }
            PublishTargetEvent(TargetedEnemies[0]);
        }
        private void ProcessEnemyReactions(MovementLogic movementEngine)
        {
            for (int i = 0; i < this.Level.GetEnemies().Length; i++)
            {
                Enemy e = this.Level.GetEnemies()[i];
                double dist = Helper.EuclidianDistance(e.Location, this.Player.Location);

                //Check for engaged
                if (dist < e.BehaviorDetails.CurrentBehavior.EngageRadius)
                    e.IsEngaged = true;
                if (dist > e.BehaviorDetails.CurrentBehavior.DisengageRadius)
                    e.IsEngaged = false;

                if (!e.IsEngaged)
                    continue;

                double turn = Calculator.CalculateEnemyTurn(e, this.Player);
                e.TurnCounter += turn;

                if (e.TurnCounter >= 1)
                {
                    //Checks/sets random switch between behaviors
                    e.BehaviorDetails.RollRandomBehavior(this.Random);

                    int turns = (int)e.TurnCounter;
                    e.TurnCounter = e.TurnCounter % 1;

                    for (int j = 0; j < turns; j++)
                    {
                        //Check altered states
                        if (e.States.Any(z => z != CharacterStateType.Normal))
                        {
                            //Sleeping
                            if (e.States.Any(z => z == CharacterStateType.Sleeping))
                                continue;

                            //Paralyzed
                            else if (e.States.Any(z => z == CharacterStateType.Paralyzed))
                                continue;

                            //Confused - check during calculate character move
                        }                        

                        //Check attack
                        List<Cell> adjCells = Helper.GetAdjacentCells(e.Location.Row, e.Location.Column, this.Level.Grid);
                        Cell playerCell = adjCells.FirstOrDefault(z => z.Location.Equals(this.Player.Location));
                        if (e.BehaviorDetails.CurrentBehavior.AttackType == CharacterAttackType.Melee && playerCell != null && !e.States.Any(z => z == CharacterStateType.Confused))
                        {
                            if (!Helper.IsPathToAdjacentCellBlocked(e.Location, playerCell.Location, this.Level, this.Player, false, true))
                            {
                                EnemyMeleeAttack(e);
                                e.BehaviorDetails.SetPrimaryInvoked();
                                continue;
                            }
                        }
                        else if (e.BehaviorDetails.CurrentBehavior.AttackType == CharacterAttackType.Skill 
                            && dist < e.BehaviorDetails.CurrentBehavior.EngageRadius 
                            && this.Level.Grid.GetVisibleCells().Any(z => z.Location.Equals(e.Location))
                            && !e.States.Any(z => z == CharacterStateType.Confused))
                        {
                            ProcessEnemyInvokeSkill(e);
                            e.BehaviorDetails.SetPrimaryInvoked();
                            continue;
                        }

                        //Process Move
                        Cell ce = movementEngine.CalculateCharacterMove(e.States, e.BehaviorDetails.CurrentBehavior.MovementType, e.BehaviorDetails.CurrentBehavior.DisengageRadius, e.Location, this.Player.Location);

                        //Check for opening of doors
                        if (ce != null)
                        {
                            CellPoint openingPosition = CellPoint.Empty;
                            Compass openingDirection = Compass.Null;
                            bool throughDoor = Helper.IsCellThroughDoor(e.Location, ce.Location, this.Level, out openingPosition, out openingDirection);
                            if (e.BehaviorDetails.CurrentBehavior.CanOpenDoors && throughDoor)
                            {
                                Compass desiredDirection = Helper.GetDirectionBetweenAdjacentPoints(e.Location, ce.Location);

                                //If not in a cardinally adjacent position then move into that position before opening.
                                if (e.Location.Equals(openingPosition))
                                    ToggleDoor(openingDirection, e.Location);
                                else
                                {
                                    if (!Helper.IsPathToAdjacentCellBlocked(e.Location, openingPosition, this.Level, this.Player, true, false))
                                        e.Location = openingPosition;
                                }
                            }
                            else if (!throughDoor && !Helper.IsCellOccupied(ce.Location, this.Level, this.Player, true) && !Helper.IsCellThroughWall(e.Location, ce.Location, this.Level))
                                e.Location = ce.Location;

                            Doodad dd = null;
                            Item it = null;
                            if (Helper.DoesCellContainDoodad(e.Location, this.Level, out dd))
                                StepOnDoodad(e, dd);
                            if (Helper.DoesCellContainItem(e.Location, this.Level, out it))
                                StepOnItem(e, it);
                        }
                    }
                }
            }
        }
        public void ProcessTurn(MovementLogic movementEngine, Cell[] exploredCells, bool regenerate, bool enemyReactions)
        {
            //Collect auras that affect player
            List<AlterationEffect> passiveEnemyAuraEffects = new List<AlterationEffect>();

            //Calculate first before reactions so enemies don't get last word
            for (int i = this.Level.GetEnemies().Length - 1; i >= 0; i--)
            {
                Enemy enemy = this.Level.GetEnemies()[i];
                if (this.Level.GetEnemies()[i].Hp <= 0)
                {
                    EnemyDeath(this.Level.GetEnemies()[i]);
                }

                else
                {
                    //distance between enemy and player
                    double dist = Helper.RoguianDistance(this.Player.Location, enemy.Location);

                    //Add enemy auras to list where player is within aura effect range
                    passiveEnemyAuraEffects.AddRange(enemy.GetActiveAuras().Where(z => z.EffectRange >= dist));

                    //Process tick - pass in active player auras where the enemy is within effect radius
                    PlayerAdvancementEventArgs enemyAdvancementEventArgs = null;
                    LevelMessageEventArgs[] msgArgs = enemy.OnDungeonTick(this.Random, this.Player.GetActiveAuras().Where(z => z.EffectRange >= dist), true, out enemyAdvancementEventArgs);
                    foreach (LevelMessageEventArgs a in msgArgs)
                        PublishScenarioMessage(a.Message);
                }
            }
            
            if (enemyReactions)
                ProcessEnemyReactions(movementEngine);

            ProcessMonsterGeneration();

            //Maintain aura effects
            foreach (SkillSet s in this.Player.Skills)
            {
                //Maintain aura effects
                //if (s.IsTurnedOn && s.CurrentSkill.AuraConsumesMp && s.CurrentSkill.IsAura() && this.Player.Mp <= 0)
                LevelMessageEventArgs args = null;
                if (s.IsTurnedOn && !Calculator.CalculatePlayerMeetsAlterationCost(s.CurrentSkill.Cost, this.Player, out args))
                {
                    s.IsTurnedOn = false;
                    PublishScenarioMessage(args.Message);
                    PublishScenarioMessage("Deactivating " + s.RogueName);

                    if (s.CurrentSkill.Type == AlterationType.PassiveAura)
                        this.Player.DeactivatePassiveAura(s.CurrentSkill.Id);

                    else
                        this.Player.DeactivatePassiveEffect(s.CurrentSkill.Id);
                }
            }

            //Calculate base added values for player - includes skills
            PlayerAdvancementEventArgs playerAdvancementEventArgs = null;
            LevelMessageEventArgs[] msgArgss = this.Player.OnDungeonTick(this.Random, passiveEnemyAuraEffects, regenerate, out playerAdvancementEventArgs);
            foreach (LevelMessageEventArgs arg in msgArgss)
                PublishScenarioMessage(arg.Message);

            //if (playerAdvancementEventArgs != null)
            //    OnPlayerAdvancementEvent(this, playerAdvancementEventArgs);

            //Identify new skills
            foreach (SkillSet s in this.Player.Skills)
            {
                //Check for new learned skills
                if (this.Player.Level >= s.LevelLearned && !s.IsLearned)
                {
                    this.Encyclopedia[s.RogueName].IsIdentified = true;
                    s.IsLearned = true;
                    //OnPlayerAdvancementEvent(this, new PlayerAdvancementEventArgs(this.Player.Rogue2Name + " Has Learned A New Skill!", new string[]{s.Rogue2Name}));
                }
            }

            //End Targeting
            this.TargetedEnemies.Clear();
            PublishTargetEvent(null);

            //Check for shop
            Doodad shop = null;
            this.Level.IsPlayerOnShop = (Helper.DoesCellContainDoodad(this.Player.Location, this.Level, out shop) && 
                shop.Type == DoodadType.Normal && 
                ((DoodadNormal)shop).NormalType == DoodadNormalType.Shop);

            //I'm Not DEEEAD!
            if (this.Player.Hunger >= 100 || this.Player.Hp <= 0.1)
                PublishPlayerDiedEvent("Had a rough day");

            //Add a tick
            PublishScenarioTickEvent();
        }
        public void ToggleActiveSkill(string id, bool activate)
        {
            SkillSet set = this.Player.Skills.FirstOrDefault(z => z.Id == id);
            bool isActive = set.IsActive;

            if (isActive && activate)
                return;

            if (!isActive && !activate)
                return;

            //Good measure - set non active for all skill sets
            foreach (SkillSet skillSet in this.Player.Skills)
                skillSet.IsActive = false;

            //Shut off aura effects
            foreach (SkillSet s in this.Player.Skills)
            {
                //Maintain aura effects
                if (s.IsTurnedOn)
                {
                    s.IsTurnedOn = false;
                    PublishScenarioMessage("Deactivating " + s.RogueName);

                    if (s.CurrentSkill.Type == AlterationType.PassiveAura)
                        this.Player.DeactivatePassiveAura(s.CurrentSkill.Id);

                    else
                        this.Player.DeactivatePassiveEffect(s.CurrentSkill.Id);
                }
            }

            if (set != null)
                set.IsActive = !isActive;
        }
        public void CycleActiveSkill()
        {
            List<SkillSet> learnedSkills = this.Player.Skills.Where(z => z.IsLearned).ToList();
            SkillSet activeSet = learnedSkills.FirstOrDefault(z => z.IsActive);

            if (activeSet != null)
            {
                ToggleActiveSkill(activeSet.Id, false);

                int idx = learnedSkills.IndexOf(activeSet);
                if (idx == learnedSkills.Count() - 1)
                    ToggleActiveSkill(learnedSkills[0].Id, true);

                else
                    ToggleActiveSkill(learnedSkills[idx + 1].Id, true);
            }
            else if (learnedSkills.Count > 0)
                ToggleActiveSkill(learnedSkills.First().Id, true);
               
        }
        public void EmphasizeSkillUp(string id)
        {
            SkillSet set = this.Player.Skills.FirstOrDefault(z => z.Id == id);
            if (set != null)
            {
                if (set.Emphasis < 3)
                    set.Emphasis++;
            }
        }
        public void EmphasizeSkillDown(string id)
        {
            SkillSet set = this.Player.Skills.FirstOrDefault(z => z.Id == id);
            if (set != null)
            {
                if (set.Emphasis > 0)
                    set.Emphasis--;
            }
        }
        public void ToggleMarkForTrade(string id)
        {
            Item item = this.Player.ConsumableInventory.FirstOrDefault(i => i.Id == id);
            if (item != null)
            {
                item.IsMarkedForTrade = !item.IsMarkedForTrade;
                return;
            }

            item = this.Player.EquipmentInventory.FirstOrDefault(i => i.Id == id);
            if (item != null)
            {
                item.IsMarkedForTrade = !item.IsMarkedForTrade;
                return;
            }

            item = this.ShopConsumables.FirstOrDefault(i => i.Id == id);
            if (item != null)
            {
                item.IsMarkedForTrade = !item.IsMarkedForTrade;
                return;
            }

            item = this.ShopEquipment.FirstOrDefault(i => i.Id == id);
            if (item != null)
                item.IsMarkedForTrade = !item.IsMarkedForTrade;
        }
        public void ClearMarkedForTrade()
        {
            foreach (Item item in this.Player.ConsumableInventory)
                item.IsMarkedForTrade = false;

            foreach (Item item in this.Player.EquipmentInventory)
                item.IsMarkedForTrade = false;

            foreach (Item item in this.ShopConsumables)
                item.IsMarkedForTrade = false;

            foreach (Item item in this.ShopEquipment)
                item.IsMarkedForTrade = false;
        }
        public void Trade()
        {
            for (int i=this.ShopConsumables.Count - 1;i>=0;i--)
            {
                Consumable item = this.ShopConsumables[i];
                if (item.IsMarkedForTrade)
                {
                    //Have to reset flag so it doesn't get traded back
                    item.IsMarkedForTrade = false;
                    this.Player.ConsumableInventory.Add(item as Consumable);

                    this.ShopConsumables.RemoveAt(i);
                    PublishScenarioMessage(this.Player.RogueName + " received a " +
                        (this.Encyclopedia[item.RogueName].IsIdentified ? item.RogueName : "???"));
                }
            }
            for (int i = this.ShopEquipment.Count - 1; i >= 0; i--)
            {
                Item item = this.ShopEquipment[i];
                if (item.IsMarkedForTrade)
                {
                    //Have to reset flag so it doesn't get traded back
                    item.IsMarkedForTrade = false;
                    this.Player.EquipmentInventory.Add(item as Equipment);

                    this.ShopEquipment.RemoveAt(i);
                    PublishScenarioMessage(this.Player.RogueName + " received a " +
                        (this.Encyclopedia[item.RogueName].IsIdentified ? item.RogueName : "???"));
                }
            }
            for (int i = this.Player.ConsumableInventory.Count - 1; i >= 0; i--)
            {
                Consumable item = this.Player.ConsumableInventory[i];
                if (item.IsMarkedForTrade)
                {
                    item.IsMarkedForTrade = false;
                    this.ShopConsumables.Add(item);
                    this.Player.ConsumableInventory.RemoveAt(i);
                }
            }
            for (int i = this.Player.EquipmentInventory.Count - 1; i >= 0; i--)
            {
                Equipment item = this.Player.EquipmentInventory[i];
                if (item.IsMarkedForTrade)
                {
                    item.IsMarkedForTrade = false;
                    this.ShopEquipment.Add(item);
                    this.Player.EquipmentInventory.RemoveAt(i);
                }
            }
        }
        public void DebugSkillUp()
        {
            foreach (SkillSet set in this.Player.Skills)
            {
                set.Level++;
            }
        }

        private void CheckObjective()
        {
            bool acheived = true;
            foreach (ScenarioMetaData objectiveMeta in this.Encyclopedia.Values.Where(z => z.IsObjective))
            {
                switch (objectiveMeta.ObjectType)
                {
                    case DungeonMetaDataObjectTypes.Skill:
                        acheived &= this.Player.Skills.Any(skill => skill.RogueName == objectiveMeta.RogueName);
                        break;
                    case DungeonMetaDataObjectTypes.Item:
                        acheived &= (this.Player.ConsumableInventory.Any(item => item.RogueName == objectiveMeta.RogueName)
                                  || this.Player.EquipmentInventory.Any(item => item.RogueName == objectiveMeta.RogueName));
                        break;
                    case DungeonMetaDataObjectTypes.Enemy:
                        acheived &= objectiveMeta.IsIdentified;
                        break;
                    case DungeonMetaDataObjectTypes.Doodad:
                        acheived &= objectiveMeta.IsIdentified;
                        break;
                }
            }

            //if (acheived)
            //    OnSpecialDungeonEvent(this, new SpecialDungeonEventArgs(SpecialDungeonEvents.ObjectiveTrue));
            //else
            //    OnSpecialDungeonEvent(this, new SpecialDungeonEventArgs(SpecialDungeonEvents.ObjectiveFalse));
        }

        /// <summary>
        /// Returns true if a turn should be processed
        /// </summary>
        public LevelContinuationAction InvokePlayerSkill()
        {
            SkillSet set = this.Player.Skills.FirstOrDefault(z => z.IsActive == true);
            if (set == null)
            {
                PublishScenarioMessage("No Active Skill");
                return LevelContinuationAction.ProcessTurn;
            }

            Enemy targeted = this.TargetedEnemies.FirstOrDefault();
            if (Calculator.CalculateSpellRequiresTarget(set.CurrentSkill) && targeted == null)
            {
                PublishScenarioMessage(set.RogueName + " requires a targeted enemy");
                return LevelContinuationAction.ProcessTurn;
            }

            LevelMessageEventArgs args = null;
            if (!Calculator.CalculatePlayerMeetsAlterationCost(set.CurrentSkill.Cost, this.Player, out args))
            {
                if (args != null)
                    PublishScenarioMessage(args.Message);
                return LevelContinuationAction.ProcessTurn;
            }

            //Deactivate passive if it's turned on
            if (set.CurrentSkill.Type == AlterationType.PassiveAura
                || set.CurrentSkill.Type == AlterationType.PassiveSource)
            {
                if (set.IsTurnedOn)
                {
                    PublishScenarioMessage("Deactivating - " + set.RogueName);
                    this.Player.DeactivatePassiveEffect(set.CurrentSkill.Id);
                    set.IsTurnedOn = false;
                    return LevelContinuationAction.ProcessTurn;
                }
                else
                    set.IsTurnedOn = true;
            }


            PublishScenarioMessage("Invoking - " + set.RogueName);
            return ProcessPlayerMagicSpell(set.CurrentSkill);
        }
        public LevelContinuationAction InvokeDoodad()
        {
            Doodad d = null;
            Helper.DoesCellContainDoodad(this.Player.Location, this.Level, out d);
            if (d == null)
                return LevelContinuationAction.ProcessTurn;

            switch (d.Type)
            {
                case DoodadType.Magic:
                    {
                        //Identify
                        this.Encyclopedia[d.RogueName].IsIdentified = true;
                        if (this.Encyclopedia[d.RogueName].IsObjective)
                            CheckObjective();

                        if (d.IsOneUse && d.HasBeenUsed || !((DoodadMagic)d).IsInvoked)
                            PublishScenarioMessage("Nothing Happens");
                        else
                        {
                            PublishScenarioMessage("Using " + d.RogueName);
                            //--------
                            DoodadMagic m = (DoodadMagic)d;
                            m.HasBeenUsed = true;
                            if (m.IsInvoked)
                                return ProcessPlayerMagicSpell(m.InvokedSpell);
                        }
                    }
                    break;
                case DoodadType.Normal:
                    {
                        DoodadNormal n = (DoodadNormal)d;
                        
                        //Sets identified
                        this.Encyclopedia[n.RogueName].IsIdentified = true;
                        if (this.Encyclopedia[d.RogueName].IsObjective)
                            CheckObjective();

                        switch (n.NormalType)
                        {
                            case DoodadNormalType.SavePoint:
                                PublishSaveEvent();
                                break;
                            case DoodadNormalType.Shop:
                                //OnSpecialDungeonEvent(this, new SpecialDungeonEventArgs(SpecialDungeonEvents.ShopEnter));
                                break;
                            case DoodadNormalType.StairsDown:
                                PublishLoadLevelRequest(this.Level.Number + 1, PlayerStartLocation.StairsUp);
                                break;
                            case DoodadNormalType.StairsUp:
                                PublishLoadLevelRequest(this.Level.Number - 1, PlayerStartLocation.StairsDown);
                                break;
                        }
                    }
                    break;
            }

            return LevelContinuationAction.ProcessTurn;
        }
        private void ProcessMonsterGeneration()
        {
            if (this.Level.Type == LayoutType.Shop)
                return;

            if (this.ScenarioConfig.DungeonTemplate.MonsterGenerationBase > this.Random.NextDouble() && this.ScenarioConfig.EnemyTemplates.Count > 0)
            {
                List<EnemyTemplate> list = new List<EnemyTemplate>();
                foreach (EnemyTemplate temp in this.ScenarioConfig.EnemyTemplates)
                {
                    if ((temp.IsUnique && temp.HasBeenGenerated) || temp.IsObjectiveItem)
                        continue;

                    if (temp.Level.Contains(this.Level.Number))
                        list.Add(temp);
                }
                if (list.Count == 0)
                    return;

                EnemyTemplate t = list[this.Random.Next(0, list.Count)];
                Enemy e = TemplateGenerator.GenerateEnemy(t, this.Random);

                if (e == null)
                    return;

                Cell c = this.Level.Grid.GetRandomCell(this.Random);
                if (!c.IsPhysicallyVisible && !Helper.IsCellOccupied(c.Location, this.Level, this.Player, false))
                {
                    // turn off animation for adding a monster
                    e.Location = c.Location;
                    this.Level.AddEnemy(e);
                }
            }
        }

        private void DropItem(Character c, Item i)
        {
            List<Cell> adj = Helper.GetAdjacentCells(c.Location.Row, c.Location.Column, this.Level.Grid);
            foreach (Cell ce in adj)
            {
                if (!Helper.IsPathToAdjacentCellBlocked(this.Player.Location, ce.Location, this.Level, this.Player, false, false))
                {
                    if (i is Equipment)
                    {
                        Equipment e = (Equipment)i;
                        if (e.IsEquiped)
                        {
                            if (!Equip(e.Id))
                                return;
                        }

                        e.Location = ce.Location;
                        c.EquipmentInventory.Remove(e);

                        //Deactivate passives
                        if (e.HasEquipSpell)
                        {
                            if (e.EquipSpell.Type == AlterationType.PassiveSource)
                                this.Player.DeactivatePassiveEffect(e.EquipSpell.Id);

                            else
                                this.Player.DeactivatePassiveAura(e.EquipSpell.Id);
                        }

                        this.Level.AddEquipment(e);
                        if (c is Player)
                            PublishScenarioMessage((this.Encyclopedia[e.RogueName].IsIdentified ? e.RogueName : "???") + " Dropped");
                    }
                    if (i is Consumable)
                    {
                        Consumable co = (Consumable)i;
                        co.Location = ce.Location;
                        c.ConsumableInventory.Remove(co);
                        this.Level.AddConsumable(co);
                        if (c is Player)
                            PublishScenarioMessage(this.Encyclopedia[i.RogueName].IsIdentified ? i.RogueName : "???" + " Dropped");
                    }
                    break;
                }
            }
        }
        private void EnemyMeleeAttack(Enemy e)
        {
            bool throughWall = Helper.IsPathToAdjacentCellBlocked(e.Location, this.Player.Location, this.Level, this.Player, false, true);
            if (!throughWall)
            {
                double hit = Calculator.CalculateEnemyHit(this.Player, e,  this.Random);

                if (hit > 0 && (this.Random.NextDouble() > this.Player.Dodge))
                {

                    if (this.Random.NextDouble() < e.BehaviorDetails.CurrentBehavior.CriticalRatio)
                    {
                        hit *= 2;
                        PublishScenarioMessage(e.RogueName + " attacks for a critical hit!");
                    }
                    else
                        PublishScenarioMessage(e.RogueName + " attacks");

                    this.Player.Hp -= hit;
                    if (this.Player.Hp <= 0)
                        this.Player.Hp = 0;
                }
                else
                    PublishScenarioMessage(e.RogueName + " Misses");
            }
        }
        private void Teleport(Character c)
        {
            CellPoint p = Helper.GetRandomCellPoint(this.Level,  this.Random);
            if (!Helper.IsCellOccupied(p, this.Level, this.Player, false))
            {
                c.Location = p;
                PublishScenarioMessage("You were teleported!");
            }
        }
        private void RevealSavePoint()
        {
            if (this.Level.HasSavePoint)
            {
                this.Level.GetSavePoint().IsRevealed = true;
            }
            PublishScenarioMessage("You sense disaster recovery centers near by...");
        }
        private void RevealMonsters()
        {
            foreach (Enemy e in this.Level.GetEnemies())
                e.IsRevealed = true;

            PublishScenarioMessage("You hear growling in the distance...");
        }
        private void RevealLevel()
        {
            foreach (Cell c in this.Level.Grid.GetCellsAsArray())
                c.IsRevealed = true;

            RevealSavePoint();
            RevealStairs();
            RevealMonsters();
            RevealContent();
            PublishScenarioMessage("Your spatial senses are heightened");

            //Need to send an update for the affected cells
            //OnDungeonTickEvent(this, new DungeonTickEventArgs(this.Level, this.Player, this.Encyclopedia, this.ShopConsumables, this.ShopEquipment, this.Level.Grid.GetCellsAsArray(), ""));
        }
        private void RevealStairs()
        {
            if (this.Level.HasStairsDown)
                this.Level.GetStairsDown().IsRevealed = true;

            if (this.Level.HasStairsUp)
                this.Level.GetStairsUp().IsRevealed = true;

            PublishScenarioMessage("You sense exits nearby");
        }
        private void RevealItems()
        {
            foreach (Item i in this.Level.GetConsumables())
                i.IsRevealed = true;

            foreach (Item i in this.Level.GetEquipment())
                i.IsRevealed = true;

            PublishScenarioMessage("You sense objects nearby");
        }
        private void RevealContent()
        {
            foreach (DoodadNormal n in this.Level.GetNormalDoodads())
            {
                n.IsRevealed = true;
                n.IsHidden = false;
            }

            foreach (DoodadMagic m in this.Level.GetMagicDoodads())
            {
                m.IsRevealed = true;
                m.IsHidden = false;
            }
            foreach (Item i in this.Level.GetConsumables())
                i.IsRevealed = true;

            foreach (Item i in this.Level.GetEquipment())
                i.IsRevealed = true;

            PublishScenarioMessage("You sense objects nearby");
        }
        private void RevealFood()
        {
            foreach (Consumable c in this.Level.GetConsumables().Where(z => z.SubType == ConsumableSubType.Food))
                c.IsPhysicallyVisible = true;

            PublishScenarioMessage("Hunger makes a good sauce.....  :)");
        }
        private void CreateMonster(CellPoint p, string monsterName)
        {
            EnemyTemplate template = this.ScenarioConfig.EnemyTemplates.FirstOrDefault(z => z.Name == monsterName);
            if (template != null)
            {
                PublishScenarioMessage("You hear growling in the distance");
                Enemy e = TemplateGenerator.GenerateEnemy( template,  this.Random);
                e.Location = p;
                this.Level.AddEnemy(e);
            }
        }

        //Magic
        private LevelContinuationAction ProcessPlayerMagicSpell(Spell spell)
        {
            LevelMessageEventArgs ea = null;

            //Cost will be applied on turn
            if (!Calculator.CalculatePlayerMeetsAlterationCost(spell.Cost, this.Player, out ea))
            {
                if (ea != null)
                    PublishScenarioMessage(ea.Message);

                return LevelContinuationAction.ProcessTurn;
            }

            //Calculate alteration from spell's random parameters
            var alteration = TemplateGenerator.GenerateAlteration(spell, this.Random);

            //If no targets available & spell requires a target, log and return
            if (Calculator.CalculateSpellRequiresTarget(spell) && this.TargetedEnemies.Count == 0)
            {
                PublishScenarioMessage("You must target an enemy");
                return LevelContinuationAction.ProcessTurn;
            }

            //Apply alteration cost
            this.Player.ApplyAlterationCost(alteration.Cost);

            //Run animations before applying effects
            if (spell.Animations.Count > 0)
            {
                PublishAnimationEvent(
                    AnimationReturnAction.ProcessPlayerSpell,
                    spell.Animations,
                    alteration,
                    this.Player,
                    this.TargetedEnemies.Cast<Character>().ToList());
                return LevelContinuationAction.DoNothing;
            }
            //No animations - just apply effects
            else
            {
                ProcessPlayerMagicSpellAfterAnimation(new AnimationCompletedEvent()
                {
                     Alteration = alteration,
                     ReturnAction = AnimationReturnAction.ProcessPlayerSpell,
                     Source = this.Player,
                     Targets = this.TargetedEnemies.Cast<Character>().ToList()
                });
                return LevelContinuationAction.ProcessTurn;
            }
        }
        //Used for item throw effects
        private LevelContinuationAction ProcessEnemyInvokeSpellEffects(Enemy enemy, Spell spell)
        {
            //Calculate alteration from spell's random parameters
            Alteration alteration = TemplateGenerator.GenerateAlteration(spell,  this.Random);

            //Run animations before applying effects
            if (spell.Animations.Count > 0)
            {
                PublishAnimationEvent(
                    AnimationReturnAction.ProcessEnemySpell,
                    spell.Animations,
                    alteration,
                    enemy,
                    new List<Character>(new Character[] { this.Player }));

                return LevelContinuationAction.ProcessTurnAfterAnimation;
            }
            //No animations - just apply effects
            else
            {
                ProcessEnemyMagicSpellAfterAnimation(new AnimationCompletedEvent()
                {
                    Alteration = alteration,
                    ReturnAction = AnimationReturnAction.ProcessEnemySpell,
                    Source = enemy,
                    Targets = new List<Character>(new Character[] { this.Player })
                });
                return LevelContinuationAction.ProcessTurn;
            }
        }
        private void ProcessEnemyInvokeSkill(Enemy enemy)
        {
            LevelMessageEventArgs ea = null;
            if (!Calculator.CalculateEnemyMeetsAlterationCost(
                enemy.BehaviorDetails.CurrentBehavior.EnemySkill.Cost, 
                enemy, 
                out ea))
                return;

            Alteration alteration = TemplateGenerator.GenerateAlteration(
                enemy.BehaviorDetails.CurrentBehavior.EnemySkill,
                 this.Random);

            //Apply alteration cost
            enemy.ApplyAlterationCost(alteration.Cost);

            if (enemy.BehaviorDetails.CurrentBehavior.EnemySkill.Animations.Count > 0)
            {
                PublishAnimationEvent(
                    AnimationReturnAction.ProcessEnemySpell,
                    enemy.BehaviorDetails.CurrentBehavior.EnemySkill.Animations,
                    alteration,
                    enemy,
                    new List<Character>(new Character[] { this.Player }));
            }
            else
            {
                ProcessEnemyMagicSpellAfterAnimation(new AnimationCompletedEvent()
                {
                    Alteration = alteration,
                    ReturnAction = AnimationReturnAction.ProcessEnemySpell,
                    Source = enemy,
                    Targets = new List<Character>(new Character[]{this.Player})
                });
            }
        }

        public void ProcessPlayerMagicSpellAfterAnimation(AnimationCompletedEvent e)
        {
            // TODO: Apply stackable
            switch (e.Alteration.Type)
            {
                case AlterationType.PassiveAura:
                    e.Source.ApplyAuraEffect(e.Alteration.Effect, e.Alteration.AuraEffect);
                    break;
                case AlterationType.TemporarySource:
                    e.Source.ApplyTemporaryEffect(e.Alteration.Effect);
                    break;
                case AlterationType.PassiveSource:
                    e.Source.ApplyPassiveEffect(e.Alteration.Effect);
                    break;
                case AlterationType.PermanentSource:
                    {
                        LevelMessageEventArgs[] msgs = e.Source.ApplyPermanentEffect(e.Alteration.Effect);
                        foreach (LevelMessageEventArgs arg in msgs)
                            PublishScenarioMessage(arg.Message);
                    }
                    break;
                case AlterationType.TemporaryTarget:
                    {
                        foreach (Character c in e.Targets)
                        {
                            bool blocked = Calculator.CalculateSpellBlock(c, e.Alteration.BlockType == AlterationBlockType.Physical, this.Random);
                            if (blocked)
                            {
                                PublishScenarioMessage(c.RogueName + " blocked the spell!");
                                continue;
                            }

                            c.ApplyTemporaryEffect(e.Alteration.Effect);
                        }
                    }
                    break;
                case AlterationType.PermanentTarget:
                    {
                        foreach (Character c in e.Targets)
                        {
                            bool blocked = Calculator.CalculateSpellBlock(c, e.Alteration.BlockType == AlterationBlockType.Physical, this.Random);
                            if (blocked)
                            {
                                PublishScenarioMessage(c.RogueName + " blocked the spell!");
                                continue;
                            }
                            c.ApplyPermanentEffect(e.Alteration.Effect);
                        }
                    }
                    break;
                case AlterationType.TemporaryAllTargets:
                    {
                        foreach (Character c in e.Targets)
                        {
                            bool blocked = Calculator.CalculateSpellBlock(c, e.Alteration.BlockType == AlterationBlockType.Physical,  this.Random);
                            if (blocked)
                            {
                                PublishScenarioMessage(c.RogueName + " blocked the spell!");
                                continue;
                            }
                            c.ApplyTemporaryEffect(e.Alteration.Effect);
                        }
                    }
                    break;
                case AlterationType.PermanentAllTargets:
                    {
                        foreach (Character c in e.Targets)
                        {
                            bool blocked = Calculator.CalculateSpellBlock(c, e.Alteration.BlockType == AlterationBlockType.Physical,  this.Random);
                            if (blocked)
                            {
                                PublishScenarioMessage(c.RogueName + " blocked the spell!");
                                continue;
                            }
                            c.ApplyPermanentEffect(e.Alteration.Effect);
                        }
                    }
                    break;
                case AlterationType.RunAway:
                    //Not supported for player
                    break;
                case AlterationType.Steal:
                    {
                        Item[] items = e.Targets.First().EquipmentInventory.Union<Item>(e.Targets.First().ConsumableInventory).ToArray();
                        if (items.Length > 0)
                        {
                            Item stolen = items[this.Random.Next(0, items.Length)];
                            if (stolen is Equipment)
                            {
                                e.Targets.First().EquipmentInventory.Remove((Equipment)stolen);
                                this.Player.EquipmentInventory.Add((Equipment)stolen);
                            }
                            else
                            {
                                e.Targets.First().ConsumableInventory.Remove((Consumable)stolen);
                                this.Player.ConsumableInventory.Add((Consumable)stolen);
                            }
                            PublishScenarioMessage("You stole a " + (this.Encyclopedia[stolen.RogueName].IsIdentified ? stolen.RogueName : "???") + "!");
                        }
                    }
                    break;
                case AlterationType.TeleportAllTargets:
                    {
                        foreach (Character c in this.Level.GetEnemies().Where(z => z.IsPhysicallyVisible).ToArray())
                        {
                            bool blocked = Calculator.CalculateSpellBlock(c, e.Alteration.BlockType == AlterationBlockType.Physical,  this.Random);
                            if (blocked)
                            {
                                PublishScenarioMessage(c.RogueName + " blocked the spell!");
                                continue;
                            }
                            Teleport(c);
                        }
                    }
                    break;
                case AlterationType.TeleportSelf:
                    Teleport(this.Player);
                    break;
                case AlterationType.TeleportTarget:
                    Teleport(e.Targets.First());
                    break;
                case AlterationType.OtherMagicEffect:
                    {
                        switch (e.Alteration.OtherEffectType)
                        {
                            case AlterationMagicEffectType.ChangeLevelRandomDown:
                                PublishLoadLevelRequest(Math.Min(this.Random.Next(this.Level.Number + 1, this.Level.Number + 10), this.ScenarioConfig.DungeonTemplate.NumberOfLevels), PlayerStartLocation.Random);
                                break;
                            case AlterationMagicEffectType.ChangeLevelRandomUp:
                                PublishLoadLevelRequest(Math.Max(1, this.Random.Next(this.Level.Number - 10, this.Level.Number - 1)), PlayerStartLocation.Random);
                                break;
                            case AlterationMagicEffectType.EnchantArmor:
                                PublishSplashScreenEvent(SplashEventType.EnchantArmor);
                                break;
                            case AlterationMagicEffectType.EnchantWeapon:
                                PublishSplashScreenEvent(SplashEventType.EnchantWeapon);
                                break;
                            case AlterationMagicEffectType.Identify:
                                PublishSplashScreenEvent(SplashEventType.Identify);
                                break;
                            case AlterationMagicEffectType.RevealFood:
                                RevealFood();
                                break;
                            case AlterationMagicEffectType.RevealItems:
                                RevealItems();
                                break;
                            case AlterationMagicEffectType.RevealLevel:
                                RevealLevel();
                                break;
                            case AlterationMagicEffectType.RevealMonsters:
                                RevealMonsters();
                                break;
                            case AlterationMagicEffectType.RevealSavePoint:
                                RevealSavePoint();
                                break;
                            case AlterationMagicEffectType.RoamingLightSource:
                                //args.Action = LevelModule.ModuleAction.EnterRoamingLightSourceMode;
                                //int time = t.TemporaryAlteration.EventTime.GetRandomValue( r);
                                //args.Arg = time;
                                break;
                            case AlterationMagicEffectType.Uncurse:
                                PublishSplashScreenEvent(SplashEventType.Uncurse);
                                break;
                            case AlterationMagicEffectType.CreateMonster:
                                {
                                    List<Cell> adjacentPoints = Helper.GetAdjacentCells(e.Source.Location.Row, e.Source.Location.Column, this.Level.Grid);
                                    Cell c = adjacentPoints.FirstOrDefault(z => !Helper.IsCellOccupied(z.Location, this.Level, this.Player, false));
                                    if (c != null)
                                        CreateMonster(c.Location, e.Alteration.CreateMonsterEnemy);
                                }
                                break;
                        }
                    }
                    break;
                case AlterationType.AttackAttribute:
                    {
                        switch (e.Alteration.AttackAttributeType)
                        {
                            case AlterationAttackAttributeType.Imbue:
                                PublishSplashScreenEvent(SplashEventType.Imbue);
                                break;
                            case AlterationAttackAttributeType.Passive:
                                e.Source.ApplyAttackAttributePassiveEffect(e.Alteration.Effect);
                                break;
                            case AlterationAttackAttributeType.TemporaryFriendlySource:
                                e.Source.ApplyAttackAttributeTemporaryFriendlyEffect(e.Alteration.Effect);
                                break;
                            case AlterationAttackAttributeType.TemporaryFriendlyTarget:
                                foreach (Character c in e.Targets)
                                    c.ApplyAttackAttributeTemporaryFriendlyEffect(e.Alteration.Effect);
                                break;
                            case AlterationAttackAttributeType.TemporaryMalignSource:
                                e.Source.ApplyAttackAttributeTemporaryMalignEffect(e.Alteration.Effect);
                                break;
                            case AlterationAttackAttributeType.TemporaryMalignTarget:
                                foreach (Character c in e.Targets)
                                    c.ApplyAttackAttributeTemporaryMalignEffect(e.Alteration.Effect);
                                break;
                            case AlterationAttackAttributeType.MeleeTarget:
                                foreach (Character c in e.Targets)
                                {
                                    foreach (AttackAttribute attrib in e.Alteration.Effect.AttackAttributes)
                                    {
                                        double hit = c.GetAttackAttributeMelee(attrib);
                                        if (hit > 0)
                                            c.Hp -= hit;
                                    }
                                }
                                break;
                        }
                    }
                    break;
            }
        }
        public void ProcessEnemyMagicSpellAfterAnimation(AnimationCompletedEvent e)
        {
            bool blocked = Calculator.CalculateSpellBlock(this.Player, e.Alteration.BlockType == AlterationBlockType.Physical,  this.Random);
            if (blocked)
            {
                PublishScenarioMessage(this.Player.RogueName + " has blocked the spell!");
                e = null;
                return;
            }
            
            //TODO - apply stackable
            switch (e.Alteration.Type)
            {
                case AlterationType.PassiveAura:
                    e.Source.ApplyAuraEffect(e.Alteration.Effect, e.Alteration.AuraEffect);
                    break;
                case AlterationType.TemporarySource:
                    e.Source.ApplyTemporaryEffect(e.Alteration.Effect);
                    break;
                case AlterationType.PassiveSource:
                    e.Source.ApplyPassiveEffect(e.Alteration.Effect);
                    break;
                case AlterationType.PermanentSource:
                    e.Source.ApplyPermanentEffect(e.Alteration.Effect);
                    break;
                case AlterationType.TemporaryTarget:
                    {
                        foreach (Character c in e.Targets)
                            c.ApplyTemporaryEffect(e.Alteration.Effect);
                    }
                    break;
                case AlterationType.PermanentTarget:
                    {
                        foreach (Character c in e.Targets)
                            c.ApplyPermanentEffect(e.Alteration.Effect);
                    }
                    break;
                case AlterationType.TemporaryAllTargets:
                    {
                        foreach (Character c in e.Targets)
                            c.ApplyTemporaryEffect(e.Alteration.Effect);
                    }
                    break;
                case AlterationType.PermanentAllTargets:
                    {
                        foreach (Character c in e.Targets)
                            c.ApplyPermanentEffect(e.Alteration.Effect);
                    }
                    break;
                case AlterationType.RunAway:
                    {
                        this.Level.RemoveEnemy(e.Source as Enemy);
                        PublishScenarioMessage("The " + e.Source.RogueName + " ran away!");
                    }
                    break;
                case AlterationType.Steal:
                    {
                        Item[] items = e.Targets.First().EquipmentInventory.Union<Item>(e.Targets.First().ConsumableInventory).ToArray();
                        if (items.Length > 0)
                        {
                            Item stolen = items[this.Random.Next(0, items.Length)];
                            if (stolen is Equipment)
                            {
                                e.Targets.First().EquipmentInventory.Remove((Equipment)stolen);
                                e.Source.EquipmentInventory.Add((Equipment)stolen);
                            }
                            else
                            {
                                e.Targets.First().ConsumableInventory.Remove((Consumable)stolen);
                                e.Source.ConsumableInventory.Add((Consumable)stolen);
                            }
                            PublishScenarioMessage("The " + e.Source.RogueName + " stole your " + (this.Encyclopedia[stolen.RogueName].IsIdentified ? stolen.RogueName : "???") + "!");
                        }
                    }
                    break;
                case AlterationType.TeleportSelf:
                    Teleport(e.Source);
                    break;
                case AlterationType.TeleportAllTargets:
                case AlterationType.TeleportTarget:
                    Teleport(e.Targets.First());
                    break;
                case AlterationType.OtherMagicEffect:
                    {
                        switch (e.Alteration.OtherEffectType)
                        {
                            case AlterationMagicEffectType.CreateMonster:
                                {
                                    List<Cell> adjacentPoints = Helper.GetAdjacentCells(e.Source.Location.Row, e.Source.Location.Column, this.Level.Grid);
                                    Cell c = adjacentPoints.FirstOrDefault(z => !Helper.IsCellOccupied(z.Location, this.Level, this.Player, false));
                                    if (c != null)
                                        CreateMonster(c.Location, e.Alteration.CreateMonsterEnemy);
                                }
                                break;
                        }
                    }
                    break;
                case AlterationType.AttackAttribute:
                    {
                        switch (e.Alteration.AttackAttributeType)
                        {
                            case AlterationAttackAttributeType.Imbue:
                                break;
                            case AlterationAttackAttributeType.Passive:
                                e.Source.ApplyAttackAttributePassiveEffect(e.Alteration.Effect);
                                break;
                            case AlterationAttackAttributeType.TemporaryFriendlySource:
                                e.Source.ApplyAttackAttributeTemporaryFriendlyEffect(e.Alteration.Effect);
                                break;
                            case AlterationAttackAttributeType.TemporaryFriendlyTarget:
                                foreach (Character c in e.Targets)
                                    c.ApplyAttackAttributeTemporaryFriendlyEffect(e.Alteration.Effect);
                                break;
                            case AlterationAttackAttributeType.TemporaryMalignSource:
                                e.Source.ApplyAttackAttributeTemporaryMalignEffect(e.Alteration.Effect);
                                break;
                            case AlterationAttackAttributeType.TemporaryMalignTarget:
                                foreach (Character c in e.Targets)
                                    c.ApplyAttackAttributeTemporaryMalignEffect(e.Alteration.Effect);
                                break;
                            case AlterationAttackAttributeType.MeleeTarget:
                                foreach (Character c in e.Targets)
                                {
                                    foreach (AttackAttribute attrib in e.Alteration.Effect.AttackAttributes)
                                    {
                                        double hit = c.GetAttackAttributeMelee(attrib);
                                        if (hit > 0)
                                            c.Hp -= hit;
                                    }
                                }
                                break;
                        }
                    }
                    break;
            }
        }

        public new void OnAnimationCompleted(AnimationCompletedEvent e)
        {
            switch (e.ReturnAction)
            {
                case AnimationReturnAction.ProcessEnemySpell:
                    ProcessEnemyMagicSpellAfterAnimation(e);
                    break;
                case AnimationReturnAction.ProcessPlayerSpell:
                    ProcessPlayerMagicSpellAfterAnimation(e);
                    break;
            }
        }
    }
}

using Rogue.NET.Common;
using Rogue.NET.Scenario.Model;
using Rogue.NET.Model.Scenario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rogue.NET.Model.Logic
{
    public static class Calculator
    {
        const double LVL_GAIN_BASE = 0.5;

        //Calculate End of Turn Variables
        public static double CalculateEnemyTurn(Enemy e, Player p)
        {
            return e.AgilityBase / p.Agility;
        }
    
        //Calculate Other Vars
        public static double CalculateExpNext(Player p)
        {
            //	y = 100e^(0.25)x  - based on player level 8 with 8500 experience points
            //  available by level 15; and 100 to reach first level + linear component to
            //  avoid easy leveling during low levels
            //return (100 * Math.Exp(0.25*p.Level)) + (300 * p.Level);

            return (p.Level == 0) ? 100 : ((10 * Math.Pow(p.Level + 1, 3)) + (300 + p.Level));
        }
        public static PlayerAdvancementEventArgs CalculateLevelGains(Player p,  Random r)
        {
            List<string> messages = new List<string>();

            string header = p.RogueName + " Has Reached Level " + (p.Level + 1).ToString() + "!";

            //Hp Max
            double d = (p.StrengthBase) * LVL_GAIN_BASE * 2 * r.NextDouble();
            p.HpMax += d;
            messages.Add("Hp Increased By:  " + d.ToString("F3"));

            //Mp Max
            d = (p.IntelligenceBase) * LVL_GAIN_BASE * 2 * r.NextDouble();
            p.MpMax += d;
            messages.Add("Mp Increased By:  " + d.ToString("F3"));

            //Strength
            d = LVL_GAIN_BASE * r.NextDouble();
            p.StrengthBase += (p.AttributeEmphasis == AttributeEmphasis.Strength) ? 3 * d : d;
            messages.Add("Strength Increased By:  " + d.ToString("F3"));

            //Intelligence
            d = LVL_GAIN_BASE * r.NextDouble();
            p.IntelligenceBase += (p.AttributeEmphasis == AttributeEmphasis.Intelligence) ? 3 * d : d;
            messages.Add("Intelligence Increased By:  " + d.ToString("F3"));

            //Agility
            d = LVL_GAIN_BASE * r.NextDouble();
            p.AgilityBase += (p.AttributeEmphasis == AttributeEmphasis.Agility) ? 3 * d : d;
            messages.Add("Agility Increased By:  " + d.ToString("F3"));

            //Level :)
            p.Level++;

            return new PlayerAdvancementEventArgs(header, messages.ToArray());
        }
        public static double CalculatePlayerHit(Player p, Enemy e,  Random r)
        {
            double baseAttk = p.Attack;
            double attk = r.NextDouble() * (baseAttk - (e.StrengthBase / 5));

            //Attack attributes
            foreach (AttackAttribute attrib in p.MeleeAttackAttributes)
                attk += e.GetAttackAttributeMelee(attrib);

            return attk < 0 ? 0 : attk;
        }
        public static double CalculateEnemyHit(Player p, Enemy e,   Random r)
        {
            double baseDef = p.DefenseBase;
            double attk = r.NextDouble() * ((e.Strength) - baseDef);

            //Attack attributes
            foreach (AttackAttribute attrib in e.MeleeAttackAttributes)
                attk += p.GetAttackAttributeMelee(attrib);

            return attk < 0 ? 0 : attk;
        }
        public static bool CalculateSpellBlock(Character c, bool physicalBlock,  Random r)
        {
            return physicalBlock ? r.NextDouble() < c.Dodge : r.NextDouble() < c.MagicBlock;
        }
        public static bool CalculateSpellRequiresTarget(Spell s)
        {
            if (s.Type == AlterationType.PassiveAura || s.Type == AlterationType.PassiveSource)
                return false;

            if (s.Type == AlterationType.PermanentAllTargets
                || s.Type == AlterationType.PermanentTarget
                || s.Type == AlterationType.Steal
                || s.Type == AlterationType.TeleportAllTargets
                || s.Type == AlterationType.TeleportTarget
                || s.Type == AlterationType.TemporaryAllTargets
                || s.Type == AlterationType.TemporaryTarget)
                return true;

            return false;
        }

        //Alteration Calculations
        public static bool CalculateCharacterMeetsAlterationCost(AlterationCostTemplate cost, Character c, out LevelMessageEventArgs msg)
        {
            if (c is Player)
                return CalculatePlayerMeetsAlterationCost(cost, c as Player, out msg);
            else
                return CalculateEnemyMeetsAlterationCost(cost, c as Enemy, out msg);
        }
        public static bool CalculateEnemyMeetsAlterationCost(AlterationCostTemplate cost, Enemy e, out LevelMessageEventArgs msg)
        {
            bool meets = true;

            msg = null;

            meets &= (e.AgilityBase - cost.Agility) >= 0;

            if (!meets)
            {
                msg = new LevelMessageEventArgs("Not enough agility points");
                return false;
            }

            meets &= (e.Hp - cost.Hp) >= 0;

            if (!meets)
            {
                msg = new LevelMessageEventArgs("Not enough HP");
                return false;
            }

            meets &= (e.IntelligenceBase - cost.Intelligence) >= 0;

            if (!meets)
            {
                msg = new LevelMessageEventArgs("Not enough intelligence points");
                return false;
            }

            meets &= (e.Mp - cost.Mp) >= 0;

            if (!meets)
            {
                msg = new LevelMessageEventArgs("Not enough MP");
                return false;
            }

            meets &= (e.StrengthBase - cost.Strength) >= 0;

            if (!meets)
            {
                msg = new LevelMessageEventArgs("Not enough strength points");
                return false;
            }

            return meets;
        }
        public static bool CalculatePlayerMeetsAlterationCost(AlterationCostTemplate cost, Player p, out LevelMessageEventArgs msg)
        {
            bool meets = true;

            msg = null;

            if (cost.Agility > 0)
                meets &= (p.AgilityBase - cost.Agility) >= 0;

            if (!meets)
            {
                msg = new LevelMessageEventArgs("Not enough agility points");
                return false;
            }

            if (cost.Hp > 0)
                meets &= (p.Hp - cost.Hp) >= 0;

            if (!meets)
            {
                msg = new LevelMessageEventArgs("Not enough HP");
                return false;
            }

            if (cost.Intelligence > 0)
                meets &= (p.IntelligenceBase - cost.Intelligence) >= 0;

            if (!meets)
            {
                msg = new LevelMessageEventArgs("Not enough intelligence points");
                return false;
            }

            if (cost.Mp > 0)
                meets &= (p.Mp - cost.Mp) >= 0;

            if (!meets)
            {
                msg = new LevelMessageEventArgs("Not enough MP");
                return false;
            }
            
            if (cost.Strength > 0)
                meets &= (p.StrengthBase - cost.Strength) >= 0;

            if (!meets)
            {
                msg = new LevelMessageEventArgs("Not enough strength points");
                return false;
            }
            
            if (cost.AuraRadius > 0)
                meets &= (p.AuraRadiusBase - cost.AuraRadius) >= 0;

            if (!meets)
            {
                msg = new LevelMessageEventArgs("Not enough aura points");
                return false;
            }

            if (cost.Experience < 0)
                meets &= (p.Experience - cost.Experience) >= 0;

            if (!meets)
            {
                msg = new LevelMessageEventArgs("Not enough experience points");
                return false;
            }

            //meets &= (p.FoodUsagePerTurnBase - cost.FoodUsagePerTurn) >= 0;
            if (cost.Hunger > 0)
                meets &= (p.Hunger + cost.Hunger) < 100;

            if (!meets)
            {
                msg = new LevelMessageEventArgs("Not enough hunger points");
                return false;
            }

            return meets;
        }
        public static bool CalculateCharacterMeetsAlterationCost(AlterationCost cost, Character c, out LevelMessageEventArgs msg)
        {
            if (c is Player)
                return CalculatePlayerMeetsAlterationCost(cost, c as Player, out msg);
            else
                return CalculateEnemyMeetsAlterationCost(cost, c as Enemy, out msg);
        }
        public static bool CalculateEnemyMeetsAlterationCost(AlterationCost cost, Enemy e, out LevelMessageEventArgs msg)
        {
            bool meets = true;

            msg = null;

            meets &= (e.AgilityBase - cost.Agility) > 0;

            if (!meets)
            {
                msg = new LevelMessageEventArgs("Not enough agility points");
                return false;
            }

            meets &= (e.Hp - cost.Hp) > 0;

            if (!meets)
            {
                msg = new LevelMessageEventArgs("Not enough HP");
                return false;
            }

            meets &= (e.IntelligenceBase - cost.Intelligence) > 0;

            if (!meets)
            {
                msg = new LevelMessageEventArgs("Not enough intelligence points");
                return false;
            }

            meets &= (e.Mp - cost.Mp) > 0;

            if (!meets)
            {
                msg = new LevelMessageEventArgs("Not enough MP");
                return false;
            }

            meets &= (e.StrengthBase - cost.Strength) > 0;

            if (!meets)
            {
                msg = new LevelMessageEventArgs("Not enough strength points");
                return false;
            }

            return meets;
        }
        public static bool CalculatePlayerMeetsAlterationCost(AlterationCost cost, Player p, out LevelMessageEventArgs msg)
        {
            bool meets = true;

            msg = null;

            meets &= (p.AgilityBase - cost.Agility) > 0;

            if (!meets)
            {
                msg = new LevelMessageEventArgs("Not enough agility points");
                return false;
            }

            meets &= (p.Hp - cost.Hp) > 0;

            if (!meets)
            {
                msg = new LevelMessageEventArgs("Not enough HP");
                return false;
            }

            meets &= (p.IntelligenceBase - cost.Intelligence) > 0;

            if (!meets)
            {
                msg = new LevelMessageEventArgs("Not enough intelligence points");
                return false;
            }

            meets &= (p.Mp - cost.Mp) > 0;

            if (!meets)
            {
                msg = new LevelMessageEventArgs("Not enough MP");
                return false;
            }

            meets &= (p.StrengthBase - cost.Strength) > 0;

            if (!meets)
            {
                msg = new LevelMessageEventArgs("Not enough strength points");
                return false;
            }

            meets &= (p.AuraRadiusBase - cost.AuraRadius) > 0;
            if (!meets)
            {
                msg = new LevelMessageEventArgs("Not enough aura points");
                return false;
            }

            if (cost.Experience < 0)
                meets &= (p.Experience - cost.Experience) > 0;

            if (!meets)
            {
                msg = new LevelMessageEventArgs("Not enough experience points");
                return false;
            }

            meets &= (p.FoodUsagePerTurnBase - cost.FoodUsagePerTurn) > 0;
            if (cost.Hunger > 0)
                meets &= (p.Hunger + cost.Hunger) < 100;

            if (!meets)
            {
                msg = new LevelMessageEventArgs("Not enough hunger points");
                return false;
            }

            return meets;
        }

        //Copied from template generator.... (maybe should move the template generator down to this namespace...
        public static SymbolDetails CalculateSymbolDelta(SymbolDetailsTemplate t, SymbolDetails symbol)
        {
            SymbolDetails newSymbol = CalculateSymbol(t);
            SymbolDetails oldSymbolCopy = (SymbolDetails)symbol.Clone();

            //Full symbol
            if (t.IsFullSymbolDelta)
                return newSymbol;

            //Aura
            if (t.IsAuraDelta)
                oldSymbolCopy.SmileyAuraColor = t.SmileyAuraColor;

            //Body
            if (t.IsBodyDelta)
                oldSymbolCopy.SmileyBodyColor = t.SmileyBodyColor;

            //Character symbol
            if (t.IsCharacterDelta)
                oldSymbolCopy.CharacterSymbol = t.CharacterSymbol;

            //Character delta
            if (t.IsColorDelta)
                oldSymbolCopy.CharacterColor = t.CharacterColor;

            //Image
            if (t.IsImageDelta)
                oldSymbolCopy.Icon = t.Icon;

            //Line
            if (t.IsLineDelta)
                oldSymbolCopy.SmileyLineColor = t.SmileyLineColor;

            //Mood
            if (t.IsMoodDelta)
                oldSymbolCopy.SmileyMood = t.SmileyMood;

            return oldSymbolCopy;
        }
        public static SymbolDetails CalculateSymbol(SymbolDetailsTemplate t)
        {
            switch (t.Type)
            {
                case SymbolTypes.Character:
                    return new SymbolDetails(1, 10, t.CharacterSymbol, t.CharacterColor);
                case SymbolTypes.Image:
                    return new SymbolDetails(1, 10, t.Icon);
                case SymbolTypes.Smiley:
                default:
                    return new SymbolDetails(1, 10, t.SmileyMood, t.SmileyBodyColor, t.SmileyLineColor, t.SmileyAuraColor);
            }
        }

        //Trancendental functions
        /// <summary>
        /// Calcualtes a fraction [0 lt f(x) lt 1] for [x gt 0]
        /// </summary>
        /// <param name="input">greater than zero - unit scale</param>
        /// <returns>arctan based transcendental number asymptotic to zero f(0) = input, f(1) ~ 1/5, f(inf.) ~ 0</returns>
        public static double CalculateTransendentalFraction(double input)
        {
            return ((-2 / Math.PI) * (Math.Atan(3 * input))) + 1;
        }

        /// <summary>
        /// Calculates attack attribute hit F(D,R,W) = D[1 - (R / D + R)] + DW
        /// </summary>
        /// <param name="attack">Effective melee value for attribute</param>
        /// <param name="resistance">Effective resistance value for attribute</param>
        /// <param name="weakness">Effective weakness value for attribute</param>
        /// <returns>F(A,R,W) = D[1 - (R / D + R)] + DW</returns>
        public static double CalculateAttackAttributeMelee(double attack, double resistance, double weakness)
        {
            return attack > 0 ? attack * (1 - (resistance / (attack + resistance)) + weakness) : 0;
        }
    }
}

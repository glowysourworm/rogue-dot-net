using Rogue.NET.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Processing.Model.Static
{
    public static class PlayerCalculator
    {
        /// <summary>
        /// Calculates experience required to reach next level
        /// </summary>
        /// <param name="level">Current Player level</param>
        public static double CalculateExperienceNext(int level)
        {
            //	y = 100e^(0.25)x  - based on player level 8 with 8500 experience points
            //  available by level 15; and 100 to reach first level + linear component to
            //  avoid easy leveling during low levels
            //return (100 * Math.Exp(0.25*p.Level)) + (300 * p.Level);

            return (level == 0) ? 100 : ((10 * Math.Pow(level + 1, 3)) + (300 + level));
        }

        public static double CalculateHpGain(double baseValue)
        {
            return (baseValue * ModelConstants.LevelGains.LevelGainBase 
                              * ModelConstants.LevelGains.HpGainMultiplier) + ModelConstants.LevelGains.LinearOffset;
        }
        public static double CalculateMpGain(double baseValue)
        {
            return (baseValue * ModelConstants.LevelGains.LevelGainBase
                              * ModelConstants.LevelGains.MpGainMultiplier) + ModelConstants.LevelGains.LinearOffset;
        }
    }
}

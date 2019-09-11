namespace Rogue.NET.Core.Model
{
    public static class ModelConstants
    {
        public const int CellHeight = 15;
        public const int CellWidth = 10;

        public const string UnIdentifiedDisplayName = "???";

        public const double HpRegenBaseMultiplier = 0.01;
        public const double MpRegenBaseMultiplier = 0.01;

        public const double HaulFoodUsageDivisor = 1000;
        public const double HaulMaxStrengthMultiplier = 4;
        public const double MagicDefenseBase = 0.25;
        public const double CriticalHitBase = 0.1;        
        public const double MinSpeed = 0.1;
        public const double MaxSpeed = 1;
        public const double MinFiringDistance = 2.0D;
        public const double HpLowFraction = 0.1D;

        public const double SkillLowProgressIncrement = 0.001;
        public const double SkillMediumProgressIncrement = 0.005;
        public const double SkillHighProgressIncrement = 0.01;
        public const double SkillLowHungerIncrement = 0.1;
        public const double SkillMediumHungerIncrement = 0.75;
        public const double SkillHighHungerIncrement = 1.5;

        public const string DoodadSavePointRogueName = "Save Point";
        public const string DoodadStairsUpRogueName = "Stairs Up";
        public const string DoodadStairsDownRogueName = "Stairs Down";
        public const string DoodadTeleporterARogueName = "Teleporter A";
        public const string DoodadTeleporterBRogueName = "Teleporter B";
        public const string DoodadTeleporterRandomRogueName = "Random Teleporter";

        public static class Melee
        {
            public const double AttackBaseMultiplier = 1.0D;
            public const double DefenseBaseMultiplier = 0.2D;
        }

        public static class LevelGains
        {
            /// <summary>
            /// Base Multiplier for any attribute gain
            /// </summary>
            public const double LevelGainBase = 0.5;

            /// <summary>
            /// Linear offset to guarantee progression of attributes
            /// </summary>
            public const double LinearOffset = 0.1D;

            // Additional multipliers
            public const double HpGainMultiplier = 2.0D;
            public const double MpGainMultiplier = 2.0D;
            public const double AttributeEmphasisMultiplier = 2.0D;
        }

        public static class EquipmentMultipliers
        {
            public const double OneHandedMeleeWeapon = 1.0D;
            public const double TwoHandedMeleeWeapon = 2.0D;
            public const double RangeWeapon = 0.5D;

            public const double Armor =     1.0D;
            public const double Shoulder =  0.2D;
            public const double Boots =     0.15D;
            public const double Gauntlets = 0.15D;
            public const double Belt =      0.1D;
            public const double Shield =    0.3D;
            public const double Helmet =    0.2D;
        }

        public static class Hunger
        {
            public const double HungryThreshold = 50D;
            public const double VeryHungryThreshold = 70D;
            public const double CriticalThreshold = 90D;
        }
    }
}

using System.Windows.Media;

namespace Rogue.NET.Core.Model
{
    public static class ModelConstants
    {
        public const int CellHeight = 15;
        public const int CellWidth = 10;

        /// <summary>
        /// Rendering DPI used for WriteableBitmap API. Rendering the level canvas is 
        /// based on bitmap images rendered from vectors.
        /// </summary>
        public const int RenderingDPI = 96;

        public const int MaxVisibileRadiusPlayer = 100;
        public const int MaxVisibileRadiusNPC = 20;
        public const double MinLightIntensity = 0.2;
        public const double MaxLightIntensity = 1.0;

        public const string UnIdentifiedDisplayName = "???";

        public const double HaulFoodUsageDivisor = 1000;
        public const double HaulMaxStrengthMultiplier = 3.0; 
        public const double MinSpeed = 0.1;
        public const double MaxSpeed = 1;
        public const double MinFiringDistance = 2.0D;
        public const double HealthLowFraction = 0.1D;

        public const string DoodadSavePointRogueName = "Save Point";
        public const string DoodadStairsUpRogueName = "Stairs Up";
        public const string DoodadStairsDownRogueName = "Stairs Down";
        public const string DoodadTransporterRogueName = "Transporter";

        /// <summary>
        /// Number of "buckets" allowed per color / alpha channel when calculating symbol effects
        /// </summary>
        public const int ColorChannelDiscretization = 16;

        public static class Scenario
        {
            public const double MonsterGenerationPerStepDefault = 0.01;
            public const double PartyRoomGenerationRateDefault = 0.05;

            public const int EnemyGenerationDefault = 5;
            public const int FriendlyGenerationDefault = 1;
            public const int ConsumableGenerationDefault = 4;
            public const int EquipmentGenerationDefault = 2;
            public const int DoodadGenerationDefault = 2;
        }
        public static class LayoutGeneration
        {
            /// <summary>
            /// Used for the XOR intersection of the regions during generation
            /// </summary>
            public const int RoomIntersectionPaddingLimit = 3;

            /// <summary>
            /// Padding for the layout around the edge
            /// </summary>
            public const int LayoutPadding = 1;

            // TODO: Create more important layout validator
            public const int RoomMinSize = 1;
        }
        public static class Settings
        {
            public const double ZoomMax = 3.0;
            public const double ZoomMin = 1.0;
            public const double ZoomDefault = 2.0;
            public const double ZoomIncrement = 0.25;
        }
        public static class FrontEnd
        {
            public static readonly Brush LevelBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0x00, 0x0F));

            public static readonly Brush AttackForeground = Brushes.Silver;
            public static readonly Brush DefenseForeground = Brushes.Beige;
            public static readonly Brush StrengthForeground = Brushes.Goldenrod;
            public static readonly Brush AgilityForeground = Brushes.YellowGreen;
            public static readonly Brush IntelligenceForeground = Brushes.Blue;
            public static readonly Brush SpeedForeground = Brushes.Magenta;
            public static readonly Brush HealthRegenForeground = Brushes.DarkRed;
            public static readonly Brush StaminaRegenForeground = Brushes.RosyBrown;
            public static readonly Brush FoodUsageForeground = Brushes.Green;
            public static readonly Brush LightRadiusForeground = Brushes.Yellow;
        }
        public static class Melee
        {
            public const double AttackBaseMultiplier = 1.0D;
            public const double DefenseBaseMultiplier = 0.2D;
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

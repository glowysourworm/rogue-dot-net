using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
using System;
using System.Windows.Media;

namespace Rogue.NET.Core.Model.Scenario.Animation
{
    [Serializable]
    public class AnimationData : RogueBase
    {
        public int RepeatCount { get; set; }
        public int AnimationTime { get; set; }
        public bool AutoReverse { get; set; }
        public bool ConstantVelocity { get; set; }
        public double AccelerationRatio { get; set; }
        public AnimationType Type { get; set; }
        public BrushTemplate FillTemplate { get; set; }
        public BrushTemplate StrokeTemplate { get; set; }
        public double StrokeThickness { get; set; }
        public double Opacity1 { get; set; }
        public double Opacity2 { get; set; }
        public double Height1 { get; set; }
        public double Height2 { get; set; }
        public double Width1 { get; set; }
        public double Width2 { get; set; }
        public int Velocity { get; set; }
        public int ChildCount { get; set; }
        public int Erradicity { get; set; }
        public double RadiusFromFocus { get; set; }
        public double SpiralRate { get; set; }
        public double RoamRadius { get; set; }

        // TODO:ALTERATION
        public AnimationType_NEW Type_NEW { get; set; }

        public AnimationData()
        {
            this.FillTemplate = new BrushTemplate()
            {
                Type = BrushType.Solid,
                SolidColor = Colors.White.ToString()
            };
            this.StrokeTemplate = new BrushTemplate()
            {
                Type = BrushType.Solid,
                SolidColor = Colors.White.ToString()
            };
        }
    }
}

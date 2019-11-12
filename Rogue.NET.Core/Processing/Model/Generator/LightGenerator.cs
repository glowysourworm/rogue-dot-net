using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Processing.Model.Generator
{
    [Export(typeof(ILightGenerator))]
    public class LightGenerator : ILightGenerator
    {
        public LightGenerator()
        {

        }

        public Light GenerateLight(LightTemplate template)
        {
            return new Light()
            {
                Red = template.Red,
                Green = template.Green,
                Blue = template.Blue,
                Intensity = template.Intensity
            };
        }
    }
}

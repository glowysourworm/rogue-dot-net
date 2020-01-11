using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using System.ComponentModel.Composition;

namespace Rogue.NET.Core.Processing.Model.Generator
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(ISymbolDetailsGenerator))]
    public class SymbolDetailsGenerator : ISymbolDetailsGenerator
    {
        public SymbolDetailsGenerator() { }

        public void MapSymbolDetails(SymbolDetailsTemplate source, ScenarioImage dest)
        {
            dest.BackgroundColor = source.BackgroundColor;
            dest.SmileyBodyColor = source.SmileyBodyColor;
            dest.SmileyLineColor = source.SmileyLineColor;
            dest.SmileyExpression = source.SmileyExpression;
            dest.SymbolClampColor = source.SymbolClampColor;
            dest.SymbolEffectType = source.SymbolEffectType;
            dest.SymbolHue = source.SymbolHue;
            dest.SymbolLightness = source.SymbolLightness;
            dest.SymbolPath = source.SymbolPath;
            dest.SymbolSaturation = source.SymbolSaturation;
            dest.SymbolSize = source.SymbolSize;
            dest.SymbolType = source.SymbolType;
        }
    }
}

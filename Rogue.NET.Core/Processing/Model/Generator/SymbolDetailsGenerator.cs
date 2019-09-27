﻿using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using System.ComponentModel.Composition;

namespace Rogue.NET.Core.Processing.Model.Generator
{
    [Export(typeof(ISymbolDetailsGenerator))]
    public class SymbolDetailsGenerator : ISymbolDetailsGenerator
    {
        public SymbolDetailsGenerator() { }
        public void MapSymbolDetails(SymbolDetailsTemplate source, ScenarioImage dest)
        {
            dest.CharacterColor = source.CharacterColor;
            dest.CharacterSymbol = source.CharacterSymbol;
            dest.CharacterSymbolCategory = source.CharacterSymbolCategory;
            dest.SmileyLightRadiusColor = source.SmileyAuraColor;
            dest.SmileyBodyColor = source.SmileyBodyColor;
            dest.SmileyLineColor = source.SmileyLineColor;
            dest.SmileyExpression = source.SmileyExpression;
            dest.Symbol = source.Symbol;
            dest.SymbolHue = source.SymbolHue;
            dest.SymbolLightness = source.SymbolLightness;
            dest.SymbolSaturation = source.SymbolSaturation;
            dest.GameSymbol = source.GameSymbol;
            dest.SymbolType = source.SymbolType;
        }
    }
}

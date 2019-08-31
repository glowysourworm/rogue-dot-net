using Rogue.NET.Core.Model.Generator.Interface;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Content.Doodad;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Rogue.NET.Core.Model.Generator
{
    [Export(typeof(IDoodadGenerator))]
    public class DoodadGenerator : IDoodadGenerator
    {
        [ImportingConstructor]
        public DoodadGenerator()
        {
        }

        public DoodadMagic GenerateDoodad(DoodadTemplate doodadTemplate)
        {
            if (doodadTemplate.IsUnique && doodadTemplate.HasBeenGenerated)
                throw new Exception("Trying to generate a unique Doodad twice");

            var doodad = new DoodadMagic();

            if (doodadTemplate.IsAutomatic)
                doodad.AutomaticAlteration = doodadTemplate.AutomaticAlteration;

            if (doodadTemplate.IsInvoked)
                doodad.InvokedAlteration = doodadTemplate.InvokedAlteration;

            doodad.IsAutomatic = doodadTemplate.IsAutomatic;
            doodad.IsHidden = !doodadTemplate.IsVisible;
            doodad.IsInvoked = doodadTemplate.IsInvoked;
            doodad.IsOneUse = doodadTemplate.IsOneUse;
            doodad.RogueName = doodadTemplate.Name;
            doodad.Icon = doodadTemplate.SymbolDetails.Icon;
            doodad.CharacterSymbol = doodadTemplate.SymbolDetails.CharacterSymbol;
            doodad.CharacterColor = doodadTemplate.SymbolDetails.CharacterColor;
            doodad.SmileyExpression = doodadTemplate.SymbolDetails.SmileyExpression;
            doodad.SmileyLightRadiusColor = doodadTemplate.SymbolDetails.SmileyAuraColor;
            doodad.SmileyBodyColor = doodadTemplate.SymbolDetails.SmileyBodyColor;
            doodad.SmileyLineColor = doodadTemplate.SymbolDetails.SmileyLineColor;
            doodad.SymbolType = doodadTemplate.SymbolDetails.Type;
            doodad.HasBeenUsed = false;
            doodad.HasCharacterClassRequirement = doodadTemplate.HasCharacterClassRequirement;

            // Character Class Requirement
            if (doodadTemplate.HasCharacterClassRequirement)
                doodad.CharacterClass = doodadTemplate.CharacterClass;

            doodadTemplate.HasBeenGenerated = true;

            return doodad;
        }
    }
}

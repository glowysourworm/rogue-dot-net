using Rogue.NET.Core.Model.Generator.Interface;
using Rogue.NET.Core.Model.Scenario.Content.Doodad;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using System;
using System.ComponentModel.Composition;

namespace Rogue.NET.Core.Model.Generator
{
    [Export(typeof(IDoodadGenerator))]
    public class DoodadGenerator : IDoodadGenerator
    {
        private readonly ISpellGenerator _spellGenerator;

        [ImportingConstructor]
        public DoodadGenerator(ISpellGenerator spellGenerator)
        {
            _spellGenerator = spellGenerator;
        }

        public DoodadMagic GenerateDoodad(DoodadTemplate doodadTemplate)
        {
            if (doodadTemplate.IsUnique && doodadTemplate.HasBeenGenerated)
                throw new Exception("Trying to generate a unique Doodad twice");

            var doodad = new DoodadMagic();

            if (doodadTemplate.IsAutomatic)
                doodad.AutomaticSpell = _spellGenerator.GenerateSpell(doodadTemplate.AutomaticMagicSpellTemplate);

            if (doodadTemplate.IsInvoked)
                doodad.InvokedSpell = _spellGenerator.GenerateSpell(doodadTemplate.InvokedMagicSpellTemplate);

            doodad.IsAutomatic = doodadTemplate.IsAutomatic;
            doodad.IsHidden = !doodadTemplate.IsVisible;
            doodad.IsInvoked = doodadTemplate.IsInvoked;
            doodad.IsOneUse = doodadTemplate.IsOneUse;
            doodad.RogueName = doodadTemplate.Name;
            doodad.Icon = doodadTemplate.SymbolDetails.Icon;
            doodad.CharacterSymbol = doodadTemplate.SymbolDetails.CharacterSymbol;
            doodad.CharacterColor = doodadTemplate.SymbolDetails.CharacterColor;
            doodad.SmileyMood = doodadTemplate.SymbolDetails.SmileyMood;
            doodad.SmileyAuraColor = doodadTemplate.SymbolDetails.SmileyAuraColor;
            doodad.SmileyBodyColor = doodadTemplate.SymbolDetails.SmileyBodyColor;
            doodad.SmileyLineColor = doodadTemplate.SymbolDetails.SmileyLineColor;
            doodad.SymbolType = doodadTemplate.SymbolDetails.Type;
            doodad.HasBeenUsed = false;

            // Religious Affiliation Requirement
            //if (doodad.HasReligionRequirement)
            //    doodad.ReligionName = doodadTemplate.ReligiousAffiliationRequirement.Religion.Name;

            doodadTemplate.HasBeenGenerated = true;

            return doodad;
        }
    }
}

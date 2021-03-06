﻿using Rogue.NET.Common.Constant;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Doodad;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using Rogue.NET.Core.Processing.Model.Generator.Interface;

using System;
using System.ComponentModel.Composition;

namespace Rogue.NET.Core.Processing.Model.Generator
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IDoodadGenerator))]
    public class DoodadGenerator : IDoodadGenerator
    {
        readonly ISymbolDetailsGenerator _symbolDetailsGenerator;

        [ImportingConstructor]
        public DoodadGenerator(ISymbolDetailsGenerator symbolDetailsGenerator)
        {
            _symbolDetailsGenerator = symbolDetailsGenerator;
        }

        public DoodadNormal GenerateNormalDoodad(string name, DoodadNormalType type)
        {
            var result = new DoodadNormal();

            result.RogueName = name;
            result.Type = DoodadType.Normal;
            result.NormalType = type;

            result.SymbolType = SymbolType.Game;
            result.SymbolHue = 0;
            result.SymbolLightness = 1;
            result.SymbolSaturation = 1;

            switch (type)
            {
                case DoodadNormalType.StairsUp:
                    result.SymbolPath = GameSymbol.StairsUp;
                    break;
                case DoodadNormalType.StairsDown:
                    result.SymbolPath = GameSymbol.StairsDown;
                    break;
                case DoodadNormalType.SavePoint:
                    result.SymbolPath = GameSymbol.SavePoint;
                    break;
                case DoodadNormalType.Transporter:
                    result.SymbolPath = GameSymbol.Teleport1;
                    break;
                default:
                    break;
            }

            return result;
        }
        public DoodadMagic GenerateMagicDoodad(DoodadTemplate doodadTemplate)
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

            // Map Symbol Details
            _symbolDetailsGenerator.MapSymbolDetails(doodadTemplate.SymbolDetails, doodad);

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

using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using System;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Doodad
{
    [Serializable]
    public class DoodadAlteration : AlterationBase
    {
        public AlterationTargetType TargetType { get; set; }

        public DoodadAlteration()
        {
        }
        public DoodadAlteration(string guid) : base(guid)
        {
        }
        protected override bool ValidateEffectInterfaceType()
        {
            return this.Effect is IDoodadAlterationEffect;
        }
    }
}

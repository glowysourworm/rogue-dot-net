using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using System;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Doodad
{
    [Serializable]
    public class DoodadAlteration : Common.AlterationContainer
    {
        public DoodadAlteration()
        {
        }

        public override Type EffectInterfaceType
        {
            get { return typeof(IDoodadAlterationEffect); }
        }
    }
}

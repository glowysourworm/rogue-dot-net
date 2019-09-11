using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using System;

namespace Rogue.NET.Core.Model.Scenario.Alteration.TemporaryCharacter
{
    [Serializable]
    public class TemporaryCharacterAlteration : Common.AlterationContainer
    {
        public override Type EffectInterfaceType { get { return typeof(ITemporaryCharacterAlterationEffect); } }

        public TemporaryCharacterAlteration()
        {

        }
    }
}

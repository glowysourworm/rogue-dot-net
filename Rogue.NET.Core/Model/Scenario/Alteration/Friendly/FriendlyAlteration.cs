using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using System;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Friendly
{
    [Serializable]
    public class FriendlyAlteration : AlterationContainer
    {
        public override Type EffectInterfaceType { get { return typeof(IFriendlyAlterationEffect); } }

        public FriendlyAlteration()
        {

        }
    }
}

using System;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Common
{
    [Serializable]
    public class AuraSourceParameters
    {
        public string AuraColor { get; set; }
        public int AuraRange { get; set; }

        public AuraSourceParameters()
        {

        }
    }
}

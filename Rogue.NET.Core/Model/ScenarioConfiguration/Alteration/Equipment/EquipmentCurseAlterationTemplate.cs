using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Interface;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Equipment
{
    [Serializable]
    public class EquipmentCurseAlterationTemplate : AlterationTemplate
    {
        private AuraSourceParametersTemplate _auraParameters;

        public AuraSourceParametersTemplate AuraParameters
        {
            get { return _auraParameters; }
            set
            {
                if (_auraParameters != value)
                {
                    _auraParameters = value;
                    OnPropertyChanged("AuraParameters");
                }
            }
        }

        public EquipmentCurseAlterationTemplate()
        {
            this.AuraParameters = new AuraSourceParametersTemplate();
        }
    }
}

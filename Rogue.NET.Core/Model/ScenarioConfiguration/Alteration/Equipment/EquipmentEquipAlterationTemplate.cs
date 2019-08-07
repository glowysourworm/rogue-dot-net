using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Interface;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Equipment
{
    [Serializable]
    public class EquipmentEquipAlterationTemplate : Template
    {
        private IEquipmentEquipAlterationEffectTemplate _effect;
        private AuraSourceParametersTemplate _auraParameters;

        public IEquipmentEquipAlterationEffectTemplate Effect
        {
            get { return _effect; }
            set
            {
                if (_effect != value)
                {
                    _effect = value;
                    OnPropertyChanged("Effect");
                }
            }
        }
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

        public EquipmentEquipAlterationTemplate()
        {
            this.AuraParameters = new AuraSourceParametersTemplate();
        }
    }
}

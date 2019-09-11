using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Skill
{
    [Serializable]
    public class SkillAlterationTemplate : AlterationTemplate
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
                    OnPropertyChanged("BlockType");
                }
            }
        }

        public SkillAlterationTemplate()
        {
            this.AuraParameters = new AuraSourceParametersTemplate();
        }
    }
}

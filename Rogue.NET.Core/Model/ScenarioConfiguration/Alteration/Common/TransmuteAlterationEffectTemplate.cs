using Rogue.NET.Core.Model.Attribute;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Interface;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common
{
    [Serializable]
    [AlterationCostSpecifier(AlterationCostType.OneTime,
                             typeof(IConsumableAlterationEffectTemplate),
                             typeof(IDoodadAlterationEffectTemplate),
                             typeof(ISkillAlterationEffectTemplate))]
    [AlterationBlockable]
    public class TransmuteAlterationEffectTemplate
        : Template, IConsumableAlterationEffectTemplate,
                    IDoodadAlterationEffectTemplate,
                    ISkillAlterationEffectTemplate
    {
        double _probabilityOfSuccess;

        public double ProbabilityOfSuccess
        {
            get { return _probabilityOfSuccess; }
            set
            {
                if (_probabilityOfSuccess != value)
                {
                    _probabilityOfSuccess = value;
                    OnPropertyChanged("ProbabilityOfSuccess");
                }
            }
        }

        public List<TransmuteAlterationEffectItemTemplate> TransmuteItems { get; set; }

        public TransmuteAlterationEffectTemplate()
        {
            this.TransmuteItems = new List<TransmuteAlterationEffectItemTemplate>();
        }
    }
}

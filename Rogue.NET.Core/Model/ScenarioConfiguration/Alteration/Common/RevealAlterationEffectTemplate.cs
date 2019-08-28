using Rogue.NET.Core.Model.Attribute;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Interface;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common
{
    [Serializable]
    [AlterationBlockable]
    [AlterationCostSpecifier(AlterationCostType.OneTime,
                             typeof(IConsumableAlterationEffectTemplate),
                             typeof(ISkillAlterationEffectTemplate))]
    public class RevealAlterationEffectTemplate 
        : Template, IConsumableAlterationEffectTemplate,
                    IDoodadAlterationEffectTemplate,
                    ISkillAlterationEffectTemplate
    {
        AlterationRevealType _type;

        public AlterationRevealType Type
        {
            get { return _type; }
            set
            {
                if (_type != value)
                {
                    _type = value;
                    OnPropertyChanged("Type");
                }
            }
        }

        public RevealAlterationEffectTemplate() { }
    }
}

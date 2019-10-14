using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Interface;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common
{
    [Serializable]
    public class DetectAlterationAlterationEffectTemplate : Template, IConsumableAlterationEffectTemplate,
                                                                      IDoodadAlterationEffectTemplate,
                                                                      ISkillAlterationEffectTemplate
    {
        AlterationCategoryTemplate _alterationCategory;

        public AlterationCategoryTemplate AlterationCategory
        {
            get { return _alterationCategory; }
            set { this.RaiseAndSetIfChanged(ref _alterationCategory, value); }
        }

        public DetectAlterationAlterationEffectTemplate()
        {

        }
    }
}

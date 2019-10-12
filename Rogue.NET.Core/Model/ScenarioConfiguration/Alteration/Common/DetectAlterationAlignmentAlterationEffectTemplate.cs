using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common
{
    [Serializable]
    public class DetectAlterationAlignmentAlterationEffectTemplate : Template, IConsumableAlterationEffectTemplate,
                                                                               IDoodadAlterationEffectTemplate,
                                                                               ISkillAlterationEffectTemplate
    {
        AlterationAlignmentType AlterationAlignmentType;

        public AlterationAlignmentType AlterationType
        {
            get { return AlterationAlignmentType; }
            set { this.RaiseAndSetIfChanged(ref AlterationAlignmentType, value); }
        }

        public DetectAlterationAlignmentAlterationEffectTemplate()
        {

        }
    }
}

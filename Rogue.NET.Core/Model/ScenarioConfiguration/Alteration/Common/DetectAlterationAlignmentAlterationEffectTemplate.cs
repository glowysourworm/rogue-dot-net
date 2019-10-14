using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Interface;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common
{
    [Serializable]
    public class DetectAlterationAlignmentAlterationEffectTemplate : Template, IConsumableAlterationEffectTemplate,
                                                                               IDoodadAlterationEffectTemplate,
                                                                               ISkillAlterationEffectTemplate
    {
        AlterationAlignmentType _alignmentType;
        bool _includeCursedEquipment;

        public AlterationAlignmentType AlignmentType
        {
            get { return _alignmentType; }
            set { this.RaiseAndSetIfChanged(ref _alignmentType, value); }
        }

        public bool IncludeCursedEquipment
        {
            get { return _includeCursedEquipment; }
            set { this.RaiseAndSetIfChanged(ref _includeCursedEquipment, value); }
        }

        public DetectAlterationAlignmentAlterationEffectTemplate()
        {

        }
    }
}

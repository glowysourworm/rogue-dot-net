using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Interface;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common
{
    [Serializable]
    public class BlockAlterationAlterationEffectTemplate : Template, IEquipmentEquipAlterationEffectTemplate,
                                                                     ISkillAlterationEffectTemplate
    {
        AlterationCategoryTemplate _alterationCategory;
        SymbolEffectTemplate _symbolAlteration;

        public AlterationCategoryTemplate AlterationCategory
        {
            get { return _alterationCategory; }
            set { this.RaiseAndSetIfChanged(ref _alterationCategory, value); }
        }
        public SymbolEffectTemplate SymbolAlteration
        {
            get { return _symbolAlteration; }
            set { this.RaiseAndSetIfChanged(ref _symbolAlteration, value); }
        }

        public BlockAlterationAlterationEffectTemplate()
        {
            this.SymbolAlteration = new SymbolEffectTemplate();
        }
    }
}

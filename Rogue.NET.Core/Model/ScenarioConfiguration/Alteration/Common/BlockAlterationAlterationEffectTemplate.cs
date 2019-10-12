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
    public class BlockAlterationAlterationEffectTemplate : Template, IEquipmentEquipAlterationEffectTemplate,
                                                                     ISkillAlterationEffectTemplate
    {
        AlterationCategoryTemplate _alterationCategory;

        public AlterationCategoryTemplate AlterationCategory
        {
            get { return _alterationCategory; }
            set { this.RaiseAndSetIfChanged(ref _alterationCategory, value); }
        }

        public BlockAlterationAlterationEffectTemplate()
        {

        }
    }
}

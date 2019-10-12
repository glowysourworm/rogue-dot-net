using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Common
{
    public class BlockAlterationAlterationEffectTemplateViewModel 
                    : TemplateViewModel, IEquipmentEquipAlterationEffectTemplateViewModel,
                                         ISkillAlterationEffectTemplateViewModel
    {
        AlterationCategoryTemplateViewModel _alterationCategory;

        public AlterationCategoryTemplateViewModel AlterationCategory
        {
            get { return _alterationCategory; }
            set { this.RaiseAndSetIfChanged(ref _alterationCategory, value); }
        }

        public BlockAlterationAlterationEffectTemplateViewModel()
        {

        }
    }
}

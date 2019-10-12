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
    public class UncurseAlterationEffectTemplate : Template, IConsumableAlterationEffectTemplate,
                                                             IDoodadAlterationEffectTemplate,
                                                             ISkillAlterationEffectTemplate
    {
        bool _uncurseAll;

        public bool UncurseAll
        {
            get { return _uncurseAll; }
            set { this.RaiseAndSetIfChanged(ref _uncurseAll, value); }
        }

        public UncurseAlterationEffectTemplate() { }
    }
}

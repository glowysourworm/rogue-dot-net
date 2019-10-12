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
    public class IdentifyAlterationEffectTemplate : Template, IConsumableAlterationEffectTemplate,
                                                              IDoodadAlterationEffectTemplate,
                                                              ISkillAlterationEffectTemplate
    {
        bool _identifyAll;

        public bool IdentifyAll
        {
            get { return _identifyAll; }
            set { this.RaiseAndSetIfChanged(ref _identifyAll, value); }
        }

        public IdentifyAlterationEffectTemplate() { }
    }
}

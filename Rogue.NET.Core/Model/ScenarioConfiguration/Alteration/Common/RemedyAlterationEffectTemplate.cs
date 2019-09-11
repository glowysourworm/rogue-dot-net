using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Interface;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common
{
    [Serializable]
    public class RemedyAlterationEffectTemplate : 
        Template, IConsumableAlterationEffectTemplate,
                  IDoodadAlterationEffectTemplate,
                  ISkillAlterationEffectTemplate
    {
        private AlteredCharacterStateTemplate _remediedState;

        public AlteredCharacterStateTemplate RemediedState
        {
            get { return _remediedState; }
            set
            {
                if (_remediedState != value)
                {
                    _remediedState = value;
                    OnPropertyChanged("RemediedState");
                }
            }
        }

        public RemedyAlterationEffectTemplate()
        {
            this.RemediedState = new AlteredCharacterStateTemplate();
        }
    }
}

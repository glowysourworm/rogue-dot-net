using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Interface;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common
{
    [Serializable]
    public class ChangeLevelAlterationEffectTemplate 
        : Template, IConsumableAlterationEffectTemplate,
                    IDoodadAlterationEffectTemplate,
                    ISkillAlterationEffectTemplate                    
    {
        Range<int> _levelChange;

        public Range<int> LevelChange
        {
            get { return _levelChange; }
            set
            {
                if (_levelChange != value)
                {
                    _levelChange = value;
                    OnPropertyChanged("LevelChange");
                }
            }
        }

        public ChangeLevelAlterationEffectTemplate()
        {
            this.LevelChange = new Range<int>(-50, 0, 0, 50);
        }
    }
}

using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Interface;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common
{
    [Serializable]
    public class TeleportRandomAlterationEffectTemplate 
        : Template, IConsumableAlterationEffectTemplate,
                    IDoodadAlterationEffectTemplate,
                    IEnemyAlterationEffectTemplate,
                    ISkillAlterationEffectTemplate
    {
        AlterationRandomPlacementType _teleportType;
        int _range;

        public AlterationRandomPlacementType TeleportType
        {
            get { return _teleportType; }
            set
            {
                if (_teleportType != value)
                {
                    _teleportType = value;
                    OnPropertyChanged("TeleportType");
                }
            }
        }
        public int Range
        {
            get { return _range; }
            set
            {
                if (_range != value)
                {
                    _range = value;
                    OnPropertyChanged("Range");
                }
            }
        }

        public TeleportRandomAlterationEffectTemplate()
        {

        }
    }
}

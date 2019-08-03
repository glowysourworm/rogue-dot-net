using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Interface;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common
{
    [Serializable]
    public class TeleportAlterationEffectTemplate 
        : Template, IConsumableAlterationEffectTemplate,
                    IDoodadAlterationEffectTemplate,
                    IEnemyAlterationEffectTemplate,
                    ISkillAlterationEffectTemplate
    {
        AlterationRandomPlacementType _teleportType;

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

        public TeleportAlterationEffectTemplate()
        {

        }
    }
}

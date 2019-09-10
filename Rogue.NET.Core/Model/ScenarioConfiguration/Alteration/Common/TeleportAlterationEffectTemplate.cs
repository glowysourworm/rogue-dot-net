using Rogue.NET.Core.Model.Attribute;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Interface;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common
{
    [Serializable]
    [AlterationBlockable]
    [AlterationCostSpecifier(AlterationCostType.OneTime,
                             typeof(IConsumableAlterationEffectTemplate),
                             typeof(IEnemyAlterationEffectTemplate),
                             typeof(ISkillAlterationEffectTemplate))]
    public class TeleportAlterationEffectTemplate 
        : Template, IConsumableAlterationEffectTemplate,
                    IDoodadAlterationEffectTemplate,
                    IEnemyAlterationEffectTemplate,
                    ISkillAlterationEffectTemplate
    {
        AlterationLocationSelectionType _locationSelectionType;
        AlterationRandomPlacementType _teleportType;
        int _range;

        public AlterationLocationSelectionType LocationSelectionType
        {
            get { return _locationSelectionType; }
            set
            {
                if (_locationSelectionType != value)
                {
                    _locationSelectionType = value;
                    OnPropertyChanged("LocationSelectionType");
                }
            }
        }
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

        public TeleportAlterationEffectTemplate()
        {

        }
    }
}

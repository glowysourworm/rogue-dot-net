using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common
{
    [Serializable]
    public class EquipmentModifyAlterationEffectTemplate 
        : Template, IConsumableAlterationEffectTemplate,
                    IDoodadAlterationEffectTemplate,
                    IEnemyAlterationEffectTemplate,
                    ISkillAlterationEffectTemplate
    {
        AlterationModifyEquipmentType _type;
        int _classChange;
        double _qualityChange;

        public AlterationModifyEquipmentType Type
        {
            get { return _type; }
            set
            {
                if (_type != value)
                {
                    _type = value;
                    OnPropertyChanged("Type");
                }
            }
        }
        public int ClassChange
        {
            get { return _classChange; }
            set
            {
                if (_classChange != value)
                {
                    _classChange = value;
                    OnPropertyChanged("ClassChange");
                }
            }
        }
        public double QualityChange
        {
            get { return _qualityChange; }
            set
            {
                if (_qualityChange != value)
                {
                    _qualityChange = value;
                    OnPropertyChanged("QualityChange");
                }
            }
        }

        public List<AttackAttributeTemplate> AttackAttributes { get; set; }

        public EquipmentModifyAlterationEffectTemplate()
        {
            this.AttackAttributes = new List<AttackAttributeTemplate>();
        }
    }
}

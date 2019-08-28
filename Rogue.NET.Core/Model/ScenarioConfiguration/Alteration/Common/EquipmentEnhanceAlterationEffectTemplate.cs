using Rogue.NET.Core.Model.Attribute;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common
{
    /// <summary>
    /// Equipment Modification Enhancement that MAY involve a user dialog
    /// </summary
    [Serializable]
    [AlterationBlockable]
    [AlterationCostSpecifier(AlterationCostType.OneTime,
                             typeof(IConsumableAlterationEffectTemplate),
                             typeof(IDoodadAlterationEffectTemplate),
                             typeof(ISkillAlterationEffectTemplate))]
    public class EquipmentEnhanceAlterationEffectTemplate 
        : Template, IConsumableAlterationEffectTemplate,
                    IDoodadAlterationEffectTemplate,
                    ISkillAlterationEffectTemplate
    {
        AlterationModifyEquipmentType _type;
        int _classChange;
        double _qualityChange;
        bool _useDialog;

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
        public bool UseDialog
        {
            get { return _useDialog; }
            set
            {
                if (_useDialog != value)
                {
                    _useDialog = value;
                    OnPropertyChanged("UseDialog");
                }
            }
        }

        public List<AttackAttributeTemplate> AttackAttributes { get; set; }

        public EquipmentEnhanceAlterationEffectTemplate()
        {
            this.AttackAttributes = new List<AttackAttributeTemplate>();
        }
    }
}

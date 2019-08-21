using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using Rogue.NET.ScenarioEditor.ViewModel.Attribute;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
using Rogue.NET.ScenarioEditor.Views.Assets.SharedControl.AlterationControl.EffectControl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Common
{
    [Serializable]
    [UIType(DisplayName = "Enhance Equipment",
            Description = "Modifies the Player's equipment with the option to use a dialog window",
            ViewType = typeof(EquipmentEnhanceEffectParameters))]
    public class EquipmentEnhanceAlterationEffectTemplateViewModel
        : TemplateViewModel, IConsumableAlterationEffectTemplateViewModel,
                             IDoodadAlterationEffectTemplateViewModel,
                             ISkillAlterationEffectTemplateViewModel
    {
        AlterationModifyEquipmentType _type;
        int _classChange;
        double _qualityChange;
        bool _useDialog;

        public AlterationModifyEquipmentType Type
        {
            get { return _type; }
            set { this.RaiseAndSetIfChanged(ref _type, value); }
        }
        public int ClassChange
        {
            get { return _classChange; }
            set { this.RaiseAndSetIfChanged(ref _classChange, value); }
        }
        public double QualityChange
        {
            get { return _qualityChange; }
            set { this.RaiseAndSetIfChanged(ref _qualityChange, value); }
        }
        public bool UseDialog
        {
            get { return _useDialog; }
            set { this.RaiseAndSetIfChanged(ref _useDialog, value); }
        }

        public ObservableCollection<AttackAttributeTemplateViewModel> AttackAttributes { get; set; }

        public EquipmentEnhanceAlterationEffectTemplateViewModel()
        {
            this.AttackAttributes = new ObservableCollection<AttackAttributeTemplateViewModel>();
        }
    }
}

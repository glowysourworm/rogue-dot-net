﻿using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration;

using ReactiveUI;

using System.Collections.ObjectModel;


namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content
{
    public class EquipmentTemplateViewModel : DungeonObjectTemplateViewModel
    {
        public ObservableCollection<AttackAttributeTemplateViewModel> AttackAttributes { get; set; }

        private RangeViewModel<int> _class;
        private RangeViewModel<double> _quality;
        private EquipmentType _type;
        private SpellTemplateViewModel _equipSpell;
        private SpellTemplateViewModel _attackSpell;
        private SpellTemplateViewModel _curseSpell;
        private ConsumableTemplateViewModel _ammoTemplate;
        private double _weight;
        private bool _hasEquipSpell;
        private bool _hasAttackSpell;
        private bool _hasCurseSpell;

        public RangeViewModel<int> Class
        {
            get { return _class; }
            set { this.RaiseAndSetIfChanged(ref _class, value); }
        }
        public double Weight
        {
            get { return _weight; }
            set { this.RaiseAndSetIfChanged(ref _weight, value); }
        }
        public RangeViewModel<double> Quality
        {
            get { return _quality; }
            set { this.RaiseAndSetIfChanged(ref _quality, value); }
        }
        public EquipmentType Type
        {
            get { return _type; }
            set { this.RaiseAndSetIfChanged(ref _type, value); }
        }
        public SpellTemplateViewModel EquipSpell
        {
            get { return _equipSpell; }
            set { this.RaiseAndSetIfChanged(ref _equipSpell, value); }
        }
        public SpellTemplateViewModel AttackSpell
        {
            get { return _attackSpell; }
            set { this.RaiseAndSetIfChanged(ref _attackSpell, value); }
        }
        public SpellTemplateViewModel CurseSpell
        {
            get { return _curseSpell; }
            set { this.RaiseAndSetIfChanged(ref _curseSpell, value); }
        }
        public ConsumableTemplateViewModel AmmoTemplate
        {
            get { return _ammoTemplate; }
            set { this.RaiseAndSetIfChanged(ref _ammoTemplate, value); }
        }
        public bool HasEquipSpell
        {
            get { return _hasEquipSpell; }
            set { this.RaiseAndSetIfChanged(ref _hasEquipSpell, value); }
        }
        public bool HasAttackSpell
        {
            get { return _hasAttackSpell; }
            set { this.RaiseAndSetIfChanged(ref _hasAttackSpell, value); }
        }
        public bool HasCurseSpell
        {
            get { return _hasCurseSpell; }
            set { this.RaiseAndSetIfChanged(ref _hasCurseSpell, value); }
        }


        public EquipmentTemplateViewModel()
        {
            this.Class = new RangeViewModel<int>(0, 10);
            this.Type = EquipmentType.Ring;
            this.Quality = new RangeViewModel<double>(0, 0, 100, 100);
            this.EquipSpell = new SpellTemplateViewModel();
            this.AttackSpell = new SpellTemplateViewModel();
            this.CurseSpell = new SpellTemplateViewModel();
            this.AmmoTemplate = new ConsumableTemplateViewModel();
            this.AttackAttributes = new ObservableCollection<AttackAttributeTemplateViewModel>();
        }
        public EquipmentTemplateViewModel(DungeonObjectTemplateViewModel tmp)
            : base(tmp)
        {
            this.Class = new RangeViewModel<int>(0, 10);
            this.Type = EquipmentType.Ring;
            this.Quality = new RangeViewModel<double>(0, 0, 100, 100);
            this.EquipSpell = new SpellTemplateViewModel();
            this.AttackSpell = new SpellTemplateViewModel();
            this.CurseSpell = new SpellTemplateViewModel();
            this.AmmoTemplate = new ConsumableTemplateViewModel();
            this.AttackAttributes = new ObservableCollection<AttackAttributeTemplateViewModel>();
        }
    }
}
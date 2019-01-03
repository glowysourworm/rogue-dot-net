using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation;

using System.Collections.ObjectModel;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration
{
    public class SpellTemplateViewModel : DungeonObjectTemplateViewModel
    {
        public ObservableCollection<AnimationTemplateViewModel> Animations { get; set; }
        private AlterationCostTemplateViewModel _cost;
        private AlterationEffectTemplateViewModel _effect;
        private AlterationEffectTemplateViewModel _auraEffect;
        private AlterationType _type;
        //private AlterationBlockType _blockType;
        private AlterationMagicEffectType _otherEffectType;
        private AlterationAttackAttributeType _attackAttributeType;
        private double _effectRange;
        private bool _stackable;
        private string _createMonsterEnemy;
        private string _displayName;

        public AlterationCostTemplateViewModel Cost
        {
            get { return _cost; }
            set { this.RaiseAndSetIfChanged(ref _cost, value); }
        }
        public AlterationEffectTemplateViewModel Effect
        {
            get { return _effect; }
            set { this.RaiseAndSetIfChanged(ref _effect, value); }
        }
        public AlterationEffectTemplateViewModel AuraEffect
        {
            get { return _auraEffect; }
            set { this.RaiseAndSetIfChanged(ref _auraEffect, value); }
        }
        public AlterationType Type
        {
            get { return _type; }
            set { this.RaiseAndSetIfChanged(ref _type, value); }
        }
        //public AlterationBlockType BlockType
        //{
        //    get { return _blockType; }
        //    set { this.RaiseAndSetIfChanged(ref _blockType, value); }
        //}
        public AlterationMagicEffectType OtherEffectType
        {
            get { return _otherEffectType; }
            set { this.RaiseAndSetIfChanged(ref _otherEffectType, value); }
        }
        public AlterationAttackAttributeType AttackAttributeType
        {
            get { return _attackAttributeType; }
            set { this.RaiseAndSetIfChanged(ref _attackAttributeType, value); }
        }
        public double EffectRange
        {
            get { return _effectRange; }
            set { this.RaiseAndSetIfChanged(ref _effectRange, value); }
        }
        public bool Stackable
        {
            get { return _stackable; }
            set { this.RaiseAndSetIfChanged(ref _stackable, value); }
        }
        public string CreateMonsterEnemy
        {
            get { return _createMonsterEnemy; }
            set { this.RaiseAndSetIfChanged(ref _createMonsterEnemy, value); }
        }
        public string DisplayName
        {
            get { return _displayName; }
            set { this.RaiseAndSetIfChanged(ref _displayName, value); }
        }

        public SpellTemplateViewModel()
        {
            this.Animations = new ObservableCollection<AnimationTemplateViewModel>();
            this.Cost = new AlterationCostTemplateViewModel();
            this.Effect = new AlterationEffectTemplateViewModel();
            this.AuraEffect = new AlterationEffectTemplateViewModel();

            this.CreateMonsterEnemy = "";
            this.DisplayName = "";
        }
    }
}

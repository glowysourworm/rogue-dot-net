using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content
{
    public class ConsumableTemplateViewModel : DungeonObjectTemplateViewModel
    {
        private ConsumableType _type;
        private ConsumableSubType _subType;
        private double _weight;
        private int _levelRequired;
        private RangeViewModel<int> _useCount;
        private bool _hasLearnedSkill;
        private bool _hasSpell;
        private bool _isProjectile;
        private bool _identifyOnUse;
        private SpellTemplateViewModel _spellTemplate;
        private SkillSetTemplateViewModel _learnedSkill;
        private SpellTemplateViewModel _projectileSpellTemplate;
        private SpellTemplateViewModel _ammoSpellTemplate;
        private string _noteMessage;


        public ConsumableType Type
        {
            get { return _type; }
            set { this.RaiseAndSetIfChanged(ref _type, value); }
        }
        public ConsumableSubType SubType
        {
            get { return _subType; }
            set { this.RaiseAndSetIfChanged(ref _subType, value); }
        }
        public double Weight
        {
            get { return _weight; }
            set { this.RaiseAndSetIfChanged(ref _weight, value); }
        }
        public int LevelRequired
        {
            get { return _levelRequired; }
            set { this.RaiseAndSetIfChanged(ref _levelRequired, value); }
        }
        public RangeViewModel<int> UseCount
        {
            get { return _useCount; }
            set { this.RaiseAndSetIfChanged(ref _useCount, value); }
        }
        public bool HasLearnedSkill
        {
            get { return _hasLearnedSkill; }
            set { this.RaiseAndSetIfChanged(ref _hasLearnedSkill, value); }
        }
        public bool HasSpell
        {
            get { return _hasSpell; }
            set { this.RaiseAndSetIfChanged(ref _hasSpell, value); }
        }
        public bool IsProjectile
        {
            get { return _isProjectile; }
            set { this.RaiseAndSetIfChanged(ref _isProjectile, value); }
        }
        public bool IdentifyOnUse
        {
            get { return _identifyOnUse; }
            set { this.RaiseAndSetIfChanged(ref _identifyOnUse, value); }
        }
        public SpellTemplateViewModel SpellTemplate
        {
            get { return _spellTemplate; }
            set { this.RaiseAndSetIfChanged(ref _spellTemplate, value); }
        }
        public SkillSetTemplateViewModel LearnedSkill
        {
            get { return _learnedSkill; }
            set { this.RaiseAndSetIfChanged(ref _learnedSkill, value); }
        }
        public SpellTemplateViewModel ProjectileSpellTemplate
        {
            get { return _projectileSpellTemplate; }
            set { this.RaiseAndSetIfChanged(ref _projectileSpellTemplate, value); }
        }
        public SpellTemplateViewModel AmmoSpellTemplate
        {
            get { return _ammoSpellTemplate; }
            set { this.RaiseAndSetIfChanged(ref _ammoSpellTemplate, value); }
        }
        public string NoteMessage
        {
            get { return _noteMessage; }
            set { this.RaiseAndSetIfChanged(ref _noteMessage, value); }
        }

        public ConsumableTemplateViewModel()
        {
            this.Type = ConsumableType.OneUse;
            this.SubType = ConsumableSubType.Food;
            this.ProjectileSpellTemplate = new SpellTemplateViewModel();
            this.SpellTemplate = new SpellTemplateViewModel();
            this.AmmoSpellTemplate = new SpellTemplateViewModel();
            this.LearnedSkill = new SkillSetTemplateViewModel();
            this.UseCount = new RangeViewModel<int>(0, 0, 0, 20);
            this.IsObjectiveItem = false;
            this.IsUnique = false;
            this.IsProjectile = false;
            this.IdentifyOnUse = false;
            this.NoteMessage = "";
        }
        public ConsumableTemplateViewModel(DungeonObjectTemplateViewModel tmp) : base(tmp)
        {
            this.Type = ConsumableType.OneUse;
            this.SubType = ConsumableSubType.Food;
            this.ProjectileSpellTemplate = new SpellTemplateViewModel();
            this.SpellTemplate = new SpellTemplateViewModel();
            this.AmmoSpellTemplate = new SpellTemplateViewModel();
            this.LearnedSkill = new SkillSetTemplateViewModel();
            this.UseCount = new RangeViewModel<int>(0, 0, 0, 20);
            this.IsObjectiveItem = false;
            this.IsUnique = false;
            this.IsProjectile = false;
            this.IdentifyOnUse = false;
            this.NoteMessage = "";
        }
    }
}

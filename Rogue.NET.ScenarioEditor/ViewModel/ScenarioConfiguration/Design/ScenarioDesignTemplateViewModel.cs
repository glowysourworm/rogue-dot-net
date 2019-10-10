using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using System.Collections.ObjectModel;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Design
{
    public class ScenarioDesignTemplateViewModel : TemplateViewModel
    {
        private string _objectiveDescription;
        private int _skillPointsPerCharacterPoint;
        private int _hpPerCharacterPoint;
        private int _staminaPerCharacterPoint;
        private double _strengthPerCharacterPoint;
        private double _agilityPerCharacterPoint;
        private double _intelligencePerCharacterPoint;

        public string ObjectiveDescription
        {
            get { return _objectiveDescription; }
            set { this.RaiseAndSetIfChanged(ref _objectiveDescription, value); }
        }
        public int SkillPointsPerCharacterPoint
        {
            get { return _skillPointsPerCharacterPoint; }
            set { this.RaiseAndSetIfChanged(ref _skillPointsPerCharacterPoint, value); }
        }
        public int HpPerCharacterPoint
        {
            get { return _hpPerCharacterPoint; }
            set { this.RaiseAndSetIfChanged(ref _hpPerCharacterPoint, value); }
        }
        public int StaminaPerCharacterPoint
        {
            get { return _staminaPerCharacterPoint; }
            set { this.RaiseAndSetIfChanged(ref _staminaPerCharacterPoint, value); }
        }
        public double StrengthPerCharacterPoint
        {
            get { return _strengthPerCharacterPoint; }
            set { this.RaiseAndSetIfChanged(ref _strengthPerCharacterPoint, value); }
        }
        public double AgilityPerCharacterPoint
        {
            get { return _agilityPerCharacterPoint; }
            set { this.RaiseAndSetIfChanged(ref _agilityPerCharacterPoint, value); }
        }
        public double IntelligencePerCharacterPoint
        {
            get { return _intelligencePerCharacterPoint; }
            set { this.RaiseAndSetIfChanged(ref _intelligencePerCharacterPoint, value); }
        }

        public ObservableCollection<LevelTemplateViewModel> LevelDesigns { get; set; }

        public ScenarioDesignTemplateViewModel()
        {
            this.LevelDesigns = new ObservableCollection<LevelTemplateViewModel>();
            this.ObjectiveDescription = "Objective Description (Goes Here)";

            this.HpPerCharacterPoint = 5;
            this.StaminaPerCharacterPoint = 2;

            this.SkillPointsPerCharacterPoint = 1;

            this.AgilityPerCharacterPoint = 1;
            this.IntelligencePerCharacterPoint = 1;
            this.StrengthPerCharacterPoint = 1;
        }
    }
}

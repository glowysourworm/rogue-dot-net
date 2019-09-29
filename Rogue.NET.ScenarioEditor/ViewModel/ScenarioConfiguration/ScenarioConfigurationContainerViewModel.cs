using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Design;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Layout;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration
{
    public class ScenarioConfigurationContainerViewModel : INotifyPropertyChanged, INotifyPropertyChanging
    {        
        DungeonTemplateViewModel _dungeonTemplate;
        ScenarioDesignTemplateViewModel _scenarioDesign;

        public DungeonTemplateViewModel DungeonTemplate
        {
            get { return _dungeonTemplate; }
            set { this.RaiseAndSetIfChanged(ref _dungeonTemplate, value); }
        }
        public ScenarioDesignTemplateViewModel ScenarioDesign
        {
            get { return _scenarioDesign; }
            set { this.RaiseAndSetIfChanged(ref _scenarioDesign, value); }
        }
        public ObservableCollection<PlayerTemplateViewModel> PlayerTemplates { get; set; }
        public ObservableCollection<LayoutTemplateViewModel> LayoutTemplates { get; set; }
        public ObservableCollection<EnemyTemplateViewModel> EnemyTemplates { get; set; }
        public ObservableCollection<FriendlyTemplateViewModel> FriendlyTemplates { get; set; }
        public ObservableCollection<EquipmentTemplateViewModel> EquipmentTemplates { get; set; }
        public ObservableCollection<ConsumableTemplateViewModel> ConsumableTemplates { get; set; }
        public ObservableCollection<DoodadTemplateViewModel> DoodadTemplates { get; set; }
        public ObservableCollection<SkillSetTemplateViewModel> SkillTemplates { get; set; }

        // "General" Assets
        public ObservableCollection<AttackAttributeTemplateViewModel> AttackAttributes { get; set; }
        public ObservableCollection<AlteredCharacterStateTemplateViewModel> AlteredCharacterStates { get; set; }

        public ScenarioConfigurationContainerViewModel()
        {
            this.DungeonTemplate = new DungeonTemplateViewModel();
            this.ScenarioDesign = new ScenarioDesignTemplateViewModel();

            this.PlayerTemplates = new ObservableCollection<PlayerTemplateViewModel>();
            this.LayoutTemplates = new ObservableCollection<LayoutTemplateViewModel>();
            this.EnemyTemplates = new ObservableCollection<EnemyTemplateViewModel>();
            this.FriendlyTemplates = new ObservableCollection<FriendlyTemplateViewModel>();
            this.EquipmentTemplates = new ObservableCollection<EquipmentTemplateViewModel>();
            this.ConsumableTemplates = new ObservableCollection<ConsumableTemplateViewModel>();
            this.SkillTemplates = new ObservableCollection<SkillSetTemplateViewModel>();
            this.DoodadTemplates = new ObservableCollection<DoodadTemplateViewModel>();
            this.AttackAttributes = new ObservableCollection<AttackAttributeTemplateViewModel>();
            this.AlteredCharacterStates = new ObservableCollection<AlteredCharacterStateTemplateViewModel>();
        }

        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaiseAndSetIfChanged<T>(ref T field, T value, [CallerMemberName] string memberName = "")
        {
            var changed = false;
            if (field == null)
                changed = value != null;
            else
                changed = !field.Equals(value);

            if (changed)
            {
                if (PropertyChanging != null)
                    PropertyChanging(this, new PropertyChangingEventArgs(memberName));

                field = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(memberName));
            }
        }
    }
}

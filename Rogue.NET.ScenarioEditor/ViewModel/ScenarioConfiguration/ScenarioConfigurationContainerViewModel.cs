using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Layout;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration
{
    public class ScenarioConfigurationContainerViewModel : INotifyPropertyChanged, INotifyPropertyChanging
    {        
        DungeonTemplateViewModel _dungeonTemplate;
        PlayerTemplateViewModel _playerTemplate;
        ScenarioConfigurationAlterationContainerViewModel _alterationContainer;

        public DungeonTemplateViewModel DungeonTemplate
        {
            get { return _dungeonTemplate; }
            set { this.RaiseAndSetIfChanged(ref _dungeonTemplate, value); }
        }
        public PlayerTemplateViewModel PlayerTemplate
        {
            get { return _playerTemplate; }
            set { this.RaiseAndSetIfChanged(ref _playerTemplate, value); }
        }
        public ScenarioConfigurationAlterationContainerViewModel AlterationContainer
        {
            get { return _alterationContainer; }
            set { this.RaiseAndSetIfChanged(ref _alterationContainer, value); }
        }

        public ObservableCollection<SkillSetTemplateViewModel> SkillTemplates { get; set; }
        public ObservableCollection<BrushTemplateViewModel> BrushTemplates { get; set; }
        public ObservableCollection<EnemyTemplateViewModel> EnemyTemplates { get; set; }
        public ObservableCollection<AnimationTemplateViewModel> AnimationTemplates { get; set; }
        public ObservableCollection<EquipmentTemplateViewModel> EquipmentTemplates { get; set; }
        public ObservableCollection<ConsumableTemplateViewModel> ConsumableTemplates { get; set; }
        public ObservableCollection<DoodadTemplateViewModel> DoodadTemplates { get; set; }
        public ObservableCollection<AttackAttributeTemplateViewModel> AttackAttributes { get; set; }
        public ObservableCollection<AlteredCharacterStateTemplateViewModel> AlteredCharacterStates { get; set; }
        public ObservableCollection<CharacterClassTemplateViewModel> CharacterClasses { get; set; }

        public ScenarioConfigurationContainerViewModel()
        {
            this.DungeonTemplate = new DungeonTemplateViewModel();
            this.EnemyTemplates = new ObservableCollection<EnemyTemplateViewModel>();
            this.BrushTemplates = new ObservableCollection<BrushTemplateViewModel>();
            this.EquipmentTemplates = new ObservableCollection<EquipmentTemplateViewModel>();
            this.AnimationTemplates = new ObservableCollection<AnimationTemplateViewModel>();
            this.ConsumableTemplates = new ObservableCollection<ConsumableTemplateViewModel>();
            this.SkillTemplates = new ObservableCollection<SkillSetTemplateViewModel>();
            this.PlayerTemplate = new PlayerTemplateViewModel();
            this.DoodadTemplates = new ObservableCollection<DoodadTemplateViewModel>();
            this.AttackAttributes = new ObservableCollection<AttackAttributeTemplateViewModel>();
            this.AlteredCharacterStates = new ObservableCollection<AlteredCharacterStateTemplateViewModel>();
            this.CharacterClasses = new ObservableCollection<CharacterClassTemplateViewModel>();
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

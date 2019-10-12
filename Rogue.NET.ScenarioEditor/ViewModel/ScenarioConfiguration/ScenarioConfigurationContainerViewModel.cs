using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Design;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Layout;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration
{
    public class ScenarioConfigurationContainerViewModel : INotifyPropertyChanged, INotifyPropertyChanging
    {        
        // Primary Design Container
        ScenarioDesignTemplateViewModel _scenarioDesign;

        // Asset Collections
        ObservableCollection<PlayerTemplateViewModel> _playerTemplates;
        ObservableCollection<LayoutTemplateViewModel> _layoutTemplates;
        ObservableCollection<EnemyTemplateViewModel> _enemyTemplates;
        ObservableCollection<FriendlyTemplateViewModel> _friendlyTemplates;
        ObservableCollection<EquipmentTemplateViewModel> _equipmentTemplates;
        ObservableCollection<ConsumableTemplateViewModel> _consumableTemplates;
        ObservableCollection<DoodadTemplateViewModel> _doodadTemplates;
        ObservableCollection<SkillSetTemplateViewModel> _skillTemplates;

        // "General" Assets
        ObservableCollection<AttackAttributeTemplateViewModel> _attackAttributes;
        ObservableCollection<AlteredCharacterStateTemplateViewModel> _alteredCharacterStates;
        ObservableCollection<SymbolPoolItemTemplateViewModel> _symbolPool;
        ObservableCollection<AlterationCategoryTemplateViewModel> _alterationCategories;

        public ScenarioDesignTemplateViewModel ScenarioDesign
        {
            get { return _scenarioDesign; }
            set { this.RaiseAndSetIfChanged(ref _scenarioDesign, value); }
        }
        public ObservableCollection<PlayerTemplateViewModel> PlayerTemplates
        {
            get { return _playerTemplates; }
            set { this.RaiseAndSetIfChanged(ref _playerTemplates, value); }
        }
        public ObservableCollection<LayoutTemplateViewModel> LayoutTemplates
        {
            get { return _layoutTemplates; }
            set { this.RaiseAndSetIfChanged(ref _layoutTemplates, value); }
        }
        public ObservableCollection<EnemyTemplateViewModel> EnemyTemplates
        {
            get { return _enemyTemplates; }
            set { this.RaiseAndSetIfChanged(ref _enemyTemplates, value); }
        }
        public ObservableCollection<FriendlyTemplateViewModel> FriendlyTemplates
        {
            get { return _friendlyTemplates; }
            set { this.RaiseAndSetIfChanged(ref _friendlyTemplates, value); }
        }
        public ObservableCollection<EquipmentTemplateViewModel> EquipmentTemplates
        {
            get { return _equipmentTemplates; }
            set { this.RaiseAndSetIfChanged(ref _equipmentTemplates, value); }
        }
        public ObservableCollection<ConsumableTemplateViewModel> ConsumableTemplates
        {
            get { return _consumableTemplates; }
            set { this.RaiseAndSetIfChanged(ref _consumableTemplates, value); }
        }
        public ObservableCollection<DoodadTemplateViewModel> DoodadTemplates
        {
            get { return _doodadTemplates; }
            set { this.RaiseAndSetIfChanged(ref _doodadTemplates, value); }
        }
        public ObservableCollection<SkillSetTemplateViewModel> SkillTemplates
        {
            get { return _skillTemplates; }
            set { this.RaiseAndSetIfChanged(ref _skillTemplates, value); }
        }

        // "General" Assets
        public ObservableCollection<AttackAttributeTemplateViewModel> AttackAttributes
        {
            get { return _attackAttributes; }
            set { this.RaiseAndSetIfChanged(ref _attackAttributes, value); }
        }
        public ObservableCollection<AlteredCharacterStateTemplateViewModel> AlteredCharacterStates
        {
            get { return _alteredCharacterStates; }
            set { this.RaiseAndSetIfChanged(ref _alteredCharacterStates, value); }
        }
        public ObservableCollection<SymbolPoolItemTemplateViewModel> SymbolPool
        {
            get { return _symbolPool; }
            set { this.RaiseAndSetIfChanged(ref _symbolPool, value); }
        }
        public ObservableCollection<AlterationCategoryTemplateViewModel> AlterationCategories
        {
            get { return _alterationCategories; }
            set { this.RaiseAndSetIfChanged(ref _alterationCategories, value); }
        }

        public ScenarioConfigurationContainerViewModel()
        {
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
            this.SymbolPool = new ObservableCollection<SymbolPoolItemTemplateViewModel>();
            this.AlterationCategories = new ObservableCollection<AlterationCategoryTemplateViewModel>();
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

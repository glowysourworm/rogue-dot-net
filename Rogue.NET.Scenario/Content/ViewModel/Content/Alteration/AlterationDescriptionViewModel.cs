using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Extension;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using Rogue.NET.Scenario.Content.ViewModel.Content.ScenarioMetaData;

using System.Linq;
using System.Collections.ObjectModel;

namespace Rogue.NET.Scenario.Content.ViewModel.Content.Alteration
{
    public class AlterationDescriptionViewModel : NotifyViewModel
    {
        string _displayName;
        string _type;                    // Passive / Temporary / Temporary (Friendly) / Temporary (Malign)

        bool _isAlteredState;
        ScenarioImageViewModel _alteredCharacterState;

        public string DisplayName
        {
            get { return _displayName; }
            set { this.RaiseAndSetIfChanged(ref _displayName, value); }
        }
        public string Type
        {
            get { return _type; }
            set { this.RaiseAndSetIfChanged(ref _type, value); }
        }
        public bool IsAlteredState
        {
            get { return _isAlteredState; }
            set { this.RaiseAndSetIfChanged(ref _isAlteredState, value); }
        }
        public ScenarioImageViewModel AlteredCharacterState
        {
            get { return _alteredCharacterState; }
            set { this.RaiseAndSetIfChanged(ref _alteredCharacterState, value); }
        }

        /// <summary>
        /// These are calculated with pre-set display values
        /// </summary>
        public ObservableCollection<AlterationAttributeViewModel> AlterationCostAttributes { get; set; }

        /// <summary>
        /// These are calculated with pre-set display values
        /// </summary>
        public ObservableCollection<AlterationAttributeViewModel> AlterationEffectAttributes { get; set; }

        /// <summary>
        /// Attack Attributes for the alteration effect
        /// </summary>
        public ObservableCollection<AttackAttributeViewModel> AlterationEffectAttackAttributes { get; set; }

        public AlterationDescriptionViewModel()
        {
            this.AlterationCostAttributes = new ObservableCollection<AlterationAttributeViewModel>();
            this.AlterationEffectAttributes = new ObservableCollection<AlterationAttributeViewModel>();
            this.AlterationEffectAttackAttributes = new ObservableCollection<AttackAttributeViewModel>();

            this.IsAlteredState = false;
        }

        /// <summary>
        /// NOT LIKED - But this constructor can use the Character Alteration.Get tuple to create
        /// an Aleration ViewModel. This should be refactored to create some other data class
        /// </summary>
        public AlterationDescriptionViewModel(
                string alterationName, 
                string alterationType = null,
                IAlterationEffect alterationEffect = null,
                AlteredCharacterState alteredState = null,
                AlterationCost alterationCost = null)
        {
            this.DisplayName = alterationName;
            this.Type = alterationType ?? "";

            // Character State (Handles Null)
            this.IsAlteredState = alteredState != null &&
                                  alteredState.BaseType != CharacterStateType.Normal;

            // Character State (Handles Null)
            this.AlteredCharacterState = alteredState == null ? null : new ScenarioImageViewModel(alteredState);

            // Alteration Cost (Handles Null / may not have a corresponding effect)
            this.AlterationCostAttributes = alterationCost != null ?
                new ObservableCollection<AlterationAttributeViewModel>(
                    alterationCost.GetUIAttributes().Select(x => new AlterationAttributeViewModel()
                    {
                        AttributeName = x.Key,
                        AttributeValue = x.Value.ToString("F2")
                    })) :
                new ObservableCollection<AlterationAttributeViewModel>();

            // Alteration Effect (Handles Null / may not have a corresponding cost)
            this.AlterationEffectAttributes = alterationEffect != null ?
                new ObservableCollection<AlterationAttributeViewModel>(
                    alterationEffect.GetUIAttributes().Select(x => new AlterationAttributeViewModel()
                    {
                        AttributeName = x.Key,
                        AttributeValue = x.Value.ToString("F2")
                    })) :

                new ObservableCollection<AlterationAttributeViewModel>();

            // Alteration Attack Attributes (Handles Null / may not have a corresponding cost)
            this.AlterationEffectAttackAttributes = alterationEffect != null ?
                new ObservableCollection<AttackAttributeViewModel>(
                    alterationEffect.GetAttackAttributes()
                                    .Where(x => x.Attack > 0 || x.Resistance > 0 || x.Weakness > 0)
                                    .Select(x => new AttackAttributeViewModel(x))) :

                new ObservableCollection<AttackAttributeViewModel>();
        }
    }
}

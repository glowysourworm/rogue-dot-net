using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content
{
    public class DoodadTemplateViewModel : DungeonObjectTemplateViewModel
    {
        private SpellTemplateViewModel _automaticMagicSpellTemplate;
        private SpellTemplateViewModel _invokedMagicSpellTemplate;
        private bool _isAutomatic;
        private bool _isVisible;
        private bool _isInvoked;
        private bool _isOneUse;
        private bool _hasReligiousAffiliationRequirement;
        private ReligiousAffiliationRequirementTemplateViewModel _religiousAffiliationRequirement;

        public SpellTemplateViewModel AutomaticMagicSpellTemplate
        {
            get { return _automaticMagicSpellTemplate; }
            set { this.RaiseAndSetIfChanged(ref _automaticMagicSpellTemplate, value); }
        }
        public SpellTemplateViewModel InvokedMagicSpellTemplate
        {
            get { return _invokedMagicSpellTemplate; }
            set { this.RaiseAndSetIfChanged(ref _invokedMagicSpellTemplate, value); }
        }
        public bool IsAutomatic
        {
            get { return _isAutomatic; }
            set { this.RaiseAndSetIfChanged(ref _isAutomatic, value); }
        }
        public bool IsVisible
        {
            get { return _isVisible; }
            set { this.RaiseAndSetIfChanged(ref _isVisible, value); }
        }
        public bool IsInvoked
        {
            get { return _isInvoked; }
            set { this.RaiseAndSetIfChanged(ref _isInvoked, value); }
        }
        public bool IsOneUse
        {
            get { return _isOneUse; }
            set { this.RaiseAndSetIfChanged(ref _isOneUse, value); }
        }
        public bool HasReligiousAffiliationRequirement
        {
            get { return _hasReligiousAffiliationRequirement; }
            set { this.RaiseAndSetIfChanged(ref _hasReligiousAffiliationRequirement, value); }
        }
        public ReligiousAffiliationRequirementTemplateViewModel ReligiousAffiliationRequirement
        {
            get { return _religiousAffiliationRequirement; }
            set { this.RaiseAndSetIfChanged(ref _religiousAffiliationRequirement, value); }
        }

        public DoodadTemplateViewModel()
        {
            this.AutomaticMagicSpellTemplate = new SpellTemplateViewModel();
            this.InvokedMagicSpellTemplate = new SpellTemplateViewModel();
            this.IsUnique = false;
            this.IsOneUse = false;
            this.HasReligiousAffiliationRequirement = false;
            this.ReligiousAffiliationRequirement = new ReligiousAffiliationRequirementTemplateViewModel();
        }
        public DoodadTemplateViewModel(DungeonObjectTemplateViewModel tmp) : base(tmp)
        {
            this.AutomaticMagicSpellTemplate = new SpellTemplateViewModel();
            this.InvokedMagicSpellTemplate = new SpellTemplateViewModel();
            this.IsUnique = false;
            this.IsOneUse = false;
            this.HasReligiousAffiliationRequirement = false;
            this.ReligiousAffiliationRequirement = new ReligiousAffiliationRequirementTemplateViewModel();
        }
    }
}

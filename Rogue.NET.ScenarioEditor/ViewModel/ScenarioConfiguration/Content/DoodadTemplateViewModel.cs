using Rogue.NET.ScenarioEditor.ViewModel.Attribute;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Doodad;
using Rogue.NET.ScenarioEditor.Views;
using Rogue.NET.ScenarioEditor.Views.Assets;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content
{
    [UIType(DisplayName = "Scenario Object",
            Description = "Item that is used by most characters. Players can use any Scenario Object. All other characters will trip automatic Scenario Object effects.",
            ViewType = typeof(Doodad),
            BaseType = UITypeAttributeBaseType.Asset)]
    public class DoodadTemplateViewModel : DungeonObjectTemplateViewModel
    {
        private DoodadAlterationTemplateViewModel _automaticAlteration;
        private DoodadAlterationTemplateViewModel _invokedAlteration;
        private bool _isAutomatic;
        private bool _isVisible;
        private bool _isInvoked;
        private bool _isOneUse;
        private bool _hasCharacterClassRequirement;
        private string _characterClass;

        public DoodadAlterationTemplateViewModel AutomaticAlteration
        {
            get { return _automaticAlteration; }
            set { this.RaiseAndSetIfChanged(ref _automaticAlteration, value); }
        }
        public DoodadAlterationTemplateViewModel InvokedAlteration
        {
            get { return _invokedAlteration; }
            set { this.RaiseAndSetIfChanged(ref _invokedAlteration, value); }
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
        public bool HasCharacterClassRequirement
        {
            get { return _hasCharacterClassRequirement; }
            set { this.RaiseAndSetIfChanged(ref _hasCharacterClassRequirement, value); }
        }
        public string CharacterClass
        {
            get { return _characterClass; }
            set { this.RaiseAndSetIfChanged(ref _characterClass, value); }
        }

        public DoodadTemplateViewModel()
        {
            this.IsUnique = false;
            this.IsOneUse = false;
            this.HasCharacterClassRequirement = false;
            this.AutomaticAlteration = new DoodadAlterationTemplateViewModel();
            this.InvokedAlteration = new DoodadAlterationTemplateViewModel();
        }
    }
}

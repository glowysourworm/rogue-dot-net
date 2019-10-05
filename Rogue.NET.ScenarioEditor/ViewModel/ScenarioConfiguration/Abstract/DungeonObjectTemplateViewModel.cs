using System;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract
{
    public class DungeonObjectTemplateViewModel : TemplateViewModel
    {
        private SymbolDetailsTemplateViewModel _symbolDetails;
        private string _shortDescription;
        private string _longDescription;
        private bool _isCursed;
        private bool _isUnique;
        private bool _isObjectiveItem;
        private bool _hasBeenGenerated;

        public SymbolDetailsTemplateViewModel SymbolDetails
        {
            get { return _symbolDetails; }
            set { this.RaiseAndSetIfChanged(ref _symbolDetails, value); }
        }
        public string ShortDescription
        {
            get { return _shortDescription; }
            set { this.RaiseAndSetIfChanged(ref _shortDescription, value); }
        }
        public string LongDescription
        {
            get { return _longDescription; }
            set { this.RaiseAndSetIfChanged(ref _longDescription, value); }
        }
        public bool IsCursed
        {
            get { return _isCursed; }
            set { this.RaiseAndSetIfChanged(ref _isCursed, value); }
        }
        public bool IsUnique
        {
            get { return _isUnique; }
            set { this.RaiseAndSetIfChanged(ref _isUnique, value); }
        }
        public bool IsObjectiveItem
        {
            get { return _isObjectiveItem; }
            set { this.RaiseAndSetIfChanged(ref _isObjectiveItem, value); }
        }
        public bool HasBeenGenerated
        {
            get { return _hasBeenGenerated; }
            set { this.RaiseAndSetIfChanged(ref _hasBeenGenerated, value); }
        }

        public DungeonObjectTemplateViewModel()
        {
            this.SymbolDetails = new SymbolDetailsTemplateViewModel();

            this.ShortDescription = "";
            this.LongDescription = "";
        }
        public DungeonObjectTemplateViewModel(DungeonObjectTemplateViewModel tmp)
        {
            this.SymbolDetails = tmp.SymbolDetails;
            this.ShortDescription = tmp.ShortDescription;
            this.LongDescription = tmp.LongDescription;
            this.IsCursed = tmp.IsCursed;
            this.IsUnique = tmp.IsUnique;
            this.IsObjectiveItem = tmp.IsObjectiveItem;
        }
    }
}

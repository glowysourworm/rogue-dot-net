using Rogue.NET.Common.ViewModel;
using Rogue.NET.ScenarioEditor.ViewModel.Constant;
using Rogue.NET.ScenarioEditor.ViewModel.Difficulty.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using System.Windows.Input;

namespace Rogue.NET.ScenarioEditor.ViewModel.Difficulty
{
    public class DifficultyAssetViewModel : NotifyViewModel, IDifficultyAssetViewModel
    {
        string _id;
        string _name;
        int _requiredLevel;
        bool _included;

        SymbolDetailsTemplateViewModel _symbolDetailsViewModel;

        public string Id
        {
            get { return _id; }
            protected set { this.RaiseAndSetIfChanged(ref _id, value); }
        }

        public string Name
        {
            get { return _name; }
            set { this.RaiseAndSetIfChanged(ref _name, value); }
        }
        public int RequiredLevel
        {
            get { return _requiredLevel; }
            set { this.RaiseAndSetIfChanged(ref _requiredLevel, value); }
        }
        public bool Included
        {
            get { return _included; }
            set { this.RaiseAndSetIfChanged(ref _included, value); }
        }
        public SymbolDetailsTemplateViewModel SymbolDetails
        {
            get { return _symbolDetailsViewModel; }
            set { this.RaiseAndSetIfChanged(ref _symbolDetailsViewModel, value); }
        }
        public ICommand CalculateCommand { get; set; }

        public DifficultyAssetViewModel(DungeonObjectTemplateViewModel viewModel)
        {
            this.Id = viewModel.Guid;
            this.Included = true;
            this.Name = viewModel.Name;
            this.RequiredLevel = AssetType.GetRequiredLevel(viewModel);

            this.SymbolDetails = new SymbolDetailsTemplateViewModel()
            {
                CharacterColor = viewModel.SymbolDetails.CharacterColor,
                CharacterSymbol = viewModel.SymbolDetails.CharacterSymbol,
                Icon = viewModel.SymbolDetails.Icon,
                SmileyAuraColor = viewModel.SymbolDetails.SmileyAuraColor,
                SmileyBodyColor = viewModel.SymbolDetails.SmileyBodyColor,
                SmileyLineColor = viewModel.SymbolDetails.SmileyLineColor,
                SmileyMood = viewModel.SymbolDetails.SmileyMood,
                Type = viewModel.SymbolDetails.Type
            };
        }
        public DifficultyAssetViewModel(TemplateViewModel viewModel)
        {
            this.Id = viewModel.Guid;
            this.Included = true;
            this.Name = viewModel.Name;
            this.RequiredLevel = 0;
        }
    }
}

using Rogue.NET.Common.ViewModel;
using Rogue.NET.ScenarioEditor.ViewModel.Difficulty.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Rogue.NET.ScenarioEditor.ViewModel.Difficulty
{
    public class DifficultyAssetGroupViewModel : NotifyViewModel, IDifficultyAssetGroupViewModel
    {
        string _assetType;

        public ObservableCollection<IDifficultyAssetViewModel> Assets { get; set; }

        public string AssetType
        {
            get { return _assetType; }
            set { this.RaiseAndSetIfChanged(ref _assetType, value); }
        }

        public DifficultyAssetGroupViewModel(string assetType, IEnumerable<TemplateViewModel> assetCollection)
        {
            this.AssetType = assetType;

            this.Assets = new ObservableCollection<IDifficultyAssetViewModel>();

            if (Constant.AssetType.HasSymbol(assetType))
            {
                foreach (var item in assetCollection.Cast<DungeonObjectTemplateViewModel>())
                    this.Assets.Add(new DifficultyAssetViewModel(item));
            }
            else
            {
                foreach (var item in assetCollection)
                    this.Assets.Add(new DifficultyAssetViewModel(item));
            }
        }
    }
}

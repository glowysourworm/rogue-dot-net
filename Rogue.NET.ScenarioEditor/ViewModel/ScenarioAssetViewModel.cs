using Prism.Commands;
using Prism.Events;
using Rogue.NET.Common.ViewModel;
using Rogue.NET.ScenarioEditor.ViewModel.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using System.ComponentModel.Composition;
using System.Windows.Input;

namespace Rogue.NET.ScenarioEditor.ViewModel
{
    public class ScenarioAssetViewModel : NotifyViewModel, IScenarioAssetViewModel
    {
        readonly IEventAggregator _eventAggregator;

        bool _isSelected = false;
        string _name = "";
        string _type = "";
        SymbolDetailsTemplateViewModel _symbolDetailsViewModel;

        [ImportingConstructor]
        public ScenarioAssetViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            this.LoadAssetCommand = new DelegateCommand(() =>
            {
                // TODO: Use Event Bubble

                this.IsSelected = true;
            });
            this.RemoveAssetCommand = new DelegateCommand(() =>
            {
                // TODO: Use Event to Bubble up

                // notify group to remove this asset from list
                // _group.RemoveAsset(this);
            });
        }


        #region (public) Properties
        public string Type
        {
            get { return _type; }
            set { this.RaiseAndSetIfChanged(ref _type, value); }
        }
        public string Name 
        {
            get { return _name; }
            set { this.RaiseAndSetIfChanged(ref _name, value); }
        }
        public bool IsSelected 
        {
            get { return _isSelected; }
            set { this.RaiseAndSetIfChanged(ref _isSelected, value); }
        }
        public SymbolDetailsTemplateViewModel SymbolDetails
        {
            get { return _symbolDetailsViewModel; }
            set { this.RaiseAndSetIfChanged(ref _symbolDetailsViewModel, value); }
        }

        public ICommand RemoveAssetCommand { get; set; }
        public ICommand LoadAssetCommand { get; set; }
        #endregion
    }
}

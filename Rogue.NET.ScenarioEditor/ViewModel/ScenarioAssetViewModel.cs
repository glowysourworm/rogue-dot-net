using Prism.Commands;
using Rogue.NET.Common.ViewModel;
using Rogue.NET.ScenarioEditor.ViewModel.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using System;
using System.ComponentModel.Composition;
using System.Windows.Input;

namespace Rogue.NET.ScenarioEditor.ViewModel
{
    public class ScenarioAssetViewModel : NotifyViewModel, IScenarioAssetViewModel
    {
        bool _isSelected = false;
        string _name = "";
        string _type = "";
        string _subType = "";
        SymbolDetailsTemplateViewModel _symbolDetailsViewModel;

        [ImportingConstructor]
        public ScenarioAssetViewModel()
        {
            this.LoadAssetCommand = new DelegateCommand(() =>
            {
                this.IsSelected = true;

                if (this.LoadAssetEvent != null)
                    LoadAssetEvent(this, this);
            });
            this.RemoveAssetCommand = new DelegateCommand(() =>
            {
                if (this.RemoveAssetEvent != null)
                    RemoveAssetEvent(this, this);
            });
            this.RenameAssetCommand = new DelegateCommand(() =>
            {
                if (this.RenameAssetEvent != null)
                    RenameAssetEvent(this, this);
            });
            this.CopyAssetCommand = new DelegateCommand(() =>
            {
                if (this.CopyAssetEvent != null)
                    CopyAssetEvent(this, this);
            });
        }


        #region (public) Properties
        public string Type
        {
            get { return _type; }
            set { this.RaiseAndSetIfChanged(ref _type, value); }
        }
        public string SubType
        {
            get { return _subType; }
            set { this.RaiseAndSetIfChanged(ref _subType, value); }
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
        public ICommand CopyAssetCommand { get; set; }
        public ICommand RenameAssetCommand { get; set; }

        public event EventHandler<IScenarioAssetViewModel> RemoveAssetEvent;
        public event EventHandler<IScenarioAssetViewModel> LoadAssetEvent;
        public event EventHandler<IScenarioAssetViewModel> CopyAssetEvent;
        public event EventHandler<IScenarioAssetViewModel> RenameAssetEvent;
        #endregion
    }
}

using Prism.Commands;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System.ComponentModel;
using System.Windows.Input;

namespace Rogue.NET.ScenarioEditor.ViewModel
{
    public interface IScenarioAssetViewModel
    {
        string Name { get; set; }
        string Type { get; set; }
        SymbolDetailsTemplate SymbolDetails { get; set; }
        bool IsSelected { get; set; }

        ICommand RemoveAssetCommand { get; }
        ICommand LoadAssetCommand { get; }
    }
    public class ScenarioAssetViewModel : IScenarioAssetViewModel, INotifyPropertyChanged
    {
        readonly IScenarioEditorController _controller;
        readonly IScenarioAssetGroupViewModel _group;

        bool _isSelected = false;
        string _name = "";

        public string Type { get; set; }
        public string Name 
        {
            get { return _name; }
            set
            {
                if (string.IsNullOrEmpty(this.Type))
                    _name = value;

                else if (_controller.UpdateAssetName(_name, value, this.Type))
                {
                    _name = value;
                    OnPropertyChanged("Name");
                }
            }
        }
        public bool IsSelected 
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("IsSelected"));
            }
        }
        public SymbolDetailsTemplate SymbolDetails { get; set; }

        public ICommand RemoveAssetCommand
        {
            get
            {
                return new DelegateCommand<string>((name) =>
                {
                    _controller.RemoveAsset(this.Type, name);

                    // notify group to remove this asset from list
                    _group.RemoveAsset(this);
                });
            }
        }
        public ICommand LoadAssetCommand
        {
            get
            {
                return new DelegateCommand<string>((name) =>
                {
                    _controller.LoadAsset(this.Type, name);

                    this.IsSelected = true;
                });
            }
        }

        public ScenarioAssetViewModel(
            IScenarioAssetGroupViewModel group,
            IScenarioEditorController controller)
        {
            _controller = controller;
            _group = group;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}

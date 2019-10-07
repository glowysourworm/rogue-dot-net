using Rogue.NET.Common.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.ScenarioEditor.ViewModel.Attribute
{
    /// <summary>
    /// View model to attach UITypeAttribute data to the a view
    /// </summary>
    public class UITypeAttributeViewModel : NotifyViewModel
    {
        string _displayName;
        string _description;
        Type _viewModelType;
        Type _viewType;
        UITypeAttributeBaseType _baseType;

        public string DisplayName
        {
            get { return _displayName; }
            set { this.RaiseAndSetIfChanged(ref _displayName, value); }
        }
        public string Description
        {
            get { return _description; }
            set { this.RaiseAndSetIfChanged(ref _description, value); }
        }
        public Type ViewModelType
        {
            get { return _viewModelType; }
            set { this.RaiseAndSetIfChanged(ref _viewModelType, value); }
        }
        public Type ViewType
        {
            get { return _viewType; }
            set { this.RaiseAndSetIfChanged(ref _viewType, value); }
        }
        public UITypeAttributeBaseType BaseType
        {
            get { return _baseType; }
            set { this.RaiseAndSetIfChanged(ref _baseType, value); }
        }

        public UITypeAttributeViewModel()
        {

        }
    }
}

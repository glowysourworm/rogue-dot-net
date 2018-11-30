using Rogue.NET.Common.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Scenario.Content.ViewModel.Message
{
    public class AttributeChangeViewModel : NotifyViewModel
    {
        string _attributeName;
        double _change;

        public string AttributeName
        {
            get { return _attributeName; }
            set { this.RaiseAndSetIfChanged(ref _attributeName, value); }
        }
        public double Change
        {
            get { return _change; }
            set { this.RaiseAndSetIfChanged(ref _change, value); }
        }
    }
}

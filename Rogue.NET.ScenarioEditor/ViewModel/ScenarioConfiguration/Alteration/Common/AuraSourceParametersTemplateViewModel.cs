using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Common
{
    public class AuraSourceParametersTemplateViewModel : TemplateViewModel
    {
        string _auraColor;
        int _auraRange;

        public string AuraColor
        {
            get { return _auraColor; }
            set { this.RaiseAndSetIfChanged(ref _auraColor, value); }
        }
        public int AuraRange
        {
            get { return _auraRange; }
            set { this.RaiseAndSetIfChanged(ref _auraRange, value); }
        }

        public AuraSourceParametersTemplateViewModel() { }
    }
}

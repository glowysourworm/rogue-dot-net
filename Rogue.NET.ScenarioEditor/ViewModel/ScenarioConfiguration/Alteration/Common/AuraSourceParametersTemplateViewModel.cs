using Rogue.NET.Core.Media.SymbolEffect.Utility;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

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

        public AuraSourceParametersTemplateViewModel()
        {
            this.AuraColor = ColorOperations.ConvertBack(Colors.White);
            this.AuraRange = 2;
        }
    }
}

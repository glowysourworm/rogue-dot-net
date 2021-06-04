using Rogue.NET.Core.Media.SymbolEffect.Utility;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System;
using System.Windows.Media;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common
{
    [Serializable]
    public class AuraSourceParametersTemplate : Template
    {
        string _auraColor;
        int _auraRange;

        public string AuraColor
        {
            get { return _auraColor; }
            set
            {
                if (_auraColor != value)
                {
                    _auraColor = value;
                    OnPropertyChanged("AuraColor");
                }
            }
        }
        public int AuraRange
        {
            get { return _auraRange; }
            set
            {
                if (_auraRange != value)
                {
                    _auraRange = value;
                    OnPropertyChanged("AuraRange");
                }
            }
        }

        public AuraSourceParametersTemplate()
        {
            this.AuraColor = ColorOperations.ConvertBack(Colors.White);
            this.AuraRange = 2;
        }
    }
}

using LiveCharts;
using LiveCharts.Wpf;
using System.ComponentModel;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Controls
{
    public partial class ScenarioDifficultyChartTooltip : UserControl, IChartTooltip
    {
        TooltipData _data;
        TooltipSelectionMode? _selectionMode = TooltipSelectionMode.Auto;

        public TooltipData Data
        {
            get { return _data; }
            set
            {
                if (_data != value)
                {
                    _data = value;
                    if (this.PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("Data"));
                }
            }
        }
        public TooltipSelectionMode? SelectionMode { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public ScenarioDifficultyChartTooltip()
        {
            InitializeComponent();

            this.DataContext = this;
        }
    }
}

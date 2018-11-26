using Prism.Events;
using Rogue.NET.Common.Events.Splash;
using Rogue.NET.Common.ViewModel;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;

namespace Rogue.NET.ViewModel
{
    [Export]
    public class CreatingScenarioViewModel : NotifyViewModel
    {
        Color _bodyColor = Colors.Yellow;
        Color _lineColor = Colors.Black;

        string _message = "";
        double _progress = 0;

        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                OnPropertyChanged("Message");
            }
        }
        public double Progress
        {
            get { return _progress; }
            set
            {
                _progress = value;
                OnPropertyChanged("Progress");
            }
        }

        public Color SmileyBodyColor 
        {
            get { return _bodyColor; }
            set
            {
                _bodyColor = value;
                OnPropertyChanged("SmileyBodyColor");
            }
        }
        public Color SmileyLineColor 
        {
            get { return _lineColor; }
            set
            {
                _lineColor = value;
                OnPropertyChanged("SmileyLineColor");
            }
        }

        [ImportingConstructor]
        public CreatingScenarioViewModel(IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<CreatingScenarioEvent>().Subscribe(Update);
        }

        private async Task Update(CreatingScenarioEventArgs e)
        {
            this.Message = e.Message;
            this.Progress = e.Progress;

            if (e.SmileyBodyColor != Colors.Transparent)
                this.SmileyBodyColor = e.SmileyBodyColor;

            if (e.SmileyLineColor != Colors.Transparent)
                this.SmileyLineColor = e.SmileyLineColor;
        }
    }
}

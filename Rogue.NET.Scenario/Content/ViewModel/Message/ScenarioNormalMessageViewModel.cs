
namespace Rogue.NET.Scenario.Content.ViewModel.Message
{
    public class ScenarioNormalMessageViewModel : ScenarioMessageViewModel
    {
        string _message;

        public string Message
        {
            get { return _message; }
            set { this.RaiseAndSetIfChanged(ref _message, value); }
        }
    }
}

using Rogue.NET.Scenario.Content.ViewModel.Content.ScenarioMetaData;

namespace Rogue.NET.Scenario.Content.ViewModel.Message
{
    public class AttackAttributeHitViewModel : ScenarioImageViewModel
    {
        string _attackAttributeName;
        double _hit;

        public string AttackAttributeName
        {
            get { return _attackAttributeName; }
            set { this.RaiseAndSetIfChanged(ref _attackAttributeName, value); }
        }
        public double Hit
        {
            get { return _hit; }
            set { this.RaiseAndSetIfChanged(ref _hit, value); }
        }
    }
}

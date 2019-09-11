using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Scenario.Content.ViewModel.Message
{
    public class ScenarioCharacterAlterationMessageViewModel : ScenarioMessageViewModel
    {
        string _defenderName;
        string _attackerName;
        string _alterationName;

        public string AttackerName
        {
            get { return _attackerName; }
            set { this.RaiseAndSetIfChanged(ref _attackerName, value); }
        }

        public string DefenderName
        {
            get { return _defenderName; }
            set { this.RaiseAndSetIfChanged(ref _defenderName, value); }
        }

        public string AlterationName
        {
            get { return _alterationName; }
            set { this.RaiseAndSetIfChanged(ref _alterationName, value); }
        }
    }
}

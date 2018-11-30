using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Scenario.Content.ViewModel.Message
{
    public class ScenarioEnemyAlterationMessageViewModel : ScenarioMessageViewModel
    {
        string _enemyDisplayName;
        string _alterationDisplayName;

        public string EnemyDisplayName
        {
            get { return _enemyDisplayName; }
            set { this.RaiseAndSetIfChanged(ref _enemyDisplayName, value); }
        }
        public string AlterationDisplayName
        {
            get { return _alterationDisplayName; }
            set { this.RaiseAndSetIfChanged(ref _alterationDisplayName, value); }
        }
    }
}

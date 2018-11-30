using Rogue.NET.Scenario.Content.ViewModel.Content;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Scenario.Content.ViewModel.Message
{
    public class ScenarioAlterationMessageViewModel : ScenarioMessageViewModel
    {
        string _alterationDisplayName;
        string _effectedAttributeName;
        bool _isCausedByAttackAttributes;
        double _effect;

        public string AlterationDisplayName
        {
            get { return _alterationDisplayName; }
            set { this.RaiseAndSetIfChanged(ref _alterationDisplayName, value); }
        }
        public string EffectedAttributeName
        {
            get { return _effectedAttributeName; }
            set { this.RaiseAndSetIfChanged(ref _effectedAttributeName, value); }
        }
        public bool IsCausedByAttackAttributes
        {
            get { return _isCausedByAttackAttributes; }
            set { this.RaiseAndSetIfChanged(ref _isCausedByAttackAttributes, value); }
        }
        public double Effect
        {
            get { return _effect; }
            set { this.RaiseAndSetIfChanged(ref _effect, value); }
        }
        public ObservableCollection<AttackAttributeHitViewModel> AttackAttributeHits { get; set; }

        public ScenarioAlterationMessageViewModel()
        {
            this.AttackAttributeHits = new ObservableCollection<AttackAttributeHitViewModel>();
        }
    }
}

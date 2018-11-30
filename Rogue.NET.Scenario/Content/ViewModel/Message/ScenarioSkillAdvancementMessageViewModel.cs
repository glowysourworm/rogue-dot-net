using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Scenario.Content.ViewModel.Message
{
    public class ScenarioSkillAdvancementMessageViewModel : ScenarioMessageViewModel
    {
        string _skillDisplayName;
        int _skillLevel;

        public string SkillDisplayName
        {
            get { return _skillDisplayName; }
            set { this.RaiseAndSetIfChanged(ref _skillDisplayName, value); }
        }
        public int SkillLevel
        {
            get { return _skillLevel; }
            set { this.RaiseAndSetIfChanged(ref _skillLevel, value); }
        }
    }
}

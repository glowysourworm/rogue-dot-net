using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core.Model.Scenario.Content.Skill;

namespace Rogue.NET.Scenario.Content.ViewModel.Content
{
    public class SkillSetViewModel : NotifyViewModel
    {
        string _id;
        string _rogueName;

        public string Id
        {
            get { return _id; }
            set { this.RaiseAndSetIfChanged(ref _id, value); }
        }
        public string RogueName
        {
            get { return _rogueName; }
            set { this.RaiseAndSetIfChanged(ref _rogueName, value); }
        }

        public SkillSetViewModel(SkillSet scenarioObject)
        {
            this.Id = scenarioObject.Id;
            this.RogueName = scenarioObject.RogueName;
        }
    }
}

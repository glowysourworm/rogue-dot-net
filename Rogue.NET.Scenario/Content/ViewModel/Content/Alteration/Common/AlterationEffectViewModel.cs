using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Scenario.Content.ViewModel.Attribute;

namespace Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Common
{
    public abstract class AlterationEffectViewModel : RogueBaseViewModel
    {
        string _displayType;

        public string DisplayType
        {
            get { return _displayType; }
            set { this.RaiseAndSetIfChanged(ref _displayType, value); }
        }

        public AlterationEffectViewModel(RogueBase rogueBase) : base(rogueBase)
        {
            this.DisplayType = this.GetAttribute<UIDisplayAttribute>().Name;
        }

        public AlterationEffectViewModel(Template template) : base(template)
        {
            this.DisplayType = this.GetAttribute<UIDisplayAttribute>().Name;
        }
    }
}

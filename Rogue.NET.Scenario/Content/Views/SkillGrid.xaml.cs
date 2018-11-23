using System.Windows;
using System.Windows.Controls;
using Rogue.NET.Common.Events.Scenario;
using Rogue.NET.Common.EventArgs;
using Prism.Events;
using Rogue.NET.Core.Model.Scenario.Content.Skill;
using Rogue.NET.Core.Model.Enums;
using System.ComponentModel.Composition;
using Rogue.NET.Scenario.Content.ViewModel.Content;

namespace Rogue.NET.Scenario.Views
{
    [Export]
    public partial class SkillGrid : UserControl
    {
        readonly IEventAggregator _eventAggregator;

        [ImportingConstructor]
        public SkillGrid(IEventAggregator eventAggregator, PlayerViewModel playerViewModel)
        {
            _eventAggregator = eventAggregator;

            this.DataContext = playerViewModel;

            InitializeComponent();
        }
    }
}

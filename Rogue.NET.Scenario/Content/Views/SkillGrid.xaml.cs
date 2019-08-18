using System.Windows.Controls;
using System.ComponentModel.Composition;
using Rogue.NET.Scenario.Content.ViewModel.Content;
using Rogue.NET.Common.Extension.Prism.EventAggregator;

namespace Rogue.NET.Scenario.Content.Views
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export]
    public partial class SkillGrid : UserControl
    {
        readonly IRogueEventAggregator _eventAggregator;

        [ImportingConstructor]
        public SkillGrid(IRogueEventAggregator eventAggregator, PlayerViewModel playerViewModel)
        {
            _eventAggregator = eventAggregator;

            this.DataContext = playerViewModel;

            InitializeComponent();
        }
    }
}

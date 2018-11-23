using Prism.Events;
using Rogue.NET.ScenarioEditor.Events;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.EnemyControl
{
    [Export]
    public partial class EnemyBehavior : UserControl
    {
        [ImportingConstructor]
        public EnemyBehavior(IEventAggregator eventAggregator)
        {
            InitializeComponent();

            eventAggregator.GetEvent<ScenarioLoadedEvent>().Subscribe((configuration) =>
            {
                this.PrimaryAttackSkillCB.ItemsSource = configuration.MagicSpells;
                this.SecondaryAttackSkillCB.ItemsSource = configuration.MagicSpells;
            });
        }
    }
}

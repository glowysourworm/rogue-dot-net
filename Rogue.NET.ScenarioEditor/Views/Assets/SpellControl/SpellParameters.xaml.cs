using Prism.Events;
using Rogue.NET.ScenarioEditor.Events;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.SpellControl
{
    [Export]
    public partial class SpellParameters : UserControl
    {
        [ImportingConstructor]
        public SpellParameters(IEventAggregator eventAggregator)
        {
            InitializeComponent();

            eventAggregator.GetEvent<ScenarioLoadedEvent>().Subscribe(configuration =>
            {
                this.CreateMonsterCB.ItemsSource = configuration.EnemyTemplates;
                this.RemediedSpellCB.ItemsSource = configuration.MagicSpells;
            });
        }
    }
}

using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.Events;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.ConsumableControl
{
    [Export]
    public partial class ConsumableParameters : UserControl
    {
        [ImportingConstructor]
        public ConsumableParameters(IRogueEventAggregator eventAggregator)
        {
            InitializeComponent();

            eventAggregator.GetEvent<ScenarioLoadedEvent>().Subscribe((configuration) =>
            {
                // TODO:ALTERATION
                //this.SpellCB.ItemsSource = configuration.MagicSpells;
                //this.ProjectileSpellCB.ItemsSource = configuration.MagicSpells;
                //this.AmmoSpellCB.ItemsSource = configuration.MagicSpells;
                this.LearnedSkillCB.ItemsSource = configuration.SkillTemplates;
                this.CharacterClassCB.ItemsSource = configuration.CharacterClasses;
            });
        }
    }
}

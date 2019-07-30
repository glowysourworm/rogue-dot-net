using Prism.Events;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.ScenarioEditor.Events;
using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.ConsumableControl
{
    [Export]
    public partial class ConsumableParameters : UserControl
    {
        [ImportingConstructor]
        public ConsumableParameters(IEventAggregator eventAggregator)
        {
            InitializeComponent();

            eventAggregator.GetEvent<ScenarioLoadedEvent>().Subscribe((configuration) =>
            {
                this.SpellCB.ItemsSource = configuration.MagicSpells;
                this.ProjectileSpellCB.ItemsSource = configuration.MagicSpells;
                this.AmmoSpellCB.ItemsSource = configuration.MagicSpells;
                this.LearnedSkillCB.ItemsSource = configuration.SkillTemplates;
                this.CharacterClassCB.ItemsSource = configuration.CharacterClasses;
            });
        }
    }
}

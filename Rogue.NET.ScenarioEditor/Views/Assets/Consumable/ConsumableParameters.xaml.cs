using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.ScenarioEditor.ViewModel;
using Rogue.NET.ScenarioEditor.Views.Controls;
using System;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.Consumable
{
    public partial class ConsumableParameters : UserControl, IWizardPage
    {
        IWizardViewModel _containerViewModel;

        public ConsumableParameters()
        {
            InitializeComponent();
        }

        public Type NextPage
        {
            get { return typeof(ConsumableMetadata); }
        }

        public void Inject(IWizardViewModel containerViewModel, object model)
        {
            _containerViewModel = containerViewModel;

            this.DataContext = model;

            var config = containerViewModel.SecondaryPayload as ScenarioConfigurationContainer;

            this.SpellCB.ItemsSource = config.MagicSpells;
            this.ProjectileSpellCB.ItemsSource = config.MagicSpells;
            this.AmmoSpellCB.ItemsSource = config.MagicSpells;
            this.LearnedSkillCB.ItemsSource = config.SkillTemplates;
        }
    }
}

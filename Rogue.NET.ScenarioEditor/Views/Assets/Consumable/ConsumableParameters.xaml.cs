using Rogue.NET.Model;
using Rogue.NET.ScenarioEditor.ViewModel;
using Rogue.NET.ScenarioEditor.Views.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

            var config = containerViewModel.SecondaryPayload as ScenarioConfiguration;

            this.SpellCB.ItemsSource = config.MagicSpells;
            this.ProjectileSpellCB.ItemsSource = config.MagicSpells;
            this.AmmoSpellCB.ItemsSource = config.MagicSpells;
            this.LearnedSkillCB.ItemsSource = config.SkillTemplates;
        }
    }
}

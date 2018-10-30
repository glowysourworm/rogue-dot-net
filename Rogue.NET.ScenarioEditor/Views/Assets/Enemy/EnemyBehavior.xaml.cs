using Rogue.NET.ScenarioEditor.ViewModel;
using Rogue.NET.ScenarioEditor.Views.Controls;
using System;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.Enemy
{
    /// <summary>
    /// Interaction logic for EnemyBehavior.xaml
    /// </summary>
    public partial class EnemyBehavior : UserControl, IWizardPage
    {
        IWizardViewModel _containerViewModel;

        public EnemyBehavior()
        {
            InitializeComponent();
        }

        public Type NextPage
        {
            get { return typeof(EnemyAttackAttributes); }
        }

        public void Inject(IWizardViewModel containerViewModel, object model)
        {
            _containerViewModel = containerViewModel;

            this.DataContext = model;
        }
    }
}

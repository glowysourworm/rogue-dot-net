using Rogue.NET.ScenarioEditor.ViewModel;
using Rogue.NET.ScenarioEditor.Views.Controls;
using System;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.Enemy
{
    public partial class EnemyMetadata : UserControl, IWizardPage
    {
        IWizardViewModel _containerViewModel;

        public EnemyMetadata()
        {
            InitializeComponent();
        }

        public Type NextPage
        {
            get { return typeof(EnemySymbol); }
        }

        public void Inject(IWizardViewModel containerViewModel, object model)
        {
            _containerViewModel = containerViewModel;

            this.DataContext = model;
        }
    }
}

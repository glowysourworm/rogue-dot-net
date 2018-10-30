using Rogue.NET.ScenarioEditor.ViewModel;
using Rogue.NET.ScenarioEditor.Views.Controls;
using System;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.Animation
{
    public partial class AnimationBlink : UserControl, IWizardPage
    {
        IWizardViewModel _containerViewModel;

        public AnimationBlink()
        {
            InitializeComponent();
        }

        public Type NextPage
        {
            get { return typeof(AnimationBlink); }
        }

        public void Inject(IWizardViewModel containerViewModel, object model)
        {
            _containerViewModel = containerViewModel;

            this.DataContext = model;
        }
    }
}

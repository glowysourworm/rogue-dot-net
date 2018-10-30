using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.ScenarioEditor.ViewModel;
using Rogue.NET.ScenarioEditor.Views.Controls;
using System;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.Animation
{
    public partial class AnimationParametersExtended : UserControl, IWizardPage
    {
        IWizardViewModel _containerViewModel;

        public AnimationParametersExtended()
        {
            InitializeComponent();
        }

        public Type NextPage
        {
            get { return typeof(AnimationPreview); }
        }
        public void Inject(IWizardViewModel containerViewModel, object model)
        {
            _containerViewModel = containerViewModel;

            this.DataContext = model;

            var config = containerViewModel.SecondaryPayload as ScenarioConfigurationContainer;
            if (config != null)
            {
                this.ColorCB.ItemsSource = config.BrushTemplates;
                this.OutlineCB.ItemsSource = config.BrushTemplates;
            }
        }
    }
}

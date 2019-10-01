using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Layout;
using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.DesignRegion
{
    [Export]
    public partial class AssetContainerControl : UserControl
    {
        [ImportingConstructor]
        public AssetContainerControl()
        {
            InitializeComponent();
        }
    }
}

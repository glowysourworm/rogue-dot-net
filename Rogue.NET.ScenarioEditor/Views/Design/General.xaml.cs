using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.Events.Asset;
using Rogue.NET.ScenarioEditor.Events.Browser;
using Rogue.NET.ScenarioEditor.Utility;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
using Rogue.NET.ScenarioEditor.Views.Controls.Symbol;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Design
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export]
    public partial class General : UserControl
    {
        public General()
        {
            InitializeComponent();
        }
    }
}

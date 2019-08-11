using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.SharedControl.SpellControl
{
    [Export]
    public partial class SpellType : UserControl
    {
        [ImportingConstructor]
        public SpellType()
        {
            InitializeComponent();
        }
    }
}

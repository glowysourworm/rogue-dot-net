using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.EquipmentControl
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export]
    public partial class EquipmentAlterationSelection : UserControl
    {
        [ImportingConstructor]
        public EquipmentAlterationSelection()
        {
            InitializeComponent();
        }
    }
}

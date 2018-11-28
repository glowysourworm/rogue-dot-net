using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.EnemyControl
{
    [Export]
    public partial class EnemySymbol : UserControl
    {
        [ImportingConstructor]
        public EnemySymbol()
        {
            InitializeComponent();
        }
    }
}

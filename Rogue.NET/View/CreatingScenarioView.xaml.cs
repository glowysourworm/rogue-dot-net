﻿using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.View
{
    [Export]
    public partial class CreatingScenarioView : UserControl
    {
        [ImportingConstructor]
        public CreatingScenarioView()
        {
            InitializeComponent();
        }
    }
}

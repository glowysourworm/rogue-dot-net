﻿using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.View
{
    [Export]
    public partial class OpenScenarioView : UserControl
    {
        public OpenScenarioView()
        {
            InitializeComponent();
        }
    }
}
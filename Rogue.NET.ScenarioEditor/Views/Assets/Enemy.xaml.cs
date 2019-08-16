﻿using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets
{
    [Export]
    public partial class Enemy : UserControl
    {
        public Enemy()
        {
            InitializeComponent();

            // Set symbol tab to be the default to show for the consumable
            this.Loaded += (sender, e) =>
            {
                this.DefaultTab.IsSelected = true;
            };
        }
    }
}

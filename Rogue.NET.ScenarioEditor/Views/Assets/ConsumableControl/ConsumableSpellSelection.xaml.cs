﻿using Rogue.NET.ScenarioEditor.ViewModel;
using Rogue.NET.ScenarioEditor.Views.Controls;
using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.ConsumableControl
{
    [Export]
    public partial class ConsumableSpellSelection : UserControl
    {
        [ImportingConstructor]
        public ConsumableSpellSelection()
        {
            InitializeComponent();
        }
    }
}

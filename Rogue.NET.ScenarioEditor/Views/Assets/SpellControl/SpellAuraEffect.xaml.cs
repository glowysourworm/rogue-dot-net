﻿using Prism.Events;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.SpellControl
{
    [Export]
    public partial class SpellAuraEffect : UserControl
    {
        [ImportingConstructor]
        public SpellAuraEffect()
        {
            InitializeComponent();
        }
    }
}
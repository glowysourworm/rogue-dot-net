﻿using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.SharedControl.AlterationControl.EffectControl
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export]
    public partial class DrainMeleeEffectParameters : UserControl
    {
        public DrainMeleeEffectParameters()
        {
            InitializeComponent();
        }
    }
}

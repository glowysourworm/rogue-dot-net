﻿using Rogue.NET.Scenario.Content.ViewModel.Content;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.Scenario.Content.Views
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export]
    public partial class StatsControl : UserControl
    {
        [ImportingConstructor]
        public StatsControl(PlayerViewModel playerViewModel)
        {
            this.DataContext = playerViewModel;

            InitializeComponent();
        }
    }
}

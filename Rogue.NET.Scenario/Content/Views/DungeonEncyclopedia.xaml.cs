using System;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel.Composition;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Scenario.Content.ViewModel.Content;

namespace Rogue.NET.Scenario.Views
{
    [Export]
    public partial class DungeonEncyclopedia : UserControl
    {
        [ImportingConstructor]
        public DungeonEncyclopedia(RogueEncyclopediaViewModel viewModel)
        {
            this.DataContext = viewModel;

            InitializeComponent();
        }
    }
}

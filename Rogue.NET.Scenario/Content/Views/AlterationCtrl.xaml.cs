﻿using Rogue.NET.Scenario.Content.ViewModel.Content;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Rogue.NET.Scenario.Content.Views
{
    [Export]
    public partial class AlterationCtrl : UserControl
    {
        [ImportingConstructor]
        public AlterationCtrl(PlayerViewModel viewModel)
        {
            this.DataContext = viewModel;

            InitializeComponent();
        }
    }
}
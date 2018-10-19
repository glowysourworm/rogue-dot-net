﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.Specialized;
using Rogue.NET.Common;
using Rogue.NET.Common.EventArgs;

namespace Rogue.NET.Scenario.Views
{
    public partial class StatusCtrl : UserControl
    {
        public StatusCtrl()
        {
            InitializeComponent();

            this.HPBar.BarColor1 = Colors.Red;
            this.MPBar.BarColor1 = Colors.Blue;
            this.ExperienceBar.BarColor1 = Colors.Cyan;
            this.HaulBar.BarColor1 = Colors.Goldenrod;
            this.HungerBar.BarColor1 = Colors.DarkGreen;
        }
    }
}

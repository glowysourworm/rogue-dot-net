﻿using Prism.Events;
using Rogue.NET.Common.Events.Scenario;
using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.View
{
    [Export]
    public partial class EnchantArmorView : UserControl
    {
        [ImportingConstructor]
        public EnchantArmorView()
        {
            InitializeComponent();
        }
    }
}
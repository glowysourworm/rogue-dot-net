﻿using Prism.Events;
using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.View
{
    [Export]
    public partial class ImbueArmorView : UserControl
    {
        [ImportingConstructor]
        public ImbueArmorView()
        {
            InitializeComponent();
        }
    }
}
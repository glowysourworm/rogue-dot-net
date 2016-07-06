﻿using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.Unity;
using Rogue.NET.Common;
using System;
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

namespace Rogue.NET.Splash.Views
{
    public partial class ImbueWindow : Window
    {
        public ImbueWindow()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(ImbueWindow_Loaded);
        }

        [InjectionConstructor]
        public ImbueWindow(IEventAggregator eventAggregator)
        {
            InitializeComponent();

            this.ItemGrid.EventAggregator = eventAggregator;

            this.Loaded += ImbueWindow_Loaded;
        }

        private void ImbueWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.ItemGrid.ActionSubmittedEvent -= ItemGrid_ActionSubmittedEvent;
            this.ItemGrid.ActionSubmittedEvent += ItemGrid_ActionSubmittedEvent;
        }

        void ItemGrid_ActionSubmittedEvent(object sender, EventArgs e)
        {
            this.DialogResult = true;
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}

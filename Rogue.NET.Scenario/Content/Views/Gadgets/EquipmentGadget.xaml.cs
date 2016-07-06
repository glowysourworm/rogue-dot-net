﻿using Microsoft.Practices.Prism.Events;
using Rogue.NET.Common.Events.Scenario;
using Rogue.NET.Model.Events;
using System;
using System.Collections.Generic;
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

namespace Rogue.NET.Scenario.Content.Views.Gadgets
{
    public partial class EquipmentGadget : UserControl
    {
        public EquipmentGadget()
        {
            InitializeComponent();
        }
        public void InitializeEvents(IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<LevelLoadedEvent>().Subscribe((evt) =>
            {
                this.DataContext = evt.Data;
            });
        }
    }
}

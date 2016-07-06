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
using System.ComponentModel;
using System.Collections.Specialized;
using Rogue.NET.Common.Collections;
using Rogue.NET.Common;
using Rogue.NET.Model.Scenario;
using Rogue.NET.Model;
using Microsoft.Practices.Prism.Events;

namespace Rogue.NET.Scenario.Views
{
    public partial class EquipmentCtrl : UserControl
    {
        readonly IEventAggregator _eventAggregator;

        public EquipmentCtrl()
        {
            InitializeComponent();
        }

        public EquipmentCtrl(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            InitializeComponent();
        }
    }
}

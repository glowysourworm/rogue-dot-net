using Prism.Events;
using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.View
{
    [Export]
    public partial class EnchantView : UserControl
    {
        [ImportingConstructor]
        public EnchantView(IEventAggregator eventAggregator)
        {
            InitializeComponent();
        }
    }
}

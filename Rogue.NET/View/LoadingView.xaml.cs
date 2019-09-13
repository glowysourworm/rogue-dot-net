using Rogue.NET.Core.Utility;
using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Controls;

namespace Rogue.NET.View
{
    [Export]
    public partial class LoadingView : UserControl
    {
        [ImportingConstructor]
        public LoadingView()
        {
            InitializeComponent();
        }
    }
}

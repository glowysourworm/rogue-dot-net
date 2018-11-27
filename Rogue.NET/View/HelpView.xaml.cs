using Rogue.NET.Core.Service.Interface;
using Rogue.NET.Scenario.Content.ViewModel.Content;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.View
{
    [Export]
    public partial class HelpView : UserControl
    {
        [ImportingConstructor]
        public HelpView()
        {
            InitializeComponent();
        }
    }
}

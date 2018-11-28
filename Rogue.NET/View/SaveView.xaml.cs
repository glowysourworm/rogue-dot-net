using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.View
{
    [Export]
    public partial class SaveView : UserControl
    {
        [ImportingConstructor]
        public SaveView()
        {
            InitializeComponent();
        }
    }
}

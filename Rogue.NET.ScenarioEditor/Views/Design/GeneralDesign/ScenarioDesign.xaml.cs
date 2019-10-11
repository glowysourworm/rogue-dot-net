using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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

namespace Rogue.NET.ScenarioEditor.Views.Design.GeneralDesign
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export]
    public partial class ScenarioDesign : UserControl
    {
        public ScenarioDesign()
        {
            InitializeComponent();
        }
    }
}

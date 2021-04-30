using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Media;

namespace Rogue.NET.ScenarioEditor.Views.Design.LevelBranchDesign
{
    [Export]
    public partial class LevelBranchDesigner : UserControl
    {
        [ImportingConstructor]
        public LevelBranchDesigner()
        {
            InitializeComponent();

            this.DataContextChanged += (sender, args) =>
            {
                if (args.NewValue is ImageSource)
                    this.PreviewImage.Source = (ImageSource)args.NewValue;
            };
        }
    }
}

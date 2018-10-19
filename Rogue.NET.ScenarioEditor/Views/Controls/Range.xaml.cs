using Rogue.NET.Common.Model;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Controls
{
    public partial class Range : UserControl
    {

        public Range()
        {
            InitializeComponent();

            this.DataContextChanged += Range_DataContextChanged;
        }

        private void Range_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is Range<int>)
            {
                this.LowUD.Increment = 1;
                this.HighUD.Increment = 1;

                this.LowUD.FormatString = "N0";
                this.HighUD.FormatString = "N0";

                /*
                var lowBE = this.LowText.GetBindingExpression(TextBlock.TextProperty);
                lowBE.ParentBinding.StringFormat = "N0";

                var highBE = this.HighText.GetBindingExpression(TextBlock.TextProperty);
                highBE.ParentBinding.StringFormat = "N0";
                 */
            }
            else
            {
                this.LowUD.Increment = 0.1;
                this.HighUD.Increment = 0.1;

                this.LowUD.FormatString = "F2";
                this.HighUD.FormatString = "F2";

                /*
                var lowBE = this.LowText.GetBindingExpression(TextBlock.TextProperty);
                lowBE.ParentBinding.StringFormat = "F2";

                var highBE = this.HighText.GetBindingExpression(TextBlock.TextProperty);
                highBE.ParentBinding.StringFormat = "F2";
                 */ 
            }
        }
    }
}

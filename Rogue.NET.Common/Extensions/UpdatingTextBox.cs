using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Rogue.NET.Common.Extensions
{
    public class UpdatingTextBox : TextBox
    {
        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);

            var binding = GetBindingExpression(TextBox.TextProperty);
            if (binding != null)
                binding.UpdateSource();
        }
    }
}

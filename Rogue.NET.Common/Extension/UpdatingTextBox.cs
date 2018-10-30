using System.Windows.Controls;

namespace Rogue.NET.Common.Extension
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

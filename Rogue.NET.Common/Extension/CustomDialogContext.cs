using Rogue.NET.Common.ViewModel;
using System.Windows;

namespace Rogue.NET.Common.Extension
{
    public class CustomDialogContext : NotifyViewModel
    {
        bool _isOpen;

        public bool IsOpen
        {
            get { return _isOpen; }
            set
            {
                this.RaiseAndSetIfChanged(ref _isOpen, value);

                // CONSIDER MOVING THIS - responsible for enabling the application
                Application.Current.MainWindow.IsEnabled = !value;
                Application.Current.MainWindow.Opacity = value ? 0.7 : 1;
            }
        }
    }
}

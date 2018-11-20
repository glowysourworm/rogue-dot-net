using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.View
{
    [Export]
    public partial class DialogView : UserControl
    {
        public DialogView()
        {
            InitializeComponent();

            this.DataContextChanged += Dialog_DataContextChanged;
        }

        void Dialog_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // TODO
            //var data = e.NewValue as LevelData;
            //if (data != null)
            //    _lb.ItemsSource = data.DialogMessages.
            //        Select(m => m.Timestamp.ToString() + "\t" + m.Message).
            //        Reverse().
            //        Take(20);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO
            //this.DialogResult = true;
        }
    }
}

using Rogue.NET.Common;
using Rogue.NET.Scenario;
using Rogue.NET.Online;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Rogue.NET.Model;

namespace Rogue.NET.Online
{
    public partial class OutroDisplay : UserControl
    {
        IModelController _loader = null;

        public OutroDisplay()
        {
            InitializeComponent();
        }
        public void Initialize(object[] args)
        {
            _loader = args[0] as IModelController;
            Dictionary<string, string> gameStatsDictionary = _loader.GetGameDisplayStats();

            //Create game stats
            this.GameStatsPanel.Children.Clear();
            foreach (KeyValuePair<string, string> kvp in gameStatsDictionary)
            {
                Grid g = new Grid();
                g.HorizontalAlignment = HorizontalAlignment.Stretch;
                g.Width = 700;

                TextBlock label = new TextBlock();
                label.Text = kvp.Key + ":";
                label.Foreground = Brushes.Turquoise;
                label.FontSize = 24;
                label.Margin = new Thickness(8, 8, 200, 8);
                label.HorizontalAlignment = HorizontalAlignment.Left;

                TextBlock val = new TextBlock();
                val.Text = kvp.Value;
                val.Foreground = Brushes.Silver;
                val.FontSize = 24;
                val.Margin = new Thickness(200, 8, 8, 8);
                val.HorizontalAlignment = HorizontalAlignment.Right;

                g.Children.Add(label);
                g.Children.Add(val);
                GameStatsPanel.Children.Add(g);
            }
        }

        //public event EventHandler<Rogue2DisplayFinishedEventArgs> FinishedEvent;
        //public event EventHandler<SplashScreenEventArgs> SplashScreenEvent;
        //public Rogue2Displays DisplayType
        //{
        //    get { return Rogue2Displays.Outro; }
        //}

        private void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            //Ask user to upload statistics
            Window credentialsWindow = new Window();
            credentialsWindow.SizeToContent = SizeToContent.WidthAndHeight;
            credentialsWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            credentialsWindow.Owner = Application.Current.MainWindow;
            credentialsWindow.Title = "Rogue.NET User Login - www.roguedotnet.com";
            Credentials ctrl = new Credentials();
            credentialsWindow.Content = ctrl;
            if ((bool)credentialsWindow.ShowDialog())
            {
                //Client module = new Client();
                //string xmlStats = _loader.GetGameStatsXml(ctrl.UserTB.Text, ctrl.PassTB.Password);
                //if (!module.UploadEncryptedMessage(xmlStats, ctrl.UserTB.Text, ctrl.PassTB.Password))
                //    MessageBox.Show("There was an error uploading your statistics - make sure you have an account at www.roguedotnet.com and that your user name and password are correct");
                //else
                //    FinishedEvent(this, new Rogue2DisplayFinishedEventArgs(Rogue2Displays.Intro, new object[] { }));
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            //FinishedEvent(this, new Rogue2DisplayFinishedEventArgs(Rogue2Displays.Intro, new object[] { }));
        }
    }
}

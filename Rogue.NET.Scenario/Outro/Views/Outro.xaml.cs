using Rogue.NET.Common;
using Rogue.NET.Scenario;
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

namespace Rogue.NET.Scenario.Outro
{
    public partial class OutroDisplay : UserControl
    {
        IModelController _modelController = null;

        public OutroDisplay()
        {
            InitializeComponent();
        }
        public void Initialize(object[] args)
        {
            _modelController = args[0] as IModelController;
            Dictionary<string, string> gameStatsDictionary = _modelController.GetGameDisplayStats();

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
    }
}

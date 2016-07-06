using Rogue.NET.Model;
using Rogue.NET.Model.Scenario;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Rogue.NET.Splash.Views
{
    public partial class ObjectiveWindow : Window
    {
        public ObjectiveWindow()
        {
            InitializeComponent();

            this.ObjectivePanel.SeekToNearestBehavior = false;
            this.ObjectivePanel.FontSize = 16;
            this.Loaded += ObjectiveWindow_Loaded;
        }

        void ObjectiveWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var levelData = this.DataContext as LevelData;
            if (levelData != null)
            {
                //Add objectives to panel
                this.ObjectivePanel.ClearElements();
                IEnumerable<ScenarioMetaData> objectives = levelData.Encyclopedia.Values.Where(z => z.IsObjective);
                foreach (var meta in objectives)
                {
                    Image img = new Image();
                    img.Name = meta.RogueName;
                    img.Source = meta.SymbolInfo.SymbolImageSource;
                    img.Width = 20;
                    img.Height = 30;
                    this.ObjectivePanel.Children.Add(img);
                }

                this.ObjectivePanel.InitializeElements();
            }
        }
        private void ObjectivePanel_ObjectCentered(object sender, EventArgs e)
        {
            Image view = sender as Image;
            if (view != null)
                this.ObjectivePanel.CenterText = view.Name;
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}

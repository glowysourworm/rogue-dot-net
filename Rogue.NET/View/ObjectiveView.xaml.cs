using Prism.Events;
using Rogue.NET.Core.Graveyard;
using Rogue.NET.Core.Model.Scenario;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.View
{
    [Export]
    public partial class ObjectiveView : UserControl
    {
        readonly IEventAggregator _eventAggregator;

        [ImportingConstructor]
        public ObjectiveView(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

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
                    // TODO
                    //img.Source = meta.SymbolInfo.SymbolImageSource;
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
            // TODO: EventAggregator call to close window
        }
    }
}

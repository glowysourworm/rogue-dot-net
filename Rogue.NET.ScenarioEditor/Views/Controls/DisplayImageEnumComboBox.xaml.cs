using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Practices.ServiceLocation;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Service.Interface;

namespace Rogue.NET.ScenarioEditor.Views.Controls
{
    [Export]
    public partial class DisplayImageEnumComboBox : UserControl
    {
        readonly IScenarioResourceService _scenarioResourceService;

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(DisplayImageResources), typeof(DisplayImageEnumComboBox), 
            new PropertyMetadata(OnValueChanged));

        public DisplayImageResources Value
        {
            get { return (DisplayImageResources)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        private class ImageItem
        {
            public BitmapSource ImageSrc { get; set; }
            public string Text { get; set; }
            public DisplayImageResources ImgRes { get; set; }
            public ImageItem(BitmapSource src, string text, DisplayImageResources res)
            {
                this.ImageSrc = src;
                this.Text = text;
                this.ImgRes = res;
            }
        }
        [ImportingConstructor]
        public DisplayImageEnumComboBox()
        {
            _scenarioResourceService = ServiceLocator.Current.GetInstance<IScenarioResourceService>();

            InitializeComponent();
            LoadComboBox();
        }
        private void LoadComboBox()
        {
            Array resources = Enum.GetValues(typeof(DisplayImageResources));
            List<ImageItem> list = new List<ImageItem>();
            foreach (DisplayImageResources r in resources)
            {
                ImageItem item = new ImageItem(_scenarioResourceService.GetImageSource(new ScenarioImage("", r), 1.0), r.ToString(), r);
                list.Add(item);
            }
            this.TheComboBox.ItemsSource = list;
        }
        private static void OnValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DisplayImageEnumComboBox cb = o as DisplayImageEnumComboBox;
            if (cb.TheComboBox != null)
            {
                List<ImageItem> list = new List<ImageItem>(cb.TheComboBox.ItemsSource.Cast<ImageItem>());
                if (list.Any(z => z.ImgRes == (DisplayImageResources)e.NewValue))
                    cb.TheComboBox.SelectedItem = list.First(z => z.ImgRes == (DisplayImageResources)e.NewValue);
            }
        }
        private void TheComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
                this.Value = ((ImageItem)e.AddedItems[0]).ImgRes;
        }
    }
}

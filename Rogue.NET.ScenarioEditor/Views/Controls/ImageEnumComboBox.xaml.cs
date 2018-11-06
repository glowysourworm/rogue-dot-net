using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Practices.ServiceLocation;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Service.Interface;

namespace Rogue.NET.ScenarioEditor.Views.Controls
{
    public partial class ImageEnumComboBox : UserControl
    {
        readonly IScenarioResourceService _scenarioResourceService;

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(ImageResources), typeof(ImageEnumComboBox), 
            new PropertyMetadata(OnValueChanged));

        public ImageResources Value
        {
            get { return (ImageResources)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        private class ImageItem
        {
            public BitmapSource ImageSrc { get; set; }
            public string Text { get; set; }
            public ImageResources ImgRes { get; set; }
            public ImageItem(BitmapSource src, string text, ImageResources res)
            {
                this.ImageSrc = src;
                this.Text = text;
                this.ImgRes = res;
            }
        }
        public ImageEnumComboBox()
        {
            _scenarioResourceService = ServiceLocator.Current.GetInstance<IScenarioResourceService>();

            InitializeComponent();
            LoadComboBox();
        }
        private void LoadComboBox()
        {
            Array resources = Enum.GetValues(typeof(ImageResources));
            List<ImageItem> list = new List<ImageItem>();
            foreach (ImageResources r in resources)
            {
                ImageItem item = new ImageItem((BitmapSource)_scenarioResourceService.GetImage(r), r.ToString(), r);
                list.Add(item);
            }
            this.TheComboBox.ItemsSource = list;
        }
        private static void OnValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ImageEnumComboBox cb = o as ImageEnumComboBox;
            if (cb.TheComboBox != null)
            {
                List<ImageItem> list = new List<ImageItem>(cb.TheComboBox.ItemsSource.Cast<ImageItem>());
                if (list.Any(z => z.ImgRes == (ImageResources)e.NewValue))
                    cb.TheComboBox.SelectedItem = list.First(z => z.ImgRes == (ImageResources)e.NewValue);
            }
        }
        private void TheComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
                this.Value = ((ImageItem)e.AddedItems[0]).ImgRes;
        }
    }
}

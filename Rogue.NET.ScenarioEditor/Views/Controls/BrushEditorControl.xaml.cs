﻿using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Rogue.NET.ScenarioEditor.Views.Controls
{
    public partial class BrushEditorControl : UserControl
    {
        public BrushEditorControl()
        {
            InitializeComponent();

            this.DataContextChanged += BrushEditorControl_DataContextChanged;
        }

        private void BrushEditorControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var oldTemplate = e.OldValue as BrushTemplateViewModel;
            var newTemplate = e.NewValue as BrushTemplateViewModel;

            if (oldTemplate != null)
            {
                oldTemplate.PropertyChanged -= (obj, args) => OnBrushChanged();
                oldTemplate.GradientStops.CollectionChanged -= (obj, args) => OnBrushChanged();
            }

            if (newTemplate != null)
            {
                newTemplate.PropertyChanged += (obj, args) => OnBrushChanged();
                newTemplate.GradientStops.CollectionChanged += (obj, args) => OnBrushChanged();
            }
        }

        private void OnBrushChanged()
        {
            var template = this.DataContext as BrushTemplateViewModel;

            this.PreviewCanvas.Background = template.GenerateBrush();
        }

        private void AddGradientStopButton_Click(object sender, RoutedEventArgs e)
        {
            var template = this.DataContext as BrushTemplateViewModel;
            if (template == null)
                return;

            template.GradientStops.Add(new GradientStopTemplateViewModel());
        }
        private void RemoveGradientStopButton_Click(object sender, RoutedEventArgs e)
        {
            var template = this.DataContext as BrushTemplateViewModel;
            if (template == null)
                return;

            for (int i = this.GradientStopListBox.SelectedItems.Count;i >= 0;i--)
            {
                var item = this.GradientStopListBox.SelectedItems[i] as GradientStopTemplateViewModel;

                template.GradientStops.Remove(item);
            }
        }
    }
}

using Rogue.NET.Common.Extension;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Rogue.NET.ScenarioEditor.Views.Controls.Symbol
{
    public partial class SymbolPreviewControl : UserControl
    {
        public static readonly DependencyProperty BorderProperty =
            DependencyProperty.Register("Border", typeof(Brush), typeof(SymbolPreviewControl), new PropertyMetadata(Brushes.Transparent));

        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register("Scale", typeof(double), typeof(SymbolPreviewControl), new PropertyMetadata(2.0, new PropertyChangedCallback(OnScaleChanged)));

        public Brush Border
        {
            get { return (Brush)GetValue(BorderProperty); }
            set { SetValue(BorderProperty, value); }
        }

        public double Scale
        {
            get { return (double)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }

        public SymbolPreviewControl()
        {
            InitializeComponent();
        }

        private static void OnScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as SymbolPreviewControl;

            if (control != null &&
                e.NewValue != null)
            {
                control.ImageMultiBinding.ConverterParameter = ((double)e.NewValue).Clip(1, 3);
            }
        }
    }
}

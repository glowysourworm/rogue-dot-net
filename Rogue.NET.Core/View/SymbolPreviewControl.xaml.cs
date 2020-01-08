using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Converter;
using Rogue.NET.Core.Model;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Rogue.NET.Core.View
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
                // Have to re-instantiate the multi binding to set the converter parameter
                //
                var multiBinding = new MultiBinding()
                {
                    Converter = new SymbolImageSourceConverter(),
                    ConverterParameter = ((double)e.NewValue).Clip(ModelConstants.Settings.ZoomMin, ModelConstants.Settings.ZoomMax),
                };

                multiBinding.Bindings.Add(new Binding("CharacterColor"));
                multiBinding.Bindings.Add(new Binding("CharacterSymbol"));
                multiBinding.Bindings.Add(new Binding("CharacterSymbolCategory"));
                multiBinding.Bindings.Add(new Binding("CharacterScale"));
                multiBinding.Bindings.Add(new Binding("Symbol"));
                multiBinding.Bindings.Add(new Binding("SymbolHue"));
                multiBinding.Bindings.Add(new Binding("SymbolSaturation"));
                multiBinding.Bindings.Add(new Binding("SymbolLightness"));
                multiBinding.Bindings.Add(new Binding("SymbolScale"));
                multiBinding.Bindings.Add(new Binding("SymbolUseColorMask"));
                multiBinding.Bindings.Add(new Binding("SmileyExpression"));
                multiBinding.Bindings.Add(new Binding("SmileyBodyColor"));
                multiBinding.Bindings.Add(new Binding("SmileyLineColor"));
                multiBinding.Bindings.Add(new Binding("GameSymbol"));
                multiBinding.Bindings.Add(new Binding("SymbolType"));

                control.Image.SetBinding(Image.SourceProperty, multiBinding);
            }
        }
    }
}

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using Rogue.NET.Core.Model.Enums;

namespace Rogue.NET.Core.View
{
    public partial class Smiley : UserControl
    {
        public static readonly DependencyProperty SmileyRadiusProperty;
        public static readonly DependencyProperty SmileyColorProperty;
        public static readonly DependencyProperty SmileyLineColorProperty;
        public static readonly DependencyProperty SmileyMoodProperty;
        public static readonly DependencyProperty SmileyLineThicknessProperty;
        public static readonly DependencyProperty SmileyLineBlurProperty;

        public double SmileyRadius
        {
            get { return (double)GetValue(Smiley.SmileyRadiusProperty); }
            set { SetValue(Smiley.SmileyRadiusProperty, value); }
        }
        public Color SmileyColor
        {
            get { return (Color)GetValue(Smiley.SmileyColorProperty); }
            set { SetValue(Smiley.SmileyColorProperty, value); }
        }
        public Color SmileyLineColor
        {
            get { return (Color)GetValue(Smiley.SmileyLineColorProperty); }
            set { SetValue(Smiley.SmileyLineColorProperty, value); }
        }
        public SmileyMoods SmileyMood
        {
            get { return (SmileyMoods)GetValue(Smiley.SmileyMoodProperty); }
            set { SetValue(Smiley.SmileyMoodProperty, value); }
        }
        public double SmileyLineThickness
        {
            get { return (double)GetValue(Smiley.SmileyLineThicknessProperty); }
            set { SetValue(Smiley.SmileyLineThicknessProperty, value); }
        }
        public double SmileyLineBlur
        {
            get { return (double)GetValue(Smiley.SmileyLineBlurProperty); }
            set { SetValue(Smiley.SmileyLineBlurProperty, value); }
        }
        public string Id { get; set; }

        static Smiley()
        {
            PropertyMetadata radiusMeta = new PropertyMetadata(new PropertyChangedCallback(Smiley.OnRadiusChanged));
            PropertyMetadata colorMeta = new PropertyMetadata(new PropertyChangedCallback(Smiley.OnColorChanged));
            PropertyMetadata linecolorMeta = new PropertyMetadata(new PropertyChangedCallback(Smiley.OnLineColorChanged));
            PropertyMetadata moodMeta = new PropertyMetadata(SmileyMoods.None, new PropertyChangedCallback(Smiley.OnMoodChanged));
            PropertyMetadata lineBlurMeta = new PropertyMetadata(0D, new PropertyChangedCallback(Smiley.OnLineBlurChanged));
            PropertyMetadata lineThicknessMeta = new PropertyMetadata(1D, new PropertyChangedCallback(Smiley.OnLineThicknessChanged));

            Smiley.SmileyRadiusProperty = DependencyProperty.Register("SmileyRadius", typeof(double), typeof(Smiley), radiusMeta);
            Smiley.SmileyColorProperty = DependencyProperty.Register("SmileyColor", typeof(Color), typeof(Smiley), colorMeta);
            Smiley.SmileyLineColorProperty = DependencyProperty.Register("SmileyLineColor", typeof(Color), typeof(Smiley), linecolorMeta);
            Smiley.SmileyMoodProperty = DependencyProperty.Register("SmileyMood", typeof(SmileyMoods), typeof(Smiley), moodMeta);
            Smiley.SmileyLineBlurProperty = DependencyProperty.Register("SmileyLineBlur", typeof(double), typeof(Smiley), lineBlurMeta);
            Smiley.SmileyLineThicknessProperty = DependencyProperty.Register("SmileyLineThickness", typeof(double), typeof(Smiley), lineThicknessMeta);
        }
        public Smiley()
        {
            InitializeComponent();


            this.Id = System.Guid.NewGuid().ToString();
        }

        private static void SetLines(Smiley instance)
        {
            SolidColorBrush b = new SolidColorBrush(instance.SmileyLineColor);
            instance.BodyRectangle.Stroke = b;
            instance.LeftEyeRectangle.Stroke = b;
            instance.LeftEyeDrunk1.Stroke = b;
            instance.LeftEyeDrunk2.Stroke = b;
            instance.LeftEyeEllipse.Stroke = b;
            instance.RightEyeRectangle.Stroke = b;
            instance.RightEyeDrunk1.Stroke = b;
            instance.RightEyeDrunk2.Stroke = b;
            instance.RightEyeEllipse.Stroke = b;
            instance.MouthPathHappy.Stroke = b;
            instance.MouthPathIndifferent.Stroke = b;
            instance.MouthPathSad.Stroke = b;
            instance.MouthShocked.Stroke = b;

            instance.BodyRectangle.StrokeThickness = instance.SmileyLineThickness;
            instance.LeftEyeRectangle.StrokeThickness = instance.SmileyLineThickness;
            instance.LeftEyeDrunk1.StrokeThickness = instance.SmileyLineThickness;
            instance.LeftEyeDrunk2.StrokeThickness = instance.SmileyLineThickness;
            instance.LeftEyeEllipse.StrokeThickness = instance.SmileyLineThickness;
            instance.RightEyeRectangle.StrokeThickness = instance.SmileyLineThickness;
            instance.RightEyeDrunk1.StrokeThickness = instance.SmileyLineThickness;
            instance.RightEyeDrunk2.StrokeThickness = instance.SmileyLineThickness;
            instance.RightEyeEllipse.StrokeThickness = instance.SmileyLineThickness;
            instance.MouthPathHappy.StrokeThickness = instance.SmileyLineThickness;
            instance.MouthPathIndifferent.StrokeThickness = instance.SmileyLineThickness;
            instance.MouthPathSad.StrokeThickness = instance.SmileyLineThickness;
            instance.MouthShocked.StrokeThickness = instance.SmileyLineThickness;

            instance.BodyRectangle.Effect = new BlurEffect() { KernelType = KernelType.Gaussian, Radius = instance.SmileyLineBlur, RenderingBias = RenderingBias.Performance};
            instance.LeftEyeRectangle.Effect = new BlurEffect() { KernelType = KernelType.Gaussian, Radius = instance.SmileyLineBlur, RenderingBias = RenderingBias.Performance };
            instance.LeftEyeDrunk1.Effect = new BlurEffect() { KernelType = KernelType.Gaussian, Radius = instance.SmileyLineBlur, RenderingBias = RenderingBias.Performance };
            instance.LeftEyeDrunk2.Effect = new BlurEffect() { KernelType = KernelType.Gaussian, Radius = instance.SmileyLineBlur, RenderingBias = RenderingBias.Performance };
            instance.RightEyeRectangle.Effect = new BlurEffect() { KernelType = KernelType.Gaussian, Radius = instance.SmileyLineBlur, RenderingBias = RenderingBias.Performance };
            instance.RightEyeDrunk1.Effect = new BlurEffect() { KernelType = KernelType.Gaussian, Radius = instance.SmileyLineBlur, RenderingBias = RenderingBias.Performance };
            instance.RightEyeDrunk2.Effect = new BlurEffect() { KernelType = KernelType.Gaussian, Radius = instance.SmileyLineBlur, RenderingBias = RenderingBias.Performance };
            instance.RightEyeEllipse.Effect = new BlurEffect() { KernelType = KernelType.Gaussian, Radius = instance.SmileyLineBlur, RenderingBias = RenderingBias.Performance };
            instance.MouthPathHappy.Effect = new BlurEffect() { KernelType = KernelType.Gaussian, Radius = instance.SmileyLineBlur, RenderingBias = RenderingBias.Performance };
            instance.MouthPathIndifferent.Effect = new BlurEffect() { KernelType = KernelType.Gaussian, Radius = instance.SmileyLineBlur, RenderingBias = RenderingBias.Performance };
            instance.MouthPathSad.Effect = new BlurEffect() { KernelType = KernelType.Gaussian, Radius = instance.SmileyLineBlur, RenderingBias = RenderingBias.Performance };
            instance.MouthShocked.Effect = new BlurEffect() { KernelType = KernelType.Gaussian, Radius = instance.SmileyLineBlur, RenderingBias = RenderingBias.Performance };

            instance.RightEyeEllipse.Fill = b;
            instance.MouthShocked.Fill = b;
            instance.LeftEyeRectangle.Fill = b;
            instance.LeftEyeEllipse.Fill = b;
            instance.RightEyeRectangle.Fill = b;
        }

        private static void OnRadiusChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Smiley sc = o as Smiley;
            
            sc.BodyRectangle.RadiusX = (double)e.NewValue;
            sc.BodyRectangle.RadiusY = (double)e.NewValue;
        }
        private static void OnColorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Smiley sc = o as Smiley;
            sc.BodyRectangle.Fill = new SolidColorBrush((Color)e.NewValue);
            sc.InvalidateVisual();
        }
        private static void OnLineColorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SetLines(o as Smiley);
        }
        private static void OnMoodChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Smiley sc = o as Smiley;
            SmileyMoods m = (SmileyMoods)e.NewValue;
            switch (m)
            {
                case SmileyMoods.Happy:
                    sc.MouthPathHappy.Visibility = Visibility.Visible;
                    sc.MouthPathSad.Visibility = Visibility.Collapsed;
                    sc.MouthPathIndifferent.Visibility = Visibility.Collapsed;
                    sc.MouthShocked.Visibility = Visibility.Collapsed;
                    sc.LeftEyeDrunk1.Visibility = Visibility.Collapsed;
                    sc.LeftEyeDrunk2.Visibility = Visibility.Collapsed;
                    sc.RightEyeDrunk1.Visibility = Visibility.Collapsed;
                    sc.RightEyeDrunk2.Visibility = Visibility.Collapsed;
                    sc.RightEyeRectangle.Visibility = Visibility.Visible;
                    sc.LeftEyeRectangle.Visibility = Visibility.Visible;
                    sc.LeftEyeEllipse.Visibility = Visibility.Collapsed;
                    sc.RightEyeEllipse.Visibility = Visibility.Collapsed;
                    break;
                case SmileyMoods.Sad:
                    sc.MouthPathHappy.Visibility = Visibility.Collapsed;
                    sc.MouthPathSad.Visibility = Visibility.Visible;
                    sc.MouthPathIndifferent.Visibility = Visibility.Collapsed;
                    sc.MouthShocked.Visibility = Visibility.Collapsed;
                    sc.LeftEyeDrunk1.Visibility = Visibility.Collapsed;
                    sc.LeftEyeDrunk2.Visibility = Visibility.Collapsed;
                    sc.RightEyeDrunk1.Visibility = Visibility.Collapsed;
                    sc.RightEyeDrunk2.Visibility = Visibility.Collapsed;
                    sc.RightEyeRectangle.Visibility = Visibility.Visible;
                    sc.LeftEyeRectangle.Visibility = Visibility.Visible;
                    sc.LeftEyeEllipse.Visibility = Visibility.Collapsed;
                    sc.RightEyeEllipse.Visibility = Visibility.Collapsed;
                    break;
                case SmileyMoods.Indifferent:
                    sc.MouthPathHappy.Visibility = Visibility.Collapsed;
                    sc.MouthPathSad.Visibility = Visibility.Collapsed;
                    sc.MouthPathIndifferent.Visibility = Visibility.Visible;
                    sc.MouthShocked.Visibility = Visibility.Collapsed;
                    sc.LeftEyeDrunk1.Visibility = Visibility.Collapsed;
                    sc.LeftEyeDrunk2.Visibility = Visibility.Collapsed;
                    sc.RightEyeDrunk1.Visibility = Visibility.Collapsed;
                    sc.RightEyeDrunk2.Visibility = Visibility.Collapsed;
                    sc.RightEyeRectangle.Visibility = Visibility.Visible;
                    sc.LeftEyeRectangle.Visibility = Visibility.Visible;
                    sc.LeftEyeEllipse.Visibility = Visibility.Collapsed;
                    sc.RightEyeEllipse.Visibility = Visibility.Collapsed;
                    break;
                case SmileyMoods.Angry:
                    sc.MouthPathHappy.Visibility = Visibility.Collapsed;
                    sc.MouthPathSad.Visibility = Visibility.Visible;
                    sc.MouthPathIndifferent.Visibility = Visibility.Collapsed;
                    sc.MouthShocked.Visibility = Visibility.Collapsed;
                    sc.LeftEyeDrunk1.Visibility = Visibility.Visible;
                    sc.LeftEyeDrunk2.Visibility = Visibility.Collapsed;
                    sc.RightEyeDrunk1.Visibility = Visibility.Collapsed;
                    sc.RightEyeDrunk2.Visibility = Visibility.Visible;
                    sc.RightEyeRectangle.Visibility = Visibility.Collapsed;
                    sc.LeftEyeRectangle.Visibility = Visibility.Collapsed;
                    sc.LeftEyeEllipse.Visibility = Visibility.Collapsed;
                    sc.RightEyeEllipse.Visibility = Visibility.Collapsed;
                    break;
                case SmileyMoods.Drunk:
                    sc.MouthPathHappy.Visibility = Visibility.Collapsed;
                    sc.MouthPathSad.Visibility = Visibility.Collapsed;
                    sc.MouthPathIndifferent.Visibility = Visibility.Visible;
                    sc.MouthShocked.Visibility = Visibility.Collapsed;
                    sc.LeftEyeDrunk1.Visibility = Visibility.Visible;
                    sc.LeftEyeDrunk2.Visibility = Visibility.Visible;
                    sc.RightEyeDrunk1.Visibility = Visibility.Visible;
                    sc.RightEyeDrunk2.Visibility = Visibility.Visible;
                    sc.RightEyeRectangle.Visibility = Visibility.Collapsed;
                    sc.LeftEyeRectangle.Visibility = Visibility.Collapsed;
                    sc.LeftEyeEllipse.Visibility = Visibility.Collapsed;
                    sc.RightEyeEllipse.Visibility = Visibility.Collapsed;
                    break;
                case SmileyMoods.Scared:
                    sc.MouthPathHappy.Visibility = Visibility.Collapsed;
                    sc.MouthPathSad.Visibility = Visibility.Collapsed;
                    sc.MouthPathIndifferent.Visibility = Visibility.Visible;
                    sc.MouthShocked.Visibility = Visibility.Collapsed;
                    sc.LeftEyeDrunk1.Visibility = Visibility.Collapsed;
                    sc.LeftEyeDrunk2.Visibility = Visibility.Collapsed;
                    sc.RightEyeDrunk1.Visibility = Visibility.Collapsed;
                    sc.RightEyeDrunk2.Visibility = Visibility.Collapsed;
                    sc.RightEyeRectangle.Visibility = Visibility.Collapsed;
                    sc.LeftEyeRectangle.Visibility = Visibility.Collapsed;
                    sc.LeftEyeEllipse.Visibility = Visibility.Visible;
                    sc.RightEyeEllipse.Visibility = Visibility.Visible;
                    break;
                case SmileyMoods.Shocked:
                    sc.MouthPathHappy.Visibility = Visibility.Collapsed;
                    sc.MouthPathSad.Visibility = Visibility.Collapsed;
                    sc.MouthPathIndifferent.Visibility = Visibility.Collapsed;
                    sc.MouthShocked.Visibility = Visibility.Visible;
                    sc.LeftEyeDrunk1.Visibility = Visibility.Collapsed;
                    sc.LeftEyeDrunk2.Visibility = Visibility.Collapsed;
                    sc.RightEyeDrunk1.Visibility = Visibility.Collapsed;
                    sc.RightEyeDrunk2.Visibility = Visibility.Collapsed;
                    sc.RightEyeRectangle.Visibility = Visibility.Collapsed;
                    sc.LeftEyeRectangle.Visibility = Visibility.Collapsed;
                    sc.LeftEyeEllipse.Visibility = Visibility.Visible;
                    sc.RightEyeEllipse.Visibility = Visibility.Visible;
                    break;
                case SmileyMoods.Mischievous:
                    sc.MouthPathHappy.Visibility = Visibility.Visible;
                    sc.MouthPathSad.Visibility = Visibility.Collapsed;
                    sc.MouthPathIndifferent.Visibility = Visibility.Collapsed;
                    sc.MouthShocked.Visibility = Visibility.Collapsed;
                    sc.LeftEyeDrunk1.Visibility = Visibility.Visible;
                    sc.LeftEyeDrunk2.Visibility = Visibility.Collapsed;
                    sc.RightEyeDrunk1.Visibility = Visibility.Collapsed;
                    sc.RightEyeDrunk2.Visibility = Visibility.Visible;
                    sc.RightEyeRectangle.Visibility = Visibility.Collapsed;
                    sc.LeftEyeRectangle.Visibility = Visibility.Collapsed;
                    sc.LeftEyeEllipse.Visibility = Visibility.Collapsed;
                    sc.RightEyeEllipse.Visibility = Visibility.Collapsed;
                    break;
            }
        }
        private static void OnLineThicknessChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SetLines(o as Smiley);
        }
        private static void OnLineBlurChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SetLines(o as Smiley);
        }
    }
}
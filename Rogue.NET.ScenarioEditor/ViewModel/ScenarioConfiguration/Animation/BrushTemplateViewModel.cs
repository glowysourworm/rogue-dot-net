using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;

using System.Collections.ObjectModel;
using System.Windows.Media;

using ReactiveUI;


namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation
{
    public class BrushTemplateViewModel : TemplateViewModel
    {
        public ObservableCollection<GradientStopTemplateViewModel> GradientStops { get; set; }

        private BrushType _type;
        private double _opacity;
        private string _solidColor;
        private double _gradientStartX;
        private double _gradientStartY;
        private double _gradientEndX;
        private double _gradientEndY;

        public BrushType Type
        {
            get { return _type; }
            set { this.RaiseAndSetIfChanged(ref _type, value); }
        }
        public double Opacity
        {
            get { return _opacity; }
            set { this.RaiseAndSetIfChanged(ref _opacity, value); }
        }
        public string SolidColor
        {
            get { return _solidColor; }
            set { this.RaiseAndSetIfChanged(ref _solidColor, value); }
        }
        public double GradientStartX
        {
            get { return _gradientStartX; }
            set { this.RaiseAndSetIfChanged(ref _gradientStartX, value); }
        }
        public double GradientStartY
        {
            get { return _gradientStartY; }
            set { this.RaiseAndSetIfChanged(ref _gradientStartY, value); }
        }
        public double GradientEndX
        {
            get { return _gradientEndX; }
            set { this.RaiseAndSetIfChanged(ref _gradientEndX, value); }
        }
        public double GradientEndY
        {
            get { return _gradientEndY; }
            set { this.RaiseAndSetIfChanged(ref _gradientEndY, value); }
        }

        public BrushTemplateViewModel()
        {
            this.GradientStops = new ObservableCollection<GradientStopTemplateViewModel>();
            this.Opacity = 1;
            this.SolidColor = Colors.White.ToString();
        }
        public BrushTemplateViewModel(string name)
        {
            this.GradientStops = new ObservableCollection<GradientStopTemplateViewModel>();
            this.Name = name;
            this.Opacity = 1;
            this.SolidColor = Colors.White.ToString();
        }
    }
}

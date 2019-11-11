using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Layout
{
    public class LightTemplateViewModel : TemplateViewModel
    {
        byte _red;
        byte _green;
        byte _blue;
        double _intensity;

        public byte Red
        {
            get { return _red; }
            set { this.RaiseAndSetIfChanged(ref _red, value); }
        }
        public byte Green
        {
            get { return _green; }
            set { this.RaiseAndSetIfChanged(ref _green, value); }
        }
        public byte Blue
        {
            get { return _blue; }
            set { this.RaiseAndSetIfChanged(ref _blue, value); }
        }
        public double Intensity
        {
            get { return _intensity; }
            set { this.RaiseAndSetIfChanged(ref _intensity, value); }
        }

        public LightTemplateViewModel()
        {
            this.Red = 0xFF;
            this.Blue = 0xFF;
            this.Green = 0xFf;
            this.Intensity = 1.0;
        }
    }
}

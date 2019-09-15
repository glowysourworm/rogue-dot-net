using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;

using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows;
using System;
using System.Linq;
using Rogue.NET.Common.Extension;
using Rogue.NET.Common.ViewModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Data;
using Rogue.NET.Common.Extension.Event;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation
{
    public class BrushTemplateViewModel : TemplateViewModel
    {
        /// <summary>
        /// Event to deal with the combined updates for all public properties and collections for the
        /// BrushTemplateViewModel. This solves the issue of multibinding (wasn't updating the brush)
        /// and the failed attempt at creating a MarkupExtension to issue a custom binding for brushes.
        /// </summary>
        public event SimpleEventHandler BrushUpdatedEvent;

        public NotifyingObservableCollection<GradientStopTemplateViewModel> GradientStops { get; set; }

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
            set { this.RaiseAndSetIfChanged(ref _type, value); OnBrushUpdateEvent(); }
        }
        public double Opacity
        {
            get { return _opacity; }
            set { this.RaiseAndSetIfChanged(ref _opacity, value); OnBrushUpdateEvent(); }
        }
        public string SolidColor
        {
            get { return _solidColor; }
            set { this.RaiseAndSetIfChanged(ref _solidColor, value); OnBrushUpdateEvent(); }
        }
        public double GradientStartX
        {
            get { return _gradientStartX; }
            set { this.RaiseAndSetIfChanged(ref _gradientStartX, value); OnBrushUpdateEvent(); }
        }
        public double GradientStartY
        {
            get { return _gradientStartY; }
            set { this.RaiseAndSetIfChanged(ref _gradientStartY, value); OnBrushUpdateEvent(); }
        }
        public double GradientEndX
        {
            get { return _gradientEndX; }
            set { this.RaiseAndSetIfChanged(ref _gradientEndX, value); OnBrushUpdateEvent(); }
        }
        public double GradientEndY
        {
            get { return _gradientEndY; }
            set { this.RaiseAndSetIfChanged(ref _gradientEndY, value); OnBrushUpdateEvent(); }
        }

        public BrushTemplateViewModel()
        {
            this.GradientStops = new NotifyingObservableCollection<GradientStopTemplateViewModel>();
            this.Opacity = 1;
            this.SolidColor = Colors.White.ToString();

            this.GradientStops.CollectionChanged += OnGradientStopsChanged;
            this.GradientStops.ItemPropertyChanged += OnGradientStopsItemChanged;
        }
        public BrushTemplateViewModel(string name)
        {
            this.GradientStops = new NotifyingObservableCollection<GradientStopTemplateViewModel>();
            this.Name = name;
            this.Opacity = 1;
            this.SolidColor = Colors.White.ToString();

            this.GradientStops.CollectionChanged += OnGradientStopsChanged;
            this.GradientStops.ItemPropertyChanged += OnGradientStopsItemChanged;
        }
        public BrushTemplateViewModel(string name, BrushTemplateViewModel copy)
        {
            this.Name = name;
            this.GradientEndX = copy.GradientEndX;
            this.GradientEndY = copy.GradientEndY;
            this.GradientStartX = copy.GradientStartX;
            this.GradientStartY = copy.GradientStartY;
            this.GradientStops = new NotifyingObservableCollection<GradientStopTemplateViewModel>(copy.GradientStops
                                                                                                      .Select(x => new GradientStopTemplateViewModel(x.GradientOffset, x.GradientColor))
                                                                                                      .Actualize());

            this.Opacity = copy.Opacity;
            this.SolidColor = copy.SolidColor;
            this.Type = copy.Type;

            this.GradientStops.CollectionChanged += OnGradientStopsChanged;
            this.GradientStops.ItemPropertyChanged += OnGradientStopsItemChanged;
        }

        private void OnGradientStopsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnBrushUpdateEvent();
        }

        private void OnGradientStopsItemChanged(object sender, PropertyChangedEventArgs e)
        {
            OnBrushUpdateEvent();
        }

        private void OnBrushUpdateEvent()
        {
            if (this.BrushUpdatedEvent != null)
                this.BrushUpdatedEvent();
        }
    }
}

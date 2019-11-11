﻿using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System.Windows;
using System.Windows.Media;

namespace Rogue.NET.Core.Processing.Service.Cache.Interface
{
    /// <summary>
    /// Component that creates and manages caching of scenario image sources
    /// </summary>
    public interface IScenarioImageSourceFactory
    {
        DrawingImage GetImageSource(SymbolDetailsTemplate symbolDetails, double scale, Color lighting);
        DrawingImage GetImageSource(ScenarioImage scenarioImage, double scale, Color lighting);
        DrawingImage GetDesaturatedImageSource(ScenarioImage scenarioImage, double scale, Color lighting);
        FrameworkElement GetFrameworkElement(ScenarioImage scenarioImage, double scale, Color lighting);
    }
}

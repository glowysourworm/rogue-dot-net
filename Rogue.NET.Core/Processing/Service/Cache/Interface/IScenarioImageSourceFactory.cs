using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
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
        DrawingImage GetImageSource(SymbolDetailsTemplate symbolDetails, double scale, double effectiveVision, Light[] lighting);
        DrawingImage GetImageSource(ScenarioImage scenarioImage, double scale, double effectiveVision, Light[] lighting);
        DrawingImage GetDesaturatedImageSource(ScenarioImage scenarioImage, double scale, double effectiveVision, Light[] lighting);
        FrameworkElement GetFrameworkElement(ScenarioImage scenarioImage, double scale, double effectiveVision, Light[] lighting);
    }
}

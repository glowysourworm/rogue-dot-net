using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;

namespace Rogue.NET.ScenarioEditor.Service.Interface
{
    /// <summary>
    /// Service that calculates projections about the scenario
    /// </summary>
    public interface IScenarioOverviewCalculationService
    {
        /// <summary>
        /// Calculates overview for the provided asset and stores the results in the appropriate place
        /// </summary>
        void CalculateOverview(TemplateViewModel templateViewModel);
    }
}

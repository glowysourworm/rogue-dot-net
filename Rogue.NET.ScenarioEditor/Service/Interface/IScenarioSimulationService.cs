using Rogue.NET.Core.Model.ScenarioConfiguration.Design;
using Rogue.NET.ScenarioEditor.ViewModel.Overview.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.ScenarioEditor.Service.Interface
{
    /// <summary>
    /// Component that simulates quantities for the scenario
    /// </summary>
    public interface IScenarioSimulationService
    {
        IProjectionSetViewModel CalculateProjectedExperience(IEnumerable<LevelTemplateViewModel> levels);

        IProjectionSetViewModel CalculateProjectedGeneration(IEnumerable<LevelTemplateViewModel> levels, TemplateViewModel asset);
    }
}

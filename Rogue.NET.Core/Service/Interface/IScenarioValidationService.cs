using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Model.Validation.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Service.Interface
{
    /// <summary>
    /// Service to validate the ScenarioConfigurationContainer before playing
    /// </summary>
    public interface IScenarioValidationService
    {
        IEnumerable<IScenarioValidationMessage> Validate(ScenarioConfigurationContainer scenarioConfigurationContainer);
    }
}

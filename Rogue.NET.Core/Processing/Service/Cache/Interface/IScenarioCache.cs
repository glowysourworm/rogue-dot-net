using Rogue.NET.Core.Model.Scenario;
using System.Collections.Generic;

namespace Rogue.NET.Core.Processing.Service.Cache.Interface
{
    public interface IScenarioCache
    {
        IEnumerable<ScenarioContainer> GetScenarios();

        void SaveScenario(ScenarioContainer scenario);

        void DeleteScenario(ScenarioContainer scenario);
    }
}

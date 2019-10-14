using Rogue.NET.Core.Model.Scenario;
using System.Collections.Generic;

namespace Rogue.NET.Core.Processing.Service.Cache.Interface
{
    public interface IScenarioCache
    {
        IEnumerable<string> GetScenarioNames();
        IEnumerable<ScenarioInfo> GetScenarioInfos();
        ScenarioContainer GetScenario(string name);
        void SaveScenario(ScenarioContainer scenario);
        void DeleteScenario(string scenarioName);
    }
}

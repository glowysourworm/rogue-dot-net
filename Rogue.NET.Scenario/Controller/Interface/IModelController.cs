using Rogue.NET.Core.Model.ScenarioConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Scenario.Controller.Interface
{
    /// <summary>
    /// Stateful model managing controller
    /// </summary>
    public interface IModelController
    {
        void Initialize();

        // scenario loading
        void New(ScenarioConfigurationContainer config, string rogueName, int seed, bool survivorMode);
        void Open(string file);
        void Save();

        void LoadCurrentLevel();

        // TODO: move this 
        Dictionary<string, string> GetGameDisplayStats();
    }
}

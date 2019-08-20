using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.ScenarioEditor.Service.Interface
{
    /// <summary>
    /// Component responsible for copying alteration container names to the associated effects
    /// </summary>
    public interface IAlterationNameService
    {
        /// <summary>
        /// Copies name of Alteration -> Alteration Effect. This should be done just before saving
        /// </summary>
        void Execute(ScenarioConfigurationContainerViewModel configuration);
    }
}

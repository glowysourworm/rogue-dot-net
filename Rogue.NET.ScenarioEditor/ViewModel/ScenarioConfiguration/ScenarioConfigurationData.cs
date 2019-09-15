using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration
{
    /// <summary>
    /// Data class to hold ScenarioConfigurationContainerViewModel and associated "General Collections" that are 
    /// calculated while reading the configuration into memory
    /// </summary>
    public class ScenarioConfigurationData
    {
        /// <summary>
        /// Configuration read from resource or file
        /// </summary>
        public ScenarioConfigurationContainerViewModel Configuration { get; private set; }

        /// <summary>
        /// Brushes parsed from reading the configuration
        /// </summary>
        public IEnumerable<BrushTemplateViewModel> ConfigurationBrushes { get; private set; }

        public ScenarioConfigurationData(ScenarioConfigurationContainerViewModel configuration, IEnumerable<BrushTemplateViewModel> scenarioBrushes)
        {
            this.Configuration = configuration;
            this.ConfigurationBrushes = scenarioBrushes;
        }
    }
}

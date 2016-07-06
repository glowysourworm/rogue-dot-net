using Microsoft.Practices.Prism.Events;
using Rogue.NET.Common;
using Rogue.NET.Common.Events.Splash;
using Rogue.NET.Scenario;
using Rogue.NET.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Practices.Prism.PubSubEvents;

namespace Rogue.NET.Model
{
    public interface IResourceService
    {
        IEnumerable<ScenarioConfiguration> GetScenarioConfigurations();
        IDictionary<string, ScenarioFileHeader> GetScenarioHeaders();
    }
    public class ResourceService : IResourceService
    {
        readonly IEventAggregator _eventAggregator;
        readonly IEnumerable<ScenarioConfiguration> _scenarioConfigurations;

        public ResourceService(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            var easy = ResourceManager.GetEmbeddedScenarioConfiguration(ConfigResources.Fighter);
            _eventAggregator.GetEvent<SplashUpdateEvent>().Publish(new SplashUpdateEvent()
            {
                Message = "Loading Fighter Scenario Configuration...",
                Progress = 22
            });
            var normal = ResourceManager.GetEmbeddedScenarioConfiguration(ConfigResources.Paladin);
            _eventAggregator.GetEvent<SplashUpdateEvent>().Publish(new SplashUpdateEvent()
            {
                Message = "Loading Paladin Scenario Configuration...",
                Progress = 24
            });
            var hard = ResourceManager.GetEmbeddedScenarioConfiguration(ConfigResources.Witch);
            _eventAggregator.GetEvent<SplashUpdateEvent>().Publish(new SplashUpdateEvent()
            {
                Message = "Loading Witch Scenario Configuration...",
                Progress = 26
            });
            var brutal = ResourceManager.GetEmbeddedScenarioConfiguration(ConfigResources.Sorcerer);
            _eventAggregator.GetEvent<SplashUpdateEvent>().Publish(new SplashUpdateEvent()
            {
                Message = "Loading Sorcerer Scenario Configuration...",
                Progress = 28
            });

            if (!Directory.Exists(Constants.SAVED_GAMES_DIR))
                Directory.CreateDirectory(Constants.SAVED_GAMES_DIR);

            if (!Directory.Exists(Constants.SCENARIOS_DIR))
                Directory.CreateDirectory(Constants.SCENARIOS_DIR);

            _scenarioConfigurations = new List<ScenarioConfiguration>(new ScenarioConfiguration[]{
                easy,
                normal,
                hard,
                brutal
            });
        }

        public IEnumerable<ScenarioConfiguration> GetScenarioConfigurations()
        {
            return _scenarioConfigurations;
        }

        public IDictionary<string, ScenarioFileHeader> GetScenarioHeaders()
        {
            var scenarioFiles = Directory.GetFiles(Constants.SAVED_GAMES_DIR);
            var scenarioHeaders = new Dictionary<string, ScenarioFileHeader>();
            foreach (var file in scenarioFiles)
            {
                var header = ScenarioFile.OpenHeader(file);
                var name = Path.GetFileNameWithoutExtension(file);
                if (header != null)
                    scenarioHeaders.Add(name, header);
            }

            return scenarioHeaders;
        }
    }
}

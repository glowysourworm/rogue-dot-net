using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Utility;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Media;

namespace Rogue.NET.Core.View
{
    public class RandomScenarioSmiley : Smiley
    {
        static readonly IEnumerable<ScenarioImage> _configurationSmileyFaces;
        static readonly IList<ScenarioImage> _chosenSmileyFaces;

        static RandomScenarioSmiley()
        {
            _configurationSmileyFaces = GetAllSmileyFaces();
            _chosenSmileyFaces = new List<ScenarioImage>();
        }
        public RandomScenarioSmiley()
        {
            // Get Random Smiley Face
            var nonChosenSmileyFaces = _configurationSmileyFaces.Except(_chosenSmileyFaces);

            ScenarioImage chosenSmileyFace;

            // If they're all chosen - reload and start over
            if (nonChosenSmileyFaces.None())
            {
                _chosenSmileyFaces.Clear();

                nonChosenSmileyFaces = _configurationSmileyFaces.Except(_chosenSmileyFaces);
            }

            if (nonChosenSmileyFaces.Any())
            {
                chosenSmileyFace = nonChosenSmileyFaces.PickRandom();

                _chosenSmileyFaces.Add(chosenSmileyFace);
            }
            else
            {
                chosenSmileyFace = new ScenarioImage()
                {
                    SmileyBodyColor = Colors.Yellow.ToString(),
                    SmileyLineColor = Colors.Black.ToString(),
                    SmileyExpression = SmileyExpression.Happy
                };
            }

            

            // Set Traits from randomly picked character class
            this.SmileyColor = ColorUtility.Convert(chosenSmileyFace.SmileyBodyColor);
            this.SmileyLineColor = ColorUtility.Convert(chosenSmileyFace.SmileyLineColor);
            this.SmileyExpression = chosenSmileyFace.SmileyExpression;
        }

        // TODO: THIS WAS DONE AS JUST A ONE-OFF. THESE NEED TO BE LOADED AND PASSED INTO THE BOOTSTRAPPER
        private static IEnumerable<ScenarioImage> GetAllSmileyFaces()
        {
            var configurations = new List<ScenarioConfigurationContainer>();

            foreach (var configResource in Enum.GetValues(typeof(ConfigResources)))
            {
                var name = configResource.ToString();
                var assembly = Assembly.GetAssembly(typeof(ZipEncoder));
                var location = "Rogue.NET.Common.Resource.Configuration." + name.ToString() + "." + ResourceConstants.ScenarioConfigurationExtension;
                using (var stream = assembly.GetManifestResourceStream(location))
                {
                    var memoryStream = new MemoryStream();
                    stream.CopyTo(memoryStream);

                    var configuration = (ScenarioConfigurationContainer)BinarySerializer.Deserialize(memoryStream.GetBuffer());

                    configurations.Add(configuration);
                }
            }

            // Have to copy configuration because of the HasBeenGenerated flags in memory
            return configurations.SelectMany(x => x.PlayerTemplates.Select(z => new ScenarioImage(z.SymbolDetails)))
                                 .Actualize();
        }
    }
}

using Microsoft.Practices.ServiceLocation;
using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Utility;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ResourceCache;
using Rogue.NET.Core.Model.ResourceCache.Interface;
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
        static bool _IS_LOADED = false;
        static IEnumerable<ScenarioImage> _configurationSmileyFaces;
        static IList<ScenarioImage> _chosenSmileyFaces;

        public RandomScenarioSmiley()
        {
            var configurationCache = ServiceLocator.Current.GetInstance<IScenarioConfigurationCache>();

            if (!_IS_LOADED)
            {
                _configurationSmileyFaces = configurationCache.EmbeddedConfigurations
                                                              .SelectMany(x => x.PlayerTemplates.Select(z => new ScenarioImage(z.SymbolDetails)))
                                                              .Actualize();

                _chosenSmileyFaces = new List<ScenarioImage>();

                _IS_LOADED = true;
            }

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
    }
}

using Microsoft.Practices.ServiceLocation;
using Rogue.NET.Core.Media.SymbolEffect.Utility;
using Rogue.NET.Core.Processing.Service.Interface;

namespace Rogue.NET.Core.View
{
    public class RandomScenarioSmiley : Smiley
    {
        public RandomScenarioSmiley()
        {
            var resourceService = ServiceLocator.Current.GetInstance<IScenarioResourceService>();
            var chosenSmileyFace = resourceService.GetRandomSmileyCharacter(true);

            // Set Traits from randomly picked character class
            this.SmileyColor = ColorFilter.Convert(chosenSmileyFace.SmileyBodyColor);
            this.SmileyLineColor = ColorFilter.Convert(chosenSmileyFace.SmileyLineColor);
            this.SmileyExpression = chosenSmileyFace.SmileyExpression;
        }
    }
}

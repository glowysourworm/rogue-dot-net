using Rogue.NET.Core.Media.SymbolEffect.Utility;
using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.View;
using System.Windows.Controls;

namespace Rogue.NET.Scenario.Intro.ViewModel
{
    public class CharacterClassSelectionViewModel : ContentPresenter
    {
        public string RogueName { get; set; }
        public string Description { get; set; }

        public CharacterClassSelectionViewModel(SymbolDetailsTemplate template)
        {
            var smiley = new Smiley();

            smiley.SmileyColor = ColorOperations.Convert(template.SmileyBodyColor);
            smiley.SmileyLineColor = ColorOperations.Convert(template.SmileyLineColor);
            smiley.SmileyExpression = template.SmileyExpression;
            smiley.Height = ModelConstants.CellHeight * 2;
            smiley.Width = ModelConstants.CellWidth * 2;

            this.Content = smiley;
        }
    }
}

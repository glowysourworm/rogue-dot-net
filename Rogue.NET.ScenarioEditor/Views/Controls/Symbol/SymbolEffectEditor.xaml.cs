using Rogue.NET.Common.Constant;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.Utility;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Common;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Media;

namespace Rogue.NET.ScenarioEditor.Views.Controls.Symbol
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export]
    public partial class SymbolEffectEditor : UserControl
    {
        public SymbolEffectEditor()
        {
            InitializeComponent();

            this.DataContextChanged += (sender, e) =>
            {
                var oldViewModel = e.OldValue as SymbolEffectTemplateViewModel;
                var newViewModel = e.NewValue as SymbolEffectTemplateViewModel;

                if (oldViewModel != null)
                    oldViewModel.PropertyChanged -= OnSymbolEffectChanged;

                if (newViewModel != null)
                    newViewModel.PropertyChanged += OnSymbolEffectChanged;
            };
        }

        private void OnSymbolEffectChanged(object sender, PropertyChangedEventArgs e)
        {
            var viewModel = this.DataContext as SymbolEffectTemplateViewModel;

            if (viewModel != null)
            {
                var smileyPreviewSymbolDetails = new SymbolDetailsTemplateViewModel();
                var characterPreviewSymbolDetails = new SymbolDetailsTemplateViewModel();

                smileyPreviewSymbolDetails.SymbolType = SymbolType.Smiley;
                smileyPreviewSymbolDetails.SmileyBodyColor = viewModel.SmileyBodyColor ?? Colors.Yellow.ToString();
                smileyPreviewSymbolDetails.SmileyExpression = viewModel.SmileyExpression;
                smileyPreviewSymbolDetails.SmileyLineColor = viewModel.SmileyLineColor ?? Colors.Black.ToString();

                characterPreviewSymbolDetails.SymbolType = SymbolType.Character;
                characterPreviewSymbolDetails.CharacterColor = viewModel.CharacterColor ?? Colors.White.ToString();
                characterPreviewSymbolDetails.CharacterScale = 1;
                characterPreviewSymbolDetails.CharacterSymbol = viewModel.CharacterSymbol ?? CharacterSymbol.DefaultCharacterSymbol;
                characterPreviewSymbolDetails.CharacterSymbolCategory = viewModel.CharacterSymbolCategory ?? CharacterSymbol.DefaultCharacterCategory;

                this.SmileyPreviewControl.DataContext = smileyPreviewSymbolDetails;
                this.CharacterPreviewControl.DataContext = characterPreviewSymbolDetails;
            }
        }

        private void CharacterSymbolButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var view = new CharacterMap();
            var viewModel = this.DataContext as SymbolEffectTemplateViewModel;
            var characterPreviewViewModel = this.CharacterPreviewControl.DataContext as SymbolDetailsTemplateViewModel;
            view.DataContext = characterPreviewViewModel;

            // Can be shown as a dialog
            if (DialogWindowFactory.Show(view, "Rogue UTF-8 Character Map"))
            {
                viewModel.CharacterSymbol = characterPreviewViewModel.CharacterSymbol;
                viewModel.CharacterSymbolCategory = characterPreviewViewModel.CharacterSymbolCategory;
            }
        }
    }
}

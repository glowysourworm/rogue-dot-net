using Rogue.NET.Core.Service.Interface;
using Rogue.NET.ScenarioEditor.ViewModel;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Construction
{
    [Export]
    public partial class DungeonObjectPlacement : UserControl
    {
        readonly IScenarioResourceService _scenarioResourceService;

        [ImportingConstructor]
        public DungeonObjectPlacement(IScenarioResourceService scenarioResourceService)
        {
            _scenarioResourceService = scenarioResourceService;

            InitializeComponent();

            this.DataContextChanged += DungeonObjectPlacement_DataContextChanged;
        }

        private void DungeonObjectPlacement_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var config = e.NewValue as ScenarioConfigurationContainerViewModel;
            if (config == null)
                return;

            this.DataContext = new PlacementGroupViewModel(config
                    .DoodadTemplates
                    .Select(template => new PlacementViewModel()
                    {
                        ImageSource = _scenarioResourceService.GetImageSource(
                                                template.Name,
                                                template.SymbolDetails.CharacterSymbol,
                                                template.SymbolDetails.CharacterColor,
                                                template.SymbolDetails.Icon,
                                                template.SymbolDetails.SmileyMood,
                                                template.SymbolDetails.SmileyBodyColor,
                                                template.SymbolDetails.SmileyLineColor,
                                                template.SymbolDetails.SmileyAuraColor,
                                                template.SymbolDetails.Type),
                        Template = template
                    }));
        }
    }
}

using Rogue.NET.Core.Service.Interface;
using Rogue.NET.ScenarioEditor.ViewModel;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Construction
{
    [Export]
    public partial class ItemPlacement : UserControl
    {
        readonly IScenarioResourceService _scenarioResourceService;

        [ImportingConstructor]
        public ItemPlacement(IScenarioResourceService scenarioResourceService)
        {
            _scenarioResourceService = scenarioResourceService;

            InitializeComponent();

            this.DataContextChanged += ItemPlacement_DataContextChanged;
        }

        private void ItemPlacement_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var config = e.NewValue as ScenarioConfigurationContainerViewModel;
            if (config == null)
                return;

            this.DataContext = new PlacementGroupViewModel(
                                 config
                                    .ConsumableTemplates
                                    .Cast<DungeonObjectTemplateViewModel>()
                                    .Union(
                                     config
                                        .EquipmentTemplates
                                        .Cast<DungeonObjectTemplateViewModel>())
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

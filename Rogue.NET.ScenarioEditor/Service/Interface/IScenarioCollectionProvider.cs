using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Design;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Layout;
using System.Collections.ObjectModel;

namespace Rogue.NET.ScenarioEditor.Service.Interface
{
    /// <summary>
    /// Component that provides getters for the shared collections in the configuration
    /// </summary>
    public interface IScenarioCollectionProvider
    {
        // Level Design Collections
        ObservableCollection<LevelTemplateViewModel> Levels { get; }

        // Assets
        ObservableCollection<PlayerTemplateViewModel> PlayerClasses { get; }
        ObservableCollection<LayoutTemplateViewModel> Layouts { get; }
        ObservableCollection<EnemyTemplateViewModel> Enemies { get; }
        ObservableCollection<FriendlyTemplateViewModel> Friendlies { get; }
        ObservableCollection<EquipmentTemplateViewModel> Equipment { get; }
        ObservableCollection<ConsumableTemplateViewModel> Consumables { get; }
        ObservableCollection<DoodadTemplateViewModel> Doodads { get; }
        ObservableCollection<SkillSetTemplateViewModel> SkillSets { get; }

        // Shared General Assets
        ObservableCollection<AlteredCharacterStateTemplateViewModel> AlteredCharacterStates { get; }
        ObservableCollection<AttackAttributeTemplateViewModel> AttackAttributes { get; }
        ObservableCollection<SymbolPoolItemTemplateViewModel> SymbolPool { get; }

        // Shared Calculated Assets
        ObservableCollection<BrushTemplateViewModel> Brushes { get; }

        // Methods to update Calculated Assets
        void AddBrush(BrushTemplateViewModel brush);
        void RemoveBrush(BrushTemplateViewModel brush);
    }
}

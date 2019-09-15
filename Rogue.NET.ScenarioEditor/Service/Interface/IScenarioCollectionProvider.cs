using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
using System.Collections.ObjectModel;

namespace Rogue.NET.ScenarioEditor.Service.Interface
{
    /// <summary>
    /// Component that provides getters for the shared collections in the configuration
    /// </summary>
    public interface IScenarioCollectionProvider
    {
        // Assets
        ObservableCollection<EnemyTemplateViewModel> Enemies { get; }
        ObservableCollection<FriendlyTemplateViewModel> Friendlies { get; }
        ObservableCollection<EquipmentTemplateViewModel> Equipment { get; }
        ObservableCollection<ConsumableTemplateViewModel> Consumables { get; }
        ObservableCollection<SkillSetTemplateViewModel> SkillSets { get; }

        // Shared General Assets
        ObservableCollection<AlteredCharacterStateTemplateViewModel> AlteredCharacterStates { get; }
        ObservableCollection<PlayerTemplateViewModel> CharacterClasses { get; }

        // Shared Calculated Assets
        ObservableCollection<BrushTemplateViewModel> Brushes { get; }

        // Methods to update Calculated Assets
        void AddBrush(BrushTemplateViewModel brush);
        void RemoveBrush(BrushTemplateViewModel brush);
    }
}

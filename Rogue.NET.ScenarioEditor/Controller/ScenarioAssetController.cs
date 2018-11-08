using Rogue.NET.ScenarioEditor.Controller.Interface;
using Rogue.NET.ScenarioEditor.Service.Interface;
using Rogue.NET.ScenarioEditor.Utility;
using Rogue.NET.ScenarioEditor.ViewModel.Constant;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Layout;
using System;
using System.Collections;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rogue.NET.ScenarioEditor.Controller
{
    [Export(typeof(IScenarioAssetController))]
    public class ScenarioAssetController : IScenarioAssetController
    {
        readonly IScenarioEditorController _scenarioEditorController;
        readonly IScenarioConfigurationUndoService _undoService;
        readonly IScenarioAssetReferenceService _scenarioAssetReferenceService;

        [ImportingConstructor]
        public ScenarioAssetController(
            IScenarioEditorController scenarioEditorController, 
            IScenarioAssetReferenceService scenarioAssetReferenceService,
            IScenarioConfigurationUndoService scenarioConfigurationUndoService)
        {            
            _scenarioEditorController = scenarioEditorController;
            _scenarioAssetReferenceService = scenarioAssetReferenceService;
            _undoService = scenarioConfigurationUndoService;
        }

        /// <summary>
        /// Adds an asset with a pre-calculated name
        /// </summary>
        public void AddAsset(string assetType, string uniqueName)
        {
            switch (assetType)
            {
                case AssetType.Layout:
                    _scenarioEditorController.CurrentConfig.DungeonTemplate.LayoutTemplates.Add(new LayoutTemplateViewModel() { Name = uniqueName });
                    break;
                case AssetType.Enemy:
                    _scenarioEditorController.CurrentConfig.EnemyTemplates.Add(new EnemyTemplateViewModel() { Name = uniqueName });
                    break;
                case AssetType.Equipment:
                    _scenarioEditorController.CurrentConfig.EquipmentTemplates.Add(new EquipmentTemplateViewModel() { Name = uniqueName });
                    break;
                case AssetType.Consumable:
                    _scenarioEditorController.CurrentConfig.ConsumableTemplates.Add(new ConsumableTemplateViewModel() { Name = uniqueName });
                    break;
                case AssetType.Doodad:
                    _scenarioEditorController.CurrentConfig.DoodadTemplates.Add(new DoodadTemplateViewModel() { Name = uniqueName });
                    break;
                case AssetType.Spell:
                    _scenarioEditorController.CurrentConfig.MagicSpells.Add(new SpellTemplateViewModel() { Name = uniqueName });
                    break;
                case AssetType.SkillSet:
                    _scenarioEditorController.CurrentConfig.SkillTemplates.Add(new SkillSetTemplateViewModel() { Name = uniqueName });
                    break;
                case AssetType.Animation:
                    _scenarioEditorController.CurrentConfig.AnimationTemplates.Add(new AnimationTemplateViewModel() { Name = uniqueName });
                    break;
                case AssetType.Brush:
                    _scenarioEditorController.CurrentConfig.BrushTemplates.Add(new BrushTemplateViewModel() { Name = uniqueName });
                    break;
                default:
                    throw new Exception("Unidentified new asset type");
            }

            // NOTE*** HAVE TO BLOCK CHANGES TO THE UNDO STACK TO UPDATE THESE REFERENCES
            _undoService.Block();

            // Update Asset References - (Example: Attack Attributes for enemy or alteration or other affected object)
            _scenarioAssetReferenceService.UpdateAttackAttributes(_scenarioEditorController.CurrentConfig);

            // Restore Undo Service
            _undoService.UnBlock();
        }
        public void RemoveAsset(string assetType, string name)
        {
            var collection = (IList)ConfigurationCollectionResolver.GetAssetCollection(_scenarioEditorController.CurrentConfig, assetType);

            var item = collection.Cast<TemplateViewModel>().First(x => x.Name == name);
            collection.Remove(item);
        }
        public TemplateViewModel GetAsset(string name, string assetType)
        {
            var collection = (IList)ConfigurationCollectionResolver.GetAssetCollection(_scenarioEditorController.CurrentConfig, assetType);

            return collection.Cast<TemplateViewModel>().First(x => x.Name == name);
        }
    }
}

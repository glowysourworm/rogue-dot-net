using Rogue.NET.Common.Extension;
using Rogue.NET.ScenarioEditor.Controller.Interface;
using Rogue.NET.ScenarioEditor.Service.Interface;
using Rogue.NET.ScenarioEditor.Utility;
using Rogue.NET.ScenarioEditor.ViewModel.Constant;
using Rogue.NET.ScenarioEditor.ViewModel.Interface;
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

        // TODO - Move to injection
        readonly ScenarioConfigurationMapper _scenarioConfigurationMapper;

        [ImportingConstructor]
        public ScenarioAssetController(
            IScenarioEditorController scenarioEditorController, 
            IScenarioAssetReferenceService scenarioAssetReferenceService,
            IScenarioConfigurationUndoService scenarioConfigurationUndoService)
        {            
            _scenarioEditorController = scenarioEditorController;
            _scenarioAssetReferenceService = scenarioAssetReferenceService;
            _undoService = scenarioConfigurationUndoService;
            _scenarioConfigurationMapper = new ScenarioConfigurationMapper();
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
            _scenarioAssetReferenceService.UpdateCombatAttributes(_scenarioEditorController.CurrentConfig);

            // Restore Undo Service
            _undoService.UnBlock();
        }
        public void CopyAsset(string assetName, string assetNewName, string assetType)
        {
            var asset = GetAsset(assetName, assetType);
            var assetCopy = asset.DeepClone();

            // Have to give new identity to asset copy
            assetCopy.Guid = Guid.NewGuid().ToString();
            assetCopy.Name = assetNewName;

            switch (assetType)
            {
                case AssetType.Layout:
                    _scenarioEditorController.CurrentConfig.DungeonTemplate.LayoutTemplates.Add(assetCopy as LayoutTemplateViewModel);
                    break;
                case AssetType.Enemy:
                    _scenarioEditorController.CurrentConfig.EnemyTemplates.Add(assetCopy as EnemyTemplateViewModel);
                    break;
                case AssetType.Equipment:
                    _scenarioEditorController.CurrentConfig.EquipmentTemplates.Add(assetCopy as EquipmentTemplateViewModel);
                    break;
                case AssetType.Consumable:
                    _scenarioEditorController.CurrentConfig.ConsumableTemplates.Add(assetCopy as ConsumableTemplateViewModel);
                    break;
                case AssetType.Doodad:
                    _scenarioEditorController.CurrentConfig.DoodadTemplates.Add(assetCopy as DoodadTemplateViewModel);
                    break;
                case AssetType.Spell:
                    _scenarioEditorController.CurrentConfig.MagicSpells.Add(assetCopy as SpellTemplateViewModel);
                    break;
                case AssetType.SkillSet:
                    _scenarioEditorController.CurrentConfig.SkillTemplates.Add(assetCopy as SkillSetTemplateViewModel);
                    break;
                case AssetType.Animation:
                    _scenarioEditorController.CurrentConfig.AnimationTemplates.Add(assetCopy as AnimationTemplateViewModel);
                    break;
                case AssetType.Brush:
                    _scenarioEditorController.CurrentConfig.BrushTemplates.Add(assetCopy as BrushTemplateViewModel);
                    break;
                default:
                    throw new Exception("Unidentified new asset type");
            }

            // NOTE*** HAVE TO BLOCK CHANGES TO THE UNDO STACK TO UPDATE THESE REFERENCES
            _undoService.Block();

            // Fix Asset References - Uses the ScenarioConfigurationMapper to match by GUID
            _scenarioConfigurationMapper.FixReferences(_scenarioEditorController.CurrentConfig);

            // Restore Undo Service
            _undoService.UnBlock();
        }
        public void RemoveAsset(string assetType, string name)
        {
            // Get collection to modify
            var collection = (IList)ConfigurationCollectionResolver.GetAssetCollection(_scenarioEditorController.CurrentConfig, assetType);

            // Modify the collection
            var item = collection.Cast<TemplateViewModel>().First(x => x.Name == name);
            collection.Remove(item);

            // BLOCK CHANGES TO THE UNDO STACK TO UPDATE THESE REFERENCES
            _undoService.Block();

            // Update Asset References
            switch (assetType)
            {
                // No references to update
                case AssetType.Layout:
                case AssetType.Enemy:
                case AssetType.Doodad:
                    break;
                case AssetType.Equipment:
                case AssetType.Consumable:
                    _scenarioAssetReferenceService.UpdateItems(_scenarioEditorController.CurrentConfig);
                    break;
                case AssetType.Spell:
                    _scenarioAssetReferenceService.UpdateAlterations(_scenarioEditorController.CurrentConfig);
                    break;
                case AssetType.SkillSet:
                    _scenarioAssetReferenceService.UpdateSkillSets(_scenarioEditorController.CurrentConfig);
                    break;
                case AssetType.Animation:
                    _scenarioAssetReferenceService.UpdateAnimations(_scenarioEditorController.CurrentConfig);
                    break;
                case AssetType.Brush:
                    _scenarioAssetReferenceService.UpdateBrushes(_scenarioEditorController.CurrentConfig);
                    break;
                default:
                    throw new Exception("Unidentified new asset type");
            }

            // Restore Undo Service
            _undoService.UnBlock();
        }
        public TemplateViewModel GetAsset(string name, string assetType)
        {
            var collection = (IList)ConfigurationCollectionResolver.GetAssetCollection(_scenarioEditorController.CurrentConfig, assetType);

            return collection.Cast<TemplateViewModel>().First(x => x.Name == name);
        }
    }
}

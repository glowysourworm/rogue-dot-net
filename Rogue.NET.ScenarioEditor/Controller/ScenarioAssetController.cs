using Rogue.NET.Common.Extension;
using Rogue.NET.ScenarioEditor.Controller.Interface;
using Rogue.NET.ScenarioEditor.Service.Interface;
using Rogue.NET.ScenarioEditor.Utility;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Consumable;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Doodad;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Equipment;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Layout;
using System;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rogue.NET.ScenarioEditor.Controller
{
    [Export(typeof(IScenarioAssetController))]
    public class ScenarioAssetController : IScenarioAssetController
    {
        readonly IScenarioCollectionProvider _scenarioCollectionProvider;
        readonly IScenarioConfigurationUndoService _undoService;
        readonly IScenarioAssetReferenceService _scenarioAssetReferenceService;

        // TODO - Move to injection
        readonly ScenarioConfigurationMapper _scenarioConfigurationMapper;

        [ImportingConstructor]
        public ScenarioAssetController(
            IScenarioCollectionProvider scenarioCollectionProvider, 
            IScenarioAssetReferenceService scenarioAssetReferenceService,
            IScenarioConfigurationUndoService scenarioConfigurationUndoService)
        {
            _scenarioCollectionProvider = scenarioCollectionProvider;
            _scenarioAssetReferenceService = scenarioAssetReferenceService;
            _undoService = scenarioConfigurationUndoService;
            _scenarioConfigurationMapper = new ScenarioConfigurationMapper();
        }

        /// <summary>
        /// Adds an asset with a pre-calculated name
        /// </summary>
        public bool AddAsset(Type assetType, string uniqueName)
        {
            if (assetType == typeof(PlayerTemplateViewModel))
            {
                if (_scenarioCollectionProvider.PlayerClasses.Any(x => x.Name == uniqueName))
                    return false;
                else
                    _scenarioCollectionProvider.PlayerClasses.Add(new PlayerTemplateViewModel() { Name = uniqueName });
            }

            else if (assetType == typeof(LayoutTemplateViewModel))
            {
                if (_scenarioCollectionProvider.Layouts.Any(x => x.Name == uniqueName))
                    return false;
                else
                    _scenarioCollectionProvider.Layouts.Add(new LayoutTemplateViewModel() { Name = uniqueName });
            }

            else if (assetType == typeof(EnemyTemplateViewModel))
            {
                if (_scenarioCollectionProvider.Enemies.Any(x => x.Name == uniqueName))
                    return false;
                else
                    _scenarioCollectionProvider.Enemies.Add(new EnemyTemplateViewModel() { Name = uniqueName });
            }

            else if (assetType == typeof(FriendlyTemplateViewModel))
            {
                if (_scenarioCollectionProvider.Friendlies.Any(x => x.Name == uniqueName))
                    return false;
                else
                    _scenarioCollectionProvider.Friendlies.Add(new FriendlyTemplateViewModel() { Name = uniqueName });
            }

            else if (assetType == typeof(EquipmentTemplateViewModel))
            {
                if (_scenarioCollectionProvider.Equipment.Any(x => x.Name == uniqueName))
                    return false;
                else
                    _scenarioCollectionProvider.Equipment.Add(new EquipmentTemplateViewModel()
                    {
                        Name = uniqueName,
                        EquipmentAttackAlteration = new EquipmentAttackAlterationTemplateViewModel()
                        {
                            Name = uniqueName + " Attack Effect"
                        },
                        EquipmentCurseAlteration = new EquipmentCurseAlterationTemplateViewModel()
                        {
                            Name = uniqueName + " Curse Effect"
                        },
                        EquipmentEquipAlteration = new EquipmentEquipAlterationTemplateViewModel()
                        {
                            Name = uniqueName + " Effect"
                        }
                    });
            }

            else if (assetType == typeof(ConsumableTemplateViewModel))
            {
                if (_scenarioCollectionProvider.Consumables.Any(x => x.Name == uniqueName))
                    return false;
                else
                    _scenarioCollectionProvider.Consumables.Add(new ConsumableTemplateViewModel()
                    {
                        Name = uniqueName,
                        ConsumableAlteration = new ConsumableAlterationTemplateViewModel()
                        {
                            Name = uniqueName + " Effect"
                        },
                        ConsumableProjectileAlteration = new ConsumableProjectileAlterationTemplateViewModel()
                        {
                            Name = uniqueName + " Throw Effect"
                        }
                    });
            }

            else if (assetType == typeof(DoodadTemplateViewModel))
            {
                if (_scenarioCollectionProvider.Doodads.Any(x => x.Name == uniqueName))
                    return false;
                else
                    _scenarioCollectionProvider.Doodads.Add(new DoodadTemplateViewModel()
                    {
                        Name = uniqueName,
                        InvokedAlteration = new DoodadAlterationTemplateViewModel()
                        {
                            Name = uniqueName + " Use Effect"
                        },
                        AutomaticAlteration = new DoodadAlterationTemplateViewModel()
                        {
                            Name = uniqueName + " Effect"
                        }
                    });
            }

            else if (assetType == typeof(SkillSetTemplateViewModel))
            {
                if (_scenarioCollectionProvider.SkillSets.Any(x => x.Name == uniqueName))
                    return false;
                else
                    _scenarioCollectionProvider.SkillSets.Add(new SkillSetTemplateViewModel() { Name = uniqueName });
            }

            else
                throw new Exception("Unhandled Asset Type ScenarioAssetController");
                    

            // NOTE*** HAVE TO BLOCK CHANGES TO THE UNDO STACK TO UPDATE THESE REFERENCES
            _undoService.Block();

            // Update Asset References - (Example: Attack Attributes for enemy or alteration or other affected object)
            _scenarioAssetReferenceService.UpdateAttackAttributes();

            // Restore Undo Service
            _undoService.UnBlock();

            return true;
        }

        // TODO:SERIALIZATION - ABANDON GUID'S ENTIRELY. GET RID OF ANYTHING IN THE NAMESPACES THAT OVERRIDES 
        //                      THESE THAT DERIVES FROM TEMPLATE.
        public void CopyAsset(string assetName, string assetNewName, Type assetType)
        {
            // Get Existing Asset
            var asset = GetAsset(assetName, assetType);

            // Creating a clone will recreate the object with the same guids; but there's a problem
            // in that it won't match references down the asset graph. SO, THIS DOESN'T WORK 100% OF THE TIME!
            var assetCopy = asset.DeepClone();

            // Have to give new identity to asset copy
            assetCopy.Guid = Guid.NewGuid().ToString();
            assetCopy.Name = assetNewName;

            if (assetType == typeof(PlayerTemplateViewModel))
                _scenarioCollectionProvider.PlayerClasses.Add(assetCopy as PlayerTemplateViewModel);

            else if (assetType == typeof(LayoutTemplateViewModel))
                _scenarioCollectionProvider.Layouts.Add(assetCopy as LayoutTemplateViewModel);

            else if (assetType == typeof(EnemyTemplateViewModel))
                _scenarioCollectionProvider.Enemies.Add(assetCopy as EnemyTemplateViewModel);

            else if (assetType == typeof(FriendlyTemplateViewModel))
                _scenarioCollectionProvider.Friendlies.Add(assetCopy as FriendlyTemplateViewModel);

            else if (assetType == typeof(EquipmentTemplateViewModel))
                _scenarioCollectionProvider.Equipment.Add(assetCopy as EquipmentTemplateViewModel);

            else if (assetType == typeof(ConsumableTemplateViewModel))
                _scenarioCollectionProvider.Consumables.Add(assetCopy as ConsumableTemplateViewModel);

            else if (assetType == typeof(DoodadTemplateViewModel))
                _scenarioCollectionProvider.Doodads.Add(assetCopy as DoodadTemplateViewModel);

            else if (assetType == typeof(SkillSetTemplateViewModel))
                _scenarioCollectionProvider.SkillSets.Add(assetCopy as SkillSetTemplateViewModel);

            else
                throw new Exception("Unhandled Asset Type ScenarioAssetController");
        }
        public void RemoveAsset(Type assetType, string name)
        {
            var asset = GetAsset(name, assetType);

            if (assetType == typeof(PlayerTemplateViewModel))
                _scenarioCollectionProvider.PlayerClasses.Remove(asset as PlayerTemplateViewModel);

            else if (assetType == typeof(LayoutTemplateViewModel))
                _scenarioCollectionProvider.Layouts.Remove(asset as LayoutTemplateViewModel);

            else if (assetType == typeof(EnemyTemplateViewModel))
                _scenarioCollectionProvider.Enemies.Remove(asset as EnemyTemplateViewModel);

            else if (assetType == typeof(FriendlyTemplateViewModel))
                _scenarioCollectionProvider.Friendlies.Remove(asset as FriendlyTemplateViewModel);

            else if (assetType == typeof(EquipmentTemplateViewModel))
                _scenarioCollectionProvider.Equipment.Remove(asset as EquipmentTemplateViewModel);

            else if (assetType == typeof(ConsumableTemplateViewModel))
                _scenarioCollectionProvider.Consumables.Remove(asset as ConsumableTemplateViewModel);

            else if (assetType == typeof(DoodadTemplateViewModel))
                _scenarioCollectionProvider.Doodads.Remove(asset as DoodadTemplateViewModel);

            else if (assetType == typeof(SkillSetTemplateViewModel))
                _scenarioCollectionProvider.SkillSets.Remove(asset as SkillSetTemplateViewModel);

            else
                throw new Exception("Unhandled Asset Type ScenarioAssetController");

            // BLOCK CHANGES TO THE UNDO STACK TO UPDATE THESE REFERENCES
            _undoService.Block();

            if (assetType == typeof(PlayerTemplateViewModel))
            { }

            else if (assetType == typeof(LayoutTemplateViewModel))
                _scenarioAssetReferenceService.UpdateLayouts();

            else if (assetType == typeof(EnemyTemplateViewModel))
                _scenarioAssetReferenceService.UpdateNonPlayerCharacters();

            else if (assetType == typeof(FriendlyTemplateViewModel))
                _scenarioAssetReferenceService.UpdateNonPlayerCharacters();

            else if (assetType == typeof(EquipmentTemplateViewModel))
                _scenarioAssetReferenceService.UpdateItems();

            else if (assetType == typeof(ConsumableTemplateViewModel))
                _scenarioAssetReferenceService.UpdateItems();

            else if (assetType == typeof(DoodadTemplateViewModel))
                _scenarioAssetReferenceService.UpdateDoodads();

            else if (assetType == typeof(SkillSetTemplateViewModel))
                _scenarioAssetReferenceService.UpdateSkillSets();

            else
                throw new Exception("Unhandled Asset Type ScenarioAssetController");

            // Restore Undo Service
            _undoService.UnBlock();
        }
        public TemplateViewModel GetAsset(string name, Type assetType)
        {
            if (assetType == typeof(PlayerTemplateViewModel))
                return _scenarioCollectionProvider.PlayerClasses.FirstOrDefault(x => x.Name == name);

            else if (assetType == typeof(LayoutTemplateViewModel))
                return _scenarioCollectionProvider.Layouts.FirstOrDefault(x => x.Name == name);

            else if (assetType == typeof(EnemyTemplateViewModel))
                return _scenarioCollectionProvider.Enemies.FirstOrDefault(x => x.Name == name);

            else if (assetType == typeof(FriendlyTemplateViewModel))
                return _scenarioCollectionProvider.Friendlies.FirstOrDefault(x => x.Name == name);

            else if (assetType == typeof(EquipmentTemplateViewModel))
                return _scenarioCollectionProvider.Equipment.FirstOrDefault(x => x.Name == name);

            else if (assetType == typeof(ConsumableTemplateViewModel))
                return _scenarioCollectionProvider.Consumables.FirstOrDefault(x => x.Name == name);

            else if (assetType == typeof(DoodadTemplateViewModel))
                return _scenarioCollectionProvider.Doodads.FirstOrDefault(x => x.Name == name);

            else if (assetType == typeof(SkillSetTemplateViewModel))
                return _scenarioCollectionProvider.SkillSets.FirstOrDefault(x => x.Name == name);

            else
                throw new Exception("Unhandled Asset Type ScenarioAssetGroupViewModel");
        }
    }
}

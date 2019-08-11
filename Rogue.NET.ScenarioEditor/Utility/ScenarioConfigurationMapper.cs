using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Consumable;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Doodad;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Enemy;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Equipment;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Skill;
using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Rogue.NET.ScenarioEditor.Utility
{
    /// <summary>
    /// LAST RESORT... I tried AutoMapper, ExpressMapper, TinyMapper, and AgileMapper. Each had an issue
    /// preventing use for mapping the ScenarioConfigurationContainer <--> ScenarioConfigurationContainerViewModel.
    /// 
    /// Mostly, performance was a BIG factor - 4-5 minutes to compile the mappings (first run). The AgileMapper was
    /// by far the best fluent API (next to AutoMapper) - that still performed well; but it didn't support mapping
    /// generics and collections easily. So, configuration would have been less-feasible. I figured it would take
    /// just about as long to write it by hand...
    /// 
    /// So, here we are........ (I'm using AgileMapper here to help out!)
    /// </summary>
    public class ScenarioConfigurationMapper
    {
        // Constructed type maps for configuration name spaces
        private static readonly IDictionary<Type, Type> _forwardTypeMap;
        private static readonly IDictionary<Type, Type> _reverseTypeMap;

        static ScenarioConfigurationMapper()
        {
            _forwardTypeMap = new Dictionary<Type, Type>();
            _reverseTypeMap = new Dictionary<Type, Type>();

            var sourceTypes = typeof(ScenarioConfigurationContainer).Assembly.GetTypes();
            var destTypes = typeof(ScenarioConfigurationContainerViewModel).Assembly.GetTypes();

            // Foward Map
            foreach (var type in sourceTypes)
            {
                var destType = destTypes.FirstOrDefault(x => x.Name == type.Name + "ViewModel");

                if (destType != null)
                    _forwardTypeMap.Add(type, destType);
            }

            // Reverse Map
            foreach (var type in destTypes)
            {
                var sourceType = sourceTypes.FirstOrDefault(x => x.Name + "ViewModel" == type.Name);

                if (sourceType != null)
                    _reverseTypeMap.Add(type, sourceType);
            }
        }

        public ScenarioConfigurationContainerViewModel Map(ScenarioConfigurationContainer model)
        {
            var result = MapObject<ScenarioConfigurationContainer, ScenarioConfigurationContainerViewModel>(model, false);

            return FixReferences(result);
        }
        public ScenarioConfigurationContainer MapBack(ScenarioConfigurationContainerViewModel viewModel)
        {
            var result = MapObject<ScenarioConfigurationContainerViewModel, ScenarioConfigurationContainer>(viewModel, true);

            var configuration = FixReferences(result);

            // Sort collections 
            configuration.AnimationTemplates.Sort((x, y) => x.Name.CompareTo(y.Name));
            configuration.BrushTemplates.Sort((x, y) => x.Name.CompareTo(y.Name));
            configuration.ConsumableTemplates.Sort((x, y) => x.Name.CompareTo(y.Name));
            configuration.DoodadTemplates.Sort((x, y) => x.Name.CompareTo(y.Name));
            configuration.EnemyTemplates.Sort((x, y) => x.Name.CompareTo(y.Name));
            configuration.EquipmentTemplates.Sort((x, y) => x.Name.CompareTo(y.Name));
            configuration.CharacterClasses.Sort((x, y) => x.Name.CompareTo(y.Name));
            configuration.SkillTemplates.Sort((x, y) => x.Name.CompareTo(y.Name));

            configuration.AlterationContainer.ConsumableAlterations.Sort((x, y) => x.Name.CompareTo(y.Name));
            configuration.AlterationContainer.ConsumableProjectileAlterations.Sort((x, y) => x.Name.CompareTo(y.Name));
            configuration.AlterationContainer.DoodadAlterations.Sort((x, y) => x.Name.CompareTo(y.Name));
            configuration.AlterationContainer.EnemyAlterations.Sort((x, y) => x.Name.CompareTo(y.Name));
            configuration.AlterationContainer.EquipmentAttackAlterations.Sort((x, y) => x.Name.CompareTo(y.Name));
            configuration.AlterationContainer.EquipmentCurseAlterations.Sort((x, y) => x.Name.CompareTo(y.Name));
            configuration.AlterationContainer.EquipmentEquipAlterations.Sort((x, y) => x.Name.CompareTo(y.Name));
            configuration.AlterationContainer.SkillAlterations.Sort((x, y) => x.Name.CompareTo(y.Name));

            return configuration;
        }
        
        public TDest MapObject<TSource, TDest>(TSource source, bool reverse)
        {
            // Map base ignoring collections
            var dest = Construct<TDest>();

            // Map collection properties separately
            var destProperties = typeof(TDest).GetProperties()
                                              .ToDictionary(x => x.Name, x => x);

            var sourceProperties = typeof(TSource).GetProperties()
                                                  .ToDictionary(x => x.Name, x => x);

            foreach (var sourceProperty in sourceProperties)
            {
                // Source Property Object
                var sourcePropertyValue = sourceProperty.Value.GetValue(source);

                // Instantiate null properties if they have a default constructor. This
                // will help to fix dangling properties that have been added to the configuration
                if (sourcePropertyValue == null &&
                   !IsValueType(sourceProperty.Value.PropertyType) &&
                   !IsInterface(sourceProperty.Value.PropertyType))
                {
                    // Construct a new property for the source object
                    var sourceNewPropertyValue = Construct(sourceProperty.Value.PropertyType);

                    // Set new property value
                    sourceProperty.Value.SetValue(source, sourceNewPropertyValue);

                    // Reset Source Property Object
                    sourcePropertyValue = sourceNewPropertyValue;
                }

                // Otherwise, handle exception for reference types
                else if (sourcePropertyValue == null &&
                        !IsValueType(sourceProperty.Value.PropertyType) &&
                        !IsInterface(sourceProperty.Value.PropertyType))
                    throw new Exception("Unhandled reference type - probably needs a default constructor");

                // Skip null value types or interface types
                else if (sourcePropertyValue == null &&
                        (IsValueType(sourceProperty.Value.PropertyType) ||
                         IsInterface(sourceProperty.Value.PropertyType)))
                    continue;

                // Have to check collections first
                if (typeof(IList).IsAssignableFrom(sourceProperty.Value.PropertyType))
                {
                    var sourceList = (IList)sourcePropertyValue;

                    if (sourceList.Count <= 0)
                        continue;

                    var destList = (IList)destProperties[sourceProperty.Key].GetValue(dest);

                    // Get Source / Dest Generic Type - MUST BE ONE ONLY
                    var sourceItemType = sourceProperty.Value.PropertyType.GetGenericArguments().First();
                    var destItemType = destProperties[sourceProperty.Key].PropertyType.GetGenericArguments().First();

                    // Call method to map collection items -> Recurses Map<,>
                    MapCollectionInit(sourceList, destList, sourceItemType, destItemType, reverse);
                }

                // Next, Check for Value Types (EXCLUDE INTERFACES)
                else if (IsValueType(sourceProperty.Value.PropertyType) &&
                        !IsInterface(sourceProperty.Value.PropertyType))
                    destProperties[sourceProperty.Key].SetValue(dest, sourcePropertyValue);

                // Non-Collection Complex Types (INCLUDES INTERFACES)
                else
                {
                    // Source / Dest property types
                    var sourcePropertyType = sourceProperty.Value.PropertyType;
                    var destPropertyType = destProperties[sourceProperty.Key].PropertyType;

                    // INTERFACES
                    //
                    // Convention is to have interface types match with a suffix (IType -> ITypeViewModel).
                    // These are found here as the source / dest property types - will have matching interface
                    // names (one with the "ViewModel" suffix).
                    //
                    // We have to identify the underlying matched implementation types using pre-calculated 
                    // type information from the namespaces.

                    if (sourcePropertyType.IsInterface &&
                        destPropertyType.IsInterface)
                    {
                        // Proper IMPLEMENTATION type comes from the actual source value type
                        sourcePropertyType = sourcePropertyValue.GetType();
                        destPropertyType = reverse ? _reverseTypeMap[sourcePropertyType] :
                                                     _forwardTypeMap[sourcePropertyType];

                        // Then, just let the proper types fall through to recurse as they otherwise would.
                    }
                    else if (sourcePropertyType.IsInterface ||
                             destPropertyType.IsInterface)
                        throw new Exception("Mis-matching interface definitions");

                    // Create method call to MapBack<TSource, TDest> using reflection
                    var methodInfo = typeof(ScenarioConfigurationMapper).GetMethod("MapObject");
                    var genericMethodInfo = methodInfo.MakeGenericMethod(sourcePropertyType, destPropertyType);

                    var destObject = genericMethodInfo.Invoke(this, new object[] { sourcePropertyValue, reverse });

                    // Set Dest property
                    destProperties[sourceProperty.Key].SetValue(dest, destObject);
                }
            }

            return dest;
        }

        public void MapCollectionInit(IList sourceCollection, IList destCollection, Type sourceItemType, Type destItemType, bool reverse)
        {
            // Create method call to MapBack<TSource, TDest> using reflection
            var methodInfo = typeof(ScenarioConfigurationMapper).GetMethod("MapCollection");
            var genericMethodInfo = methodInfo.MakeGenericMethod(sourceItemType, destItemType);

            genericMethodInfo.Invoke(this, new object[] { sourceCollection, destCollection, reverse });
        }

        public void MapCollection<TSource, TDest>(IList<TSource> sourceCollection, IList<TDest> destCollection, bool reverse)
        {
            // Create method call to MapBack<TSource, TDest> using reflection
            var methodInfo = typeof(ScenarioConfigurationMapper).GetMethod("MapObject");
            var genericMethodInfo = methodInfo.MakeGenericMethod(typeof(TSource), typeof(TDest));

            // Use Recursion to map back object graph
            foreach (var sourceItem in sourceCollection)
            {
                // Create destination item recursively
                var destItem = (TDest)genericMethodInfo.Invoke(this, new object[] { sourceItem, reverse });

                // Add to destination collection
                destCollection.Add(destItem);
            }
        }

        /// <summary>
        /// Mapping breaks references due to issues with this algorithm. This has to be run prior to returning the object.
        /// </summary>
        public ScenarioConfigurationContainer FixReferences(ScenarioConfigurationContainer configuration)
        {
            // Animations
            foreach (var template in configuration.AnimationTemplates)
            {
                template.FillTemplate = Match(configuration.BrushTemplates, template.FillTemplate);
                template.StrokeTemplate = Match(configuration.BrushTemplates, template.StrokeTemplate);
            }

            // TODO:ALTERATION
            // Alterations
            //foreach (var template in configuration.MagicSpells)
            //{
            //    MatchCollection(configuration.AnimationTemplates, template.Animations);

            //    template.Effect.AlteredState = Match(configuration.AlteredCharacterStates, template.Effect.AlteredState);
            //    template.AuraEffect.AlteredState = Match(configuration.AlteredCharacterStates, template.AuraEffect.AlteredState);

            //    template.Effect.RemediedState = Match(configuration.AlteredCharacterStates, template.Effect.RemediedState);
            //    template.AuraEffect.RemediedState = Match(configuration.AlteredCharacterStates, template.AuraEffect.RemediedState);
            //}

            // Skill Sets
            foreach (var template in configuration.SkillTemplates)
            {
                foreach (var skillTemplate in template.Skills)
                    skillTemplate.SkillAlteration = Match(configuration.AlterationContainer.SkillAlterations, skillTemplate.SkillAlteration);
            }

            // Doodads
            foreach (var template in configuration.DoodadTemplates)
            {
                template.AutomaticAlteration = Match(configuration.AlterationContainer.DoodadAlterations, template.AutomaticAlteration);
                template.InvokedAlteration = Match(configuration.AlterationContainer.DoodadAlterations, template.InvokedAlteration);
            }

            // Consumables
            foreach (var template in configuration.ConsumableTemplates)
            {
                template.LearnedSkill = Match(configuration.SkillTemplates, template.LearnedSkill);
                template.ConsumableAlteration = Match(configuration.AlterationContainer.ConsumableAlterations, template.ConsumableAlteration);
                template.ConsumableProjectileAlteration = Match(configuration.AlterationContainer.ConsumableProjectileAlterations, template.ConsumableProjectileAlteration);
            }

            // Equipment
            foreach (var template in configuration.EquipmentTemplates)
            {
                template.AmmoTemplate = Match(configuration.ConsumableTemplates, template.AmmoTemplate);
                template.EquipmentAttackAlteration = Match(configuration.AlterationContainer.EquipmentAttackAlterations, template.EquipmentAttackAlteration);
                template.EquipmentCurseAlteration = Match(configuration.AlterationContainer.EquipmentCurseAlterations, template.EquipmentCurseAlteration);
                template.EquipmentEquipAlteration = Match(configuration.AlterationContainer.EquipmentEquipAlterations, template.EquipmentEquipAlteration);
            }

            // Enemies
            foreach (var template in configuration.EnemyTemplates)
            {
                for (int i = 0; i < template.StartingConsumables.Count; i++)
                    template.StartingConsumables[i].TheTemplate = Match(configuration.ConsumableTemplates, template.StartingConsumables[i].TheTemplate);

                for (int i = 0; i < template.StartingEquipment.Count; i++)
                    template.StartingEquipment[i].TheTemplate = Match(configuration.EquipmentTemplates, template.StartingEquipment[i].TheTemplate);

                // Behavior Skills
                for (int i = 0; i < template.BehaviorDetails.Behaviors.Count; i++)
                    template.BehaviorDetails.Behaviors[i].EnemyAlteration = Match(configuration.AlterationContainer.EnemyAlterations, template.BehaviorDetails.Behaviors[i].EnemyAlteration);
            }

            // Player
            MatchCollection(configuration.SkillTemplates, configuration.PlayerTemplate.Skills);

            for (int i = 0; i < configuration.PlayerTemplate.StartingConsumables.Count; i++)
                configuration.PlayerTemplate.StartingConsumables[i].TheTemplate = Match(configuration.ConsumableTemplates, configuration.PlayerTemplate.StartingConsumables[i].TheTemplate);

            for (int i = 0; i < configuration.PlayerTemplate.StartingEquipment.Count; i++)
                configuration.PlayerTemplate.StartingEquipment[i].TheTemplate = Match(configuration.EquipmentTemplates, configuration.PlayerTemplate.StartingEquipment[i].TheTemplate);

            return configuration;
        }

        /// <summary>
        /// Mapping breaks references due to issues with this algorithm. This has to be run prior to returning the object.
        /// </summary>
        public ScenarioConfigurationContainerViewModel FixReferences(ScenarioConfigurationContainerViewModel configuration)
        {
            // Animations
            foreach (var template in configuration.AnimationTemplates)
            {
                template.FillTemplate = MatchVM(configuration.BrushTemplates, template.FillTemplate);
                template.StrokeTemplate = MatchVM(configuration.BrushTemplates, template.StrokeTemplate);
            }

            // TODO:ALTERATION
            //// Alterations
            //foreach (var template in configuration.MagicSpells)
            //{
            //    MatchCollectionVM(configuration.AnimationTemplates, template.Animations);

            //    template.Effect.AlteredState = MatchVM(configuration.AlteredCharacterStates, template.Effect.AlteredState);
            //    template.AuraEffect.AlteredState = MatchVM(configuration.AlteredCharacterStates, template.AuraEffect.AlteredState);

            //    template.Effect.RemediedState = MatchVM(configuration.AlteredCharacterStates, template.Effect.RemediedState);
            //    template.AuraEffect.RemediedState = MatchVM(configuration.AlteredCharacterStates, template.AuraEffect.RemediedState);
            //}

            // Skill Sets
            foreach (var template in configuration.SkillTemplates)
            {
                foreach (var skillTemplate in template.Skills)
                    skillTemplate.SkillAlteration = MatchVM(configuration.AlterationContainer.SkillAlterations, skillTemplate.SkillAlteration);
            }

            // Doodads
            foreach (var template in configuration.DoodadTemplates)
            {
                template.AutomaticAlteration = MatchVM(configuration.AlterationContainer.DoodadAlterations, template.AutomaticAlteration);
                template.InvokedAlteration = MatchVM(configuration.AlterationContainer.DoodadAlterations, template.InvokedAlteration);
            }

            // Consumables
            foreach (var template in configuration.ConsumableTemplates)
            {
                template.LearnedSkill = MatchVM(configuration.SkillTemplates, template.LearnedSkill);
                template.ConsumableProjectileAlteration = MatchVM(configuration.AlterationContainer.ConsumableProjectileAlterations, template.ConsumableProjectileAlteration);
                template.ConsumableAlteration = MatchVM(configuration.AlterationContainer.ConsumableAlterations, template.ConsumableAlteration);
            }

            // Equipment
            foreach (var template in configuration.EquipmentTemplates)
            {
                template.AmmoTemplate = MatchVM(configuration.ConsumableTemplates, template.AmmoTemplate);
                template.EquipmentAttackAlteration = MatchVM(configuration.AlterationContainer.EquipmentAttackAlterations, template.EquipmentAttackAlteration);
                template.EquipmentCurseAlteration = MatchVM(configuration.AlterationContainer.EquipmentCurseAlterations, template.EquipmentCurseAlteration);
                template.EquipmentEquipAlteration = MatchVM(configuration.AlterationContainer.EquipmentEquipAlterations, template.EquipmentEquipAlteration);
            }

            // Enemies
            foreach (var template in configuration.EnemyTemplates)
            {
                for (int i = 0; i < template.StartingConsumables.Count; i++)
                    template.StartingConsumables[i].TheTemplate = MatchVM(configuration.ConsumableTemplates, template.StartingConsumables[i].TheTemplate);

                for (int i = 0; i < template.StartingEquipment.Count; i++)
                    template.StartingEquipment[i].TheTemplate = MatchVM(configuration.EquipmentTemplates, template.StartingEquipment[i].TheTemplate);

                // Behavior Skills
                for (int i = 0; i < template.BehaviorDetails.Behaviors.Count; i++)
                    template.BehaviorDetails.Behaviors[i].EnemyAlteration = MatchVM(configuration.AlterationContainer.EnemyAlterations, template.BehaviorDetails.Behaviors[i].EnemyAlteration);
            }

            // Player
            MatchCollectionVM(configuration.SkillTemplates, configuration.PlayerTemplate.Skills);

            for (int i = 0; i < configuration.PlayerTemplate.StartingConsumables.Count; i++)
                configuration.PlayerTemplate.StartingConsumables[i].TheTemplate = MatchVM(configuration.ConsumableTemplates, configuration.PlayerTemplate.StartingConsumables[i].TheTemplate);

            for (int i = 0; i < configuration.PlayerTemplate.StartingEquipment.Count; i++)
                configuration.PlayerTemplate.StartingEquipment[i].TheTemplate = MatchVM(configuration.EquipmentTemplates, configuration.PlayerTemplate.StartingEquipment[i].TheTemplate);

            return configuration;
        }

        private T Match<T>(IList<T> source, T dest) where T : Template
        {
            if (dest == null)
                return dest;

            var item = source.FirstOrDefault(x => x.Guid == dest.Guid);

            // NOTE*** This will prevent null values in the configuration; but these are handled by 
            //         boolean values to show whether they have valid data (and are even used). The reason
            //         for this is to prevent "Null Reference" during mapping. (don't know why)
            return item == null ? dest : item;
        }

        private void MatchCollection<T>(IList<T> source, IList<T> dest) where T : Template
        {
            for (int i = 0; i < dest.Count; i++)
            {
                // Doing this to avoid NotifyCollectionChanged.Replace
                var replaceItem = source.First(x => x.Guid == dest[i].Guid);
                dest.RemoveAt(i);
                dest.Insert(i, replaceItem);
            }
        }

        private T MatchVM<T>(IList<T> source, T dest) where T : TemplateViewModel
        {
            if (dest == null)
                return dest;

            var item = source.FirstOrDefault(x => x.Guid == dest.Guid);

            // NOTE*** This will prevent null values in the configuration; but these are handled by 
            //         boolean values to show whether they have valid data (and are even used). The reason
            //         for this is to prevent "Null Reference" during mapping. (don't know why)
            return item == null ? dest : item;
        }

        private void MatchCollectionVM<T>(IList<T> source, IList<T> dest) where T : TemplateViewModel
        {
            for (int i = 0; i < dest.Count; i++)
            {
                // Doing this to avoid NotifyCollectionChanged.Replace
                var replaceItem = source.First(x => x.Guid == dest[i].Guid);
                dest.RemoveAt(i);
                dest.Insert(i, replaceItem);
            }
        }

        /// <summary>
        /// Creates a new instance of type T using the default constructor
        /// </summary>
        private T Construct<T>()
        {
            var constructor = typeof(T).GetConstructor(new Type[] { });

            return (T)(constructor == null ? default(T) : constructor.Invoke(new object[] { }));
        }

        /// <summary>
        /// Creates a new instance of the specified type using the default constructor
        /// </summary>
        private object Construct(Type type)
        {
            var constructor = type.GetConstructor(new Type[] { });

            return (constructor == null ? null : constructor.Invoke(new object[] { }));
        }

        private bool IsValueType(Type type)
        {
            return (type.GetConstructor(new Type[] { }) == null);
        }

        private bool IsInterface(Type type)
        {
            return type.IsInterface;
        }
    }
}

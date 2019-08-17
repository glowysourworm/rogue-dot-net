using Rogue.NET.Common.Extension;
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
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Common;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
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
            configuration.ConsumableTemplates.Sort((x, y) => x.Name.CompareTo(y.Name));
            configuration.DoodadTemplates.Sort((x, y) => x.Name.CompareTo(y.Name));
            configuration.EnemyTemplates.Sort((x, y) => x.Name.CompareTo(y.Name));
            configuration.EquipmentTemplates.Sort((x, y) => x.Name.CompareTo(y.Name));
            configuration.CharacterClasses.Sort((x, y) => x.Name.CompareTo(y.Name));
            configuration.SkillTemplates.Sort((x, y) => x.Name.CompareTo(y.Name));

            //foreach (var template in configuration.ConsumableTemplates)
            //{
            //    template.AmmoAnimationGroup.TargetType = AlterationTargetType.Target;
            //    template.ConsumableAlteration.AnimationGroup.TargetType = template.ConsumableAlteration.TargetType;
            //    template.ConsumableProjectileAlteration.AnimationGroup.TargetType = AlterationTargetType.Target;
            //}

            //foreach (var template in configuration.DoodadTemplates)
            //{
            //    template.AutomaticAlteration.AnimationGroup.TargetType = template.AutomaticAlteration.TargetType;
            //    template.InvokedAlteration.AnimationGroup.TargetType = template.InvokedAlteration.TargetType;
            //}

            //foreach (var template in configuration.EquipmentTemplates)
            //    template.EquipmentAttackAlteration.AnimationGroup.TargetType = AlterationTargetType.Target;

            //foreach (var template in configuration.EnemyTemplates)
            //    foreach (var behavior in template.BehaviorDetails.Behaviors)
            //        behavior.EnemyAlteration.AnimationGroup.TargetType = behavior.EnemyAlteration.TargetType;

            //foreach (var template in configuration.SkillTemplates)
            //    foreach (var skill in template.Skills)
            //        skill.SkillAlteration.AnimationGroup.TargetType = skill.SkillAlteration.TargetType;

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

#if DEBUG_CONFIGURATION_SOURCE_INSTANTIATE

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

                //Otherwise, handle exception for reference types
                else if (sourcePropertyValue == null &&
                        !IsValueType(sourceProperty.Value.PropertyType) &&
                        !IsInterface(sourceProperty.Value.PropertyType))
                    throw new Exception("Unhandled reference type - probably needs a default constructor");

                //Skip null value types or interface types
                else if (sourcePropertyValue == null &&
                        (IsValueType(sourceProperty.Value.PropertyType) ||
                         IsInterface(sourceProperty.Value.PropertyType)))
                    continue;
#else
                // Skip null values
                if (sourcePropertyValue == null)
                    continue;
#endif

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
            // Have to fix collections that are SHARED:  { Altered Character States, Character Classes }
            //
            // Attack Attributes are copied to new collections so don't have to be maintained.
            //
            // Also, have to fix Assets that are SHARED: 
            //
            // Consumable:  { Learned Skill }                                        and { Character Class }
            // Equipment:   { Ammo Template }                                        and { Character Class }
            // Doodad:                                                                   { Character Class }
            // Skill Set:                                                                { Character Class }
            // Enemy:       { Starting Consumables, Starting Equipment }             
            // Player:      { Skill Sets, Starting Consumables, Starting Equipment } 
            //

            // FIRST:  Do the Assets to set proper references
            //
            // SECOND: Do Alteration Effects (Just need to fix Altered Character State)

            #region Assets - { Consumable Skill, Equipment Ammo, Enemy Items, Player Items, Player Skills } AND { Character Classes }

            // Consumables
            foreach (var template in configuration.ConsumableTemplates)
            {
                template.LearnedSkill = Match(configuration.SkillTemplates, template.LearnedSkill);
                template.CharacterClass = Match(configuration.CharacterClasses, template.CharacterClass);
            }

            // Equipment
            foreach (var template in configuration.EquipmentTemplates)
            {
                template.AmmoTemplate = Match(configuration.ConsumableTemplates, template.AmmoTemplate);
                template.CharacterClass = Match(configuration.CharacterClasses, template.CharacterClass);
            }

            // Doodad
            foreach (var template in configuration.DoodadTemplates)
                template.CharacterClass = Match(configuration.CharacterClasses, template.CharacterClass);

            // Skill Sets
            foreach (var template in configuration.SkillTemplates.SelectMany(x => x.Skills))
                template.CharacterClass = Match(configuration.CharacterClasses, template.CharacterClass);

            // Enemies
            foreach (var template in configuration.EnemyTemplates)
            {
                for (int i = 0; i < template.StartingConsumables.Count; i++)
                    template.StartingConsumables[i].TheTemplate = Match(configuration.ConsumableTemplates, template.StartingConsumables[i].TheTemplate);

                for (int i = 0; i < template.StartingEquipment.Count; i++)
                    template.StartingEquipment[i].TheTemplate = Match(configuration.EquipmentTemplates, template.StartingEquipment[i].TheTemplate);
            }

            // Player
            MatchCollection(configuration.SkillTemplates, configuration.PlayerTemplate.Skills);

            for (int i = 0; i < configuration.PlayerTemplate.StartingConsumables.Count; i++)
                configuration.PlayerTemplate.StartingConsumables[i].TheTemplate = Match(configuration.ConsumableTemplates, configuration.PlayerTemplate.StartingConsumables[i].TheTemplate);

            for (int i = 0; i < configuration.PlayerTemplate.StartingEquipment.Count; i++)
                configuration.PlayerTemplate.StartingEquipment[i].TheTemplate = Match(configuration.EquipmentTemplates, configuration.PlayerTemplate.StartingEquipment[i].TheTemplate);

            #endregion

            #region Alteration Effects - Fix up references { Altered States, Brushes }
            var consumableFunc = new Func<ConsumableTemplate, IEnumerable<IAlterationEffectTemplate>>(consumable =>
            {
                var list = new List<IEnumerable<IAlterationEffectTemplate>>()
                {
                    configuration.ConsumableTemplates.Select(x => x.ConsumableAlteration.Effect),
                    configuration.ConsumableTemplates.Select(x => x.ConsumableProjectileAlteration.Effect),
                    configuration.ConsumableTemplates.SelectMany(x => x.LearnedSkill.Skills.Select(z => z.SkillAlteration.Effect))
                };

                return list.SelectMany(x => x);
            });

            var equipmentFunc = new Func<EquipmentTemplate, IEnumerable<IAlterationEffectTemplate>>(consumable =>
            {
                var list = new List<IEnumerable<IAlterationEffectTemplate>>()
                {
                    configuration.EquipmentTemplates.Select(x => x.EquipmentAttackAlteration.Effect),
                    configuration.EquipmentTemplates.Select(x => x.EquipmentCurseAlteration.Effect),
                    configuration.EquipmentTemplates.Select(x => x.EquipmentEquipAlteration.Effect)
                };

                return list.SelectMany(x => x);
            });

            var alterationEffects = new List<IEnumerable<IAlterationEffectTemplate>>()
            {
                configuration.ConsumableTemplates.SelectMany(x => consumableFunc(x)),
                configuration.EquipmentTemplates.SelectMany(x => equipmentFunc(x)),
                configuration.DoodadTemplates.Select(x => x.AutomaticAlteration.Effect),
                configuration.DoodadTemplates.Select(x => x.InvokedAlteration.Effect),
                configuration.EnemyTemplates.SelectMany(x => x.BehaviorDetails.Behaviors.Select(z => z.EnemyAlteration.Effect)),
                //configuration.EnemyTemplates.SelectMany(x => x.StartingConsumables.SelectMany(z => consumableFunc(z.TheTemplate))),
                //configuration.EnemyTemplates.SelectMany(x => x.StartingEquipment.SelectMany(z => equipmentFunc(z.TheTemplate))),
                configuration.SkillTemplates.SelectMany(x => x.Skills.Select(z => z.SkillAlteration.Effect)),
                //configuration.PlayerTemplate.Skills.SelectMany(x => x.Skills.Select(z => z.SkillAlteration.Effect)),
                //configuration.PlayerTemplate.StartingConsumables.SelectMany(x => consumableFunc(x.TheTemplate)),
                //configuration.PlayerTemplate.StartingEquipment.SelectMany(x => equipmentFunc(x.TheTemplate)),
            }.SelectMany(x => x)
             .Actualize();

            // Altered States
            foreach (var alterationEffect in alterationEffects)
            {
                if (alterationEffect is AttackAttributeTemporaryAlterationEffectTemplate)
                {
                    var effect = (alterationEffect as AttackAttributeTemporaryAlterationEffectTemplate);

                    effect.AlteredState = Match(configuration.AlteredCharacterStates, effect.AlteredState);
                }
                else if (alterationEffect is TemporaryAlterationEffectTemplate)
                {
                    var effect = (alterationEffect as TemporaryAlterationEffectTemplate);

                    effect.AlteredState = Match(configuration.AlteredCharacterStates, effect.AlteredState);
                }
            }

            #endregion

            return configuration;
        }

        /// <summary>
        /// Mapping breaks references due to issues with this algorithm. This has to be run prior to returning the object.
        /// </summary>
        public ScenarioConfigurationContainerViewModel FixReferences(ScenarioConfigurationContainerViewModel configuration)
        {
            // Have to fix collections that are SHARED:  { Altered Character States, Character Classes }
            //
            // Attack Attributes are copied to new collections so don't have to be maintained.
            //
            // Also, have to fix Assets that are SHARED: 
            //
            // Consumable:  { Learned Skill }                                        and { Character Class }
            // Equipment:   { Ammo Template }                                        and { Character Class }
            // Doodad:                                                                   { Character Class }
            // Skill Set:                                                                { Character Class }
            // Enemy:       { Starting Consumables, Starting Equipment }             
            // Player:      { Skill Sets, Starting Consumables, Starting Equipment } 
            //

            // FIRST:  Do the Assets to set proper references
            //
            // SECOND: Do Alteration Effects (Just need to fix Altered Character State)

            #region Assets - { Consumable Skill, Equipment Ammo, Enemy Items, Player Items, Player Skills } AND { Character Classes }

            // Consumables
            foreach (var template in configuration.ConsumableTemplates)
            {
                template.LearnedSkill = MatchVM(configuration.SkillTemplates, template.LearnedSkill);
                template.CharacterClass = MatchVM(configuration.CharacterClasses, template.CharacterClass);
            }

            // Equipment
            foreach (var template in configuration.EquipmentTemplates)
            {
                template.AmmoTemplate = MatchVM(configuration.ConsumableTemplates, template.AmmoTemplate);
                template.CharacterClass = MatchVM(configuration.CharacterClasses, template.CharacterClass);
            }

            // Doodad
            foreach (var template in configuration.DoodadTemplates)
                template.CharacterClass = MatchVM(configuration.CharacterClasses, template.CharacterClass);

            // Skill Sets
            foreach (var template in configuration.SkillTemplates.SelectMany(x => x.Skills))
                template.CharacterClass = MatchVM(configuration.CharacterClasses, template.CharacterClass);

            // Enemies
            foreach (var template in configuration.EnemyTemplates)
            {
                for (int i = 0; i < template.StartingConsumables.Count; i++)
                    template.StartingConsumables[i].TheTemplate = MatchVM(configuration.ConsumableTemplates, template.StartingConsumables[i].TheTemplate);

                for (int i = 0; i < template.StartingEquipment.Count; i++)
                    template.StartingEquipment[i].TheTemplate = MatchVM(configuration.EquipmentTemplates, template.StartingEquipment[i].TheTemplate);
            }

            // Player
            MatchCollectionVM(configuration.SkillTemplates, configuration.PlayerTemplate.Skills);

            for (int i = 0; i < configuration.PlayerTemplate.StartingConsumables.Count; i++)
                configuration.PlayerTemplate.StartingConsumables[i].TheTemplate = MatchVM(configuration.ConsumableTemplates, configuration.PlayerTemplate.StartingConsumables[i].TheTemplate);

            for (int i = 0; i < configuration.PlayerTemplate.StartingEquipment.Count; i++)
                configuration.PlayerTemplate.StartingEquipment[i].TheTemplate = MatchVM(configuration.EquipmentTemplates, configuration.PlayerTemplate.StartingEquipment[i].TheTemplate);

            #endregion

            #region Alteration Effects - Fix up references { Altered States, Brushes }
            var consumableFunc = new Func<ConsumableTemplateViewModel, IEnumerable<IAlterationEffectTemplateViewModel>>(consumable =>
            {
                var list = new List<IEnumerable<IAlterationEffectTemplateViewModel>>()
                {
                    configuration.ConsumableTemplates.Select(x => x.ConsumableAlteration.Effect),
                    configuration.ConsumableTemplates.Select(x => x.ConsumableProjectileAlteration.Effect),
                    configuration.ConsumableTemplates.SelectMany(x => x.LearnedSkill.Skills.Select(z => z.SkillAlteration.Effect))
                };

                return list.SelectMany(x => x);
            });

            var equipmentFunc = new Func<EquipmentTemplateViewModel, IEnumerable<IAlterationEffectTemplateViewModel>>(consumable =>
            {
                var list = new List<IEnumerable<IAlterationEffectTemplateViewModel>>()
                {
                    configuration.EquipmentTemplates.Select(x => x.EquipmentAttackAlteration.Effect),
                    configuration.EquipmentTemplates.Select(x => x.EquipmentCurseAlteration.Effect),
                    configuration.EquipmentTemplates.Select(x => x.EquipmentEquipAlteration.Effect)
                };

                return list.SelectMany(x => x);
            });

            var alterationEffects = new List<IEnumerable<IAlterationEffectTemplateViewModel>>()
            {
                configuration.ConsumableTemplates.SelectMany(x => consumableFunc(x)),
                configuration.EquipmentTemplates.SelectMany(x => equipmentFunc(x)),
                configuration.DoodadTemplates.Select(x => x.AutomaticAlteration.Effect),
                configuration.DoodadTemplates.Select(x => x.InvokedAlteration.Effect),
                configuration.EnemyTemplates.SelectMany(x => x.BehaviorDetails.Behaviors.Select(z => z.EnemyAlteration.Effect)),
                //configuration.EnemyTemplates.SelectMany(x => x.StartingConsumables.SelectMany(z => consumableFunc(z.TheTemplate))),
                //configuration.EnemyTemplates.SelectMany(x => x.StartingEquipment.SelectMany(z => equipmentFunc(z.TheTemplate))),
                configuration.SkillTemplates.SelectMany(x => x.Skills.Select(z => z.SkillAlteration.Effect)),
                //configuration.PlayerTemplate.Skills.SelectMany(x => x.Skills.Select(z => z.SkillAlteration.Effect)),
                //configuration.PlayerTemplate.StartingConsumables.SelectMany(x => consumableFunc(x.TheTemplate)),
                //configuration.PlayerTemplate.StartingEquipment.SelectMany(x => equipmentFunc(x.TheTemplate)),
            }.SelectMany(x => x)
             .Actualize();

            // Altered States
            foreach (var alterationEffect in alterationEffects)
            {
                if (alterationEffect is AttackAttributeTemporaryAlterationEffectTemplateViewModel)
                {
                    var effect = (alterationEffect as AttackAttributeTemporaryAlterationEffectTemplateViewModel);

                    effect.AlteredState = MatchVM(configuration.AlteredCharacterStates, effect.AlteredState);
                }
                else if (alterationEffect is TemporaryAlterationEffectTemplateViewModel)
                {
                    var effect = (alterationEffect as TemporaryAlterationEffectTemplateViewModel);

                    effect.AlteredState = MatchVM(configuration.AlteredCharacterStates, effect.AlteredState);
                }
            }

            #endregion

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

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
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation;
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
    /// So, here we are........ (I'm using AgileMapper here to help out!) (Nope.. AgileMapper also abandoned)
    /// </summary>
    public class ScenarioConfigurationMapper
    {
        // Constructed type maps for configuration name spaces
        private static readonly IDictionary<Type, Type> _forwardTypeMap;
        private static readonly IDictionary<Type, Type> _reverseTypeMap;

        // Reference map used to track duplicate references in the source and
        // replicate them in the destination.
        private IDictionary<Template, TemplateViewModel> _forwardReferenceMap;
        private IDictionary<TemplateViewModel, Template> _reverseReferenceMap;

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

        public ScenarioConfigurationMapper()
        {
            _forwardReferenceMap = new Dictionary<Template, TemplateViewModel>();
            _reverseReferenceMap = new Dictionary<TemplateViewModel, Template>();
        }

        public ScenarioConfigurationContainerViewModel Map(ScenarioConfigurationContainer model, out IEnumerable<BrushTemplateViewModel> scenarioBrushes)
        {
            // Clear out reference maps to prepare for next mapping
            _forwardReferenceMap.Clear();
            _reverseReferenceMap.Clear();

            // Map configuration
            var configuration = MapObject<ScenarioConfigurationContainer, ScenarioConfigurationContainerViewModel>(model, false);

            // Collect brushes from forward reference map
            scenarioBrushes = _forwardReferenceMap.Values
                                                  .Where(x => x is BrushTemplateViewModel)
                                                  .Select(x => x as BrushTemplateViewModel)
                                                  .ToList();

            return configuration;
        }
        public ScenarioConfigurationContainer MapBack(ScenarioConfigurationContainerViewModel viewModel)
        {
            // Clear out reference maps to prepare for next mapping
            _forwardReferenceMap.Clear();
            _reverseReferenceMap.Clear();

            var result = MapObject<ScenarioConfigurationContainerViewModel, ScenarioConfigurationContainer>(viewModel, true);

            return result;
        }
        
        public TDest MapObject<TSource, TDest>(TSource source, bool reverse) where TSource : class
                                                                             where TDest : class
        {
            // ***Reference Tracking
            //
            //    Store references as a hash to track them to recreate the source
            //    object graph. 
            //
            //    When there's a duplicate source found - terminate recursion and
            //    return the mapped destination object.
            //

            var referenceFound = false;

            // Search for Forward Reference 
            if (!reverse && source is Template)
            {
                var template = source as Template;

                // No Reference Found - Store a new destination object with this source object
                if (!_forwardReferenceMap.ContainsKey(template))
                    _forwardReferenceMap.Add(template, Construct<TDest>() as TemplateViewModel);

                // Reference Found - Return the previously mapped destination object
                else
                    return _forwardReferenceMap[template] as TDest;

                referenceFound = true;
            }

            // Search for Reverse Reference
            else if (reverse && source is TemplateViewModel)
            {
                var templateViewModel = source as TemplateViewModel;

                // No Reference Found - Store a new destination object with this source object
                if (!_reverseReferenceMap.ContainsKey(templateViewModel))
                    _reverseReferenceMap.Add(templateViewModel, Construct<TDest>() as Template);

                // Reference Found - Return the previously mapped destination object
                else
                    return _reverseReferenceMap[templateViewModel] as TDest;

                referenceFound = true;
            }

            // Fetch the newly created destination object (OR) Create new destination object - continue recursion
            var dest = referenceFound ? (!reverse ? _forwardReferenceMap[source as Template] as TDest :
                                                   _reverseReferenceMap[source as TemplateViewModel] as TDest)
                                      : Construct<TDest>();

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

                // Check for invalid enum values
                if (sourcePropertyValue != null &&
                    sourceProperty.Value.PropertyType.IsEnum &&
                    !Enum.IsDefined(sourceProperty.Value.PropertyType, sourcePropertyValue) &&
                     Enum.GetValues(sourceProperty.Value.PropertyType).Length > 0)
                {
                    // Set the sourcePropertyValue and source property to the default enum value
                    var defaultEnumValue = Enum.GetValues(sourceProperty.Value.PropertyType).GetValue(0);

                    // Set new property value
                    sourceProperty.Value.SetValue(source, defaultEnumValue);

                    // Reset Source Property Object
                    sourcePropertyValue = defaultEnumValue;
                }

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

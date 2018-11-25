using AgileObjects.AgileMapper;
using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Layout;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
        public ScenarioConfigurationContainerViewModel Map(ScenarioConfigurationContainer model)
        {
            // Create an instance mapper and ignore collections (have to be done by hand - not clear support
            // for generics - need to specify mapping between List <--> ObservableCollection
            var mapper = Mapper.CreateNew();

            // IGNORE COLLECTIONS
            mapper.WhenMapping
                  .IgnoreTargetMembersOfType<IList>();

            var result = MapRecurse<ScenarioConfigurationContainer, ScenarioConfigurationContainerViewModel>(model, mapper);

            return FixReferences(result);
        }
        public ScenarioConfigurationContainer MapBack(ScenarioConfigurationContainerViewModel viewModel)
        {
            // Create an instance mapper and ignore collections (have to be done by hand - not clear support
            // for generics - need to specify mapping between List <--> ObservableCollection
            var mapper = Mapper.CreateNew();

            // IGNORE COLLECTIONS
            mapper.WhenMapping
                  .IgnoreTargetMembersOfType<IList>();

            var result = MapRecurse<ScenarioConfigurationContainerViewModel, ScenarioConfigurationContainer>(viewModel, mapper);

            return FixReferences(result);
        }

        public TDest MapRecurse<TSource, TDest>(TSource source, IMapper mapper)
        {
            // Map base ignoring collections
            var dest = mapper.Map(source).ToANew<TDest>();

            // Map collection properties separately
            var destProperties = typeof(TDest).GetProperties()
                                                        .ToDictionary(x => x.Name, x => x);

            var sourceProperties = typeof(TSource).GetProperties()
                                                            .ToDictionary(x => x.Name, x => x);

            foreach (var sourceProperty in sourceProperties)
            {
                // Can skip value types
                if (sourceProperty.Value.PropertyType.IsValueType)
                    continue;

                // HAVE TO SKIP NULL PROPERTIES (TBD)
                if (sourceProperty.Value.GetValue(source) == null)
                    continue;

                // Non-Collection Complext Types
                if (!typeof(IList).IsAssignableFrom(sourceProperty.Value.PropertyType))
                {
                    // Source property to recurse
                    var sourceObject = sourceProperty.Value.GetValue(source);

                    // Source / Dest property types
                    var sourceObjectType = sourceProperty.Value.PropertyType;
                    var destObjectType = destProperties[sourceProperty.Key].PropertyType;

                    // Create method call to MapBack<TSource, TDest> using reflection
                    var methodInfo = typeof(ScenarioConfigurationMapper).GetMethod("MapRecurse");
                    var genericMethodInfo = methodInfo.MakeGenericMethod(sourceObjectType, destObjectType);

                    var destObject = genericMethodInfo.Invoke(this, new object[] { sourceObject, mapper });

                    // Set Dest property
                    destProperties[sourceProperty.Key].SetValue(dest, destObject);
                }
                // Collection
                else
                {
                    var sourceList = (IList)sourceProperty.Value.GetValue(source);

                    if (sourceList.Count <= 0)
                        continue;

                    var destList = (IList)destProperties[sourceProperty.Key].GetValue(dest);

                    // Get Source / Dest Generic Type - MUST BE ONE ONLY
                    var sourceItemType = sourceProperty.Value.PropertyType.GetGenericArguments().First();
                    var destItemType = destProperties[sourceProperty.Key].PropertyType.GetGenericArguments().First();

                    // Call method to map collection items -> Recurses Map<,>
                    MapCollection(sourceList, destList, sourceItemType, destItemType, mapper);
                }
            }

            return dest;
        }

        public void MapCollection(IList sourceCollection, IList destCollection, Type sourceItemType, Type destItemType, IMapper mapper)
        {
            // Create method call to MapBack<TSource, TDest> using reflection
            var methodInfo = typeof(ScenarioConfigurationMapper).GetMethod("MapRecurse");
            var genericMethodInfo = methodInfo.MakeGenericMethod(sourceItemType, destItemType);

            // Use Recursion to map back object graph
            foreach (var sourceItem in sourceCollection)
            {
                // Create destination item recursively
                var destItem = genericMethodInfo.Invoke(this, new object[] { sourceItem, mapper });

                // Add to destination collection
                destCollection.Add(destItem);
            }
        }

        /// <summary>
        /// Mapping breaks references due to issues with this algorithm. This has to be run prior to returning the object.
        /// </summary>
        private ScenarioConfigurationContainer FixReferences(ScenarioConfigurationContainer configuration)
        {
            // Animations
            foreach (var template in configuration.AnimationTemplates)
            {
                template.FillTemplate = Match(configuration.BrushTemplates, template.FillTemplate);
                template.StrokeTemplate = Match(configuration.BrushTemplates, template.StrokeTemplate);
            }

            // Alterations
            foreach (var template in configuration.MagicSpells)
            {
                MatchCollection(configuration.AnimationTemplates, template.Animations);
            }

            // Skill Sets
            foreach (var template in configuration.SkillTemplates)
            {
                MatchCollection(configuration.MagicSpells, template.Spells);
            }

            // Doodads
            foreach (var template in configuration.DoodadTemplates)
            {
                template.AutomaticMagicSpellTemplate = Match(configuration.MagicSpells, template.AutomaticMagicSpellTemplate);
                template.InvokedMagicSpellTemplate = Match(configuration.MagicSpells, template.InvokedMagicSpellTemplate);
            }

            // Consumables
            foreach (var template in configuration.ConsumableTemplates)
            {
                template.AmmoSpellTemplate = Match(configuration.MagicSpells, template.AmmoSpellTemplate);
                template.LearnedSkill = Match(configuration.SkillTemplates, template.LearnedSkill);
                template.ProjectileSpellTemplate = Match(configuration.MagicSpells, template.ProjectileSpellTemplate);
                template.SpellTemplate = Match(configuration.MagicSpells, template.SpellTemplate);
            }

            // Equipment
            foreach (var template in configuration.EquipmentTemplates)
            {
                template.AmmoTemplate = Match(configuration.ConsumableTemplates, template.AmmoTemplate);
                template.CurseSpell = Match(configuration.MagicSpells, template.CurseSpell);
                template.EquipSpell = Match(configuration.MagicSpells, template.EquipSpell);
            }

            // Enemies
            foreach (var template in configuration.EnemyTemplates)
            {
                for (int i=0;i<template.StartingConsumables.Count;i++)
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

            return configuration;
        }

        /// <summary>
        /// Mapping breaks references due to issues with this algorithm. This has to be run prior to returning the object.
        /// </summary>
        private ScenarioConfigurationContainerViewModel FixReferences(ScenarioConfigurationContainerViewModel configuration)
        {
            // Animations
            foreach (var template in configuration.AnimationTemplates)
            {
                template.FillTemplate = MatchVM(configuration.BrushTemplates, template.FillTemplate);
                template.StrokeTemplate = MatchVM(configuration.BrushTemplates, template.StrokeTemplate);
            }

            // Alterations
            foreach (var template in configuration.MagicSpells)
            {
                MatchCollectionVM(configuration.AnimationTemplates, template.Animations);
            }

            // Skill Sets
            foreach (var template in configuration.SkillTemplates)
            {
                MatchCollectionVM(configuration.MagicSpells, template.Spells);
            }

            // Doodads
            foreach (var template in configuration.DoodadTemplates)
            {
                template.AutomaticMagicSpellTemplate = MatchVM(configuration.MagicSpells, template.AutomaticMagicSpellTemplate);
                template.InvokedMagicSpellTemplate = MatchVM(configuration.MagicSpells, template.InvokedMagicSpellTemplate);
            }

            // Consumables
            foreach (var template in configuration.ConsumableTemplates)
            {
                template.AmmoSpellTemplate = MatchVM(configuration.MagicSpells, template.AmmoSpellTemplate);
                template.LearnedSkill = MatchVM(configuration.SkillTemplates, template.LearnedSkill);
                template.ProjectileSpellTemplate = MatchVM(configuration.MagicSpells, template.ProjectileSpellTemplate);
                template.SpellTemplate = MatchVM(configuration.MagicSpells, template.SpellTemplate);
            }

            // Equipment
            foreach (var template in configuration.EquipmentTemplates)
            {
                template.AmmoTemplate = MatchVM(configuration.ConsumableTemplates, template.AmmoTemplate);
                template.CurseSpell = MatchVM(configuration.MagicSpells, template.CurseSpell);
                template.EquipSpell = MatchVM(configuration.MagicSpells, template.EquipSpell);
            }

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

            return configuration;
        }

        private T Match<T>(IList<T> source, T dest) where T : Template
        {
            var item = source.FirstOrDefault(x => x.Guid == dest.Guid);

            // NOTE*** This will prevent null values in the configuration; but these are handled by 
            //         boolean values to show whether they have valid data (and are even used). The reason
            //         for this is to prevent "Null Reference" during mapping. (don't know why)
            return item == null ? dest : item;
        }

        private void MatchCollection<T>(IList<T> source, IList<T> dest) where T : Template
        {
            for (int i=0;i< dest.Count;i++)
                dest[i] = source.First(x => x.Guid == dest[i].Guid);
        }

        private T MatchVM<T>(IList<T> source, T dest) where T : TemplateViewModel
        {
            var item = source.FirstOrDefault(x => x.Guid == dest.Guid);

            // NOTE*** This will prevent null values in the configuration; but these are handled by 
            //         boolean values to show whether they have valid data (and are even used). The reason
            //         for this is to prevent "Null Reference" during mapping. (don't know why)
            return item == null ? dest : item;
        }

        private void MatchCollectionVM<T>(IList<T> source, IList<T> dest) where T : TemplateViewModel
        {
            for (int i = 0; i < dest.Count; i++)
                dest[i] = source.First(x => x.Guid == dest[i].Guid);
        }
    }
}

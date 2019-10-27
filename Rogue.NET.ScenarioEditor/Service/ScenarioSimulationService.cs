using Rogue.NET.Core.Processing.Model.Static;
using Rogue.NET.ScenarioEditor.Service.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.Overview;
using Rogue.NET.ScenarioEditor.ViewModel.Overview.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Design;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Layout;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rogue.NET.ScenarioEditor.Service
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IScenarioSimulationService))]
    public class ScenarioSimulationService : IScenarioSimulationService
    {
        // Probability = P_i = (probability of getting the ith "bin") = Weight_i  / Sum(Weight_j)
        //
        // Expectation Value = E[X] = Sum( x_i * p_i )  where x_i is the vector representing indicies of the assets
        //
        // Variance = E[X^2] - E[X]^2 = Sum ((x_i)^2 * p_i) - E[X]^2 
        //

        public ScenarioSimulationService() { }

        public IProjectionSetViewModel CalculateProjectedExperience(IEnumerable<LevelTemplateViewModel> levels)
        {
            // First calculate enemy experience
            var projectionSet = CalculateProjectionSet(
                                        levels,
                                        new Func<TemplateViewModel, double>(enemyGenerationTemplate => (enemyGenerationTemplate as EnemyGenerationTemplateViewModel).GenerationWeight),
                                        new Func<LevelBranchTemplateViewModel, IEnumerable<TemplateViewModel>>(levelBranch => levelBranch.Enemies),
                                        new Func<TemplateViewModel, EnemyTemplateViewModel>(enemyGenerationTemplate => (enemyGenerationTemplate as EnemyGenerationTemplateViewModel).Asset),
                                        new Func<TemplateViewModel, double>(enemyTemplate => (enemyTemplate as EnemyTemplateViewModel).ExperienceGiven.GetAverage()),
                                        new Func<LevelBranchTemplateViewModel, int>(levelBranch => levelBranch.EnemyGenerationRange.GetAverage()),
                                        true);

            // Use results to calculate expected player level
            for (int i = 0; i < projectionSet.Count; i++)
            {
                var projection = projectionSet.GetProjection(i);

                for (int j = 0; j < projection.Value.Count(); j++)
                {
                    var playerLevel = 0;
                    while (RogueCalculator.CalculateExperienceNext(playerLevel) < projection.Value.ElementAt(j).Mean)
                        playerLevel++;

                    // Setting the mean value to be the player level
                    projection.Value.ElementAt(j).Mean = playerLevel;
                }
            }

            return projectionSet;
        }

        public IProjectionSetViewModel CalculateProjectedGeneration(IEnumerable<LevelTemplateViewModel> levels, TemplateViewModel asset, bool cummulative)
        {
            if (asset is LayoutTemplateViewModel)
            {
                return CalculateProjectionSet(
                        levels,
                        new Func<TemplateViewModel, double>(layoutGenerationTemplate => (layoutGenerationTemplate as LayoutGenerationTemplateViewModel).GenerationWeight),
                        new Func<LevelBranchTemplateViewModel, IEnumerable<TemplateViewModel>>(levelBranch => levelBranch.Layouts),
                        new Func<TemplateViewModel, LayoutTemplateViewModel>(layoutGenerationTemplate => (layoutGenerationTemplate as LayoutGenerationTemplateViewModel).Asset),
                        new Func<TemplateViewModel, double>(layoutTemplate =>
                        {
                            // Moment selector selects this asset from the asset collection to include in the calculation
                            return layoutTemplate == asset ? 1.0 : 0.0;
                        }),
                        new Func<LevelBranchTemplateViewModel, int>(levelBranch => 1),
                        cummulative);
            }
            else if (asset is EnemyTemplateViewModel)
            {
                return CalculateProjectionSet(
                        levels,
                        new Func<TemplateViewModel, double>(enemyGenerationTemplate => (enemyGenerationTemplate as EnemyGenerationTemplateViewModel).GenerationWeight),
                        new Func<LevelBranchTemplateViewModel, IEnumerable<TemplateViewModel>>(levelBranch => levelBranch.Enemies),
                        new Func<TemplateViewModel, EnemyTemplateViewModel>(enemyGenerationTemplate => (enemyGenerationTemplate as EnemyGenerationTemplateViewModel).Asset),
                        new Func<TemplateViewModel, double>(enemyTemplate =>
                        {
                            // Moment selector selects this asset from the asset collection to include in the calculation
                            return enemyTemplate == asset ? 1.0 : 0.0;
                        }),
                        new Func<LevelBranchTemplateViewModel, int>(levelBranch => levelBranch.EnemyGenerationRange.GetAverage()),
                        cummulative);
            }
            else if (asset is FriendlyTemplateViewModel)
            {
                return CalculateProjectionSet(
                        levels,
                        new Func<TemplateViewModel, double>(friendlyGenerationTemplate => (friendlyGenerationTemplate as FriendlyGenerationTemplateViewModel).GenerationWeight),
                        new Func<LevelBranchTemplateViewModel, IEnumerable<TemplateViewModel>>(levelBranch => levelBranch.Friendlies),
                        new Func<TemplateViewModel, FriendlyTemplateViewModel>(friendlyGenerationTemplate => (friendlyGenerationTemplate as FriendlyGenerationTemplateViewModel).Asset),
                        new Func<TemplateViewModel, double>(friendlyTemplate =>
                        {
                            // Moment selector selects this asset from the asset collection to include in the calculation
                            return friendlyTemplate == asset ? 1.0 : 0.0;
                        }),
                        new Func<LevelBranchTemplateViewModel, int>(levelBranch => levelBranch.FriendlyGenerationRange.GetAverage()),
                        cummulative);
            }
            else if (asset is EquipmentTemplateViewModel)
            {
                return CalculateProjectionSet(
                        levels,
                        new Func<TemplateViewModel, double>(equipmentGenerationTemplate => (equipmentGenerationTemplate as EquipmentGenerationTemplateViewModel).GenerationWeight),
                        new Func<LevelBranchTemplateViewModel, IEnumerable<TemplateViewModel>>(levelBranch => levelBranch.Equipment),
                        new Func<TemplateViewModel, EquipmentTemplateViewModel>(equipmentGenerationTemplate => (equipmentGenerationTemplate as EquipmentGenerationTemplateViewModel).Asset),
                        new Func<TemplateViewModel, double>(equipmentTemplate =>
                        {
                            // Moment selector selects this asset from the asset collection to include in the calculation
                            return equipmentTemplate == asset ? 1.0 : 0.0;
                        }),
                        new Func<LevelBranchTemplateViewModel, int>(levelBranch => levelBranch.EquipmentGenerationRange.GetAverage()),
                        cummulative);
            }
            else if (asset is ConsumableTemplateViewModel)
            {
                return CalculateProjectionSet(
                        levels,
                        new Func<TemplateViewModel, double>(consumableGenerationTemplate => (consumableGenerationTemplate as ConsumableGenerationTemplateViewModel).GenerationWeight),
                        new Func<LevelBranchTemplateViewModel, IEnumerable<TemplateViewModel>>(levelBranch => levelBranch.Consumables),
                        new Func<TemplateViewModel, ConsumableTemplateViewModel>(consumableGenerationTemplate => (consumableGenerationTemplate as ConsumableGenerationTemplateViewModel).Asset),
                        new Func<TemplateViewModel, double>(consumableTemplate =>
                        {
                            // Moment selector selects this asset from the asset collection to include in the calculation
                            return consumableTemplate == asset ? 1.0 : 0.0;
                        }),
                        new Func<LevelBranchTemplateViewModel, int>(levelBranch => levelBranch.ConsumableGenerationRange.GetAverage()),
                        cummulative);
            }
            else if (asset is DoodadTemplateViewModel)
            {
                return CalculateProjectionSet(
                        levels,
                        new Func<TemplateViewModel, double>(doodadGenerationTemplate => (doodadGenerationTemplate as DoodadGenerationTemplateViewModel).GenerationWeight),
                        new Func<LevelBranchTemplateViewModel, IEnumerable<TemplateViewModel>>(levelBranch => levelBranch.Doodads),
                        new Func<TemplateViewModel, DoodadTemplateViewModel>(doodadGenerationTemplate => (doodadGenerationTemplate as DoodadGenerationTemplateViewModel).Asset),
                        new Func<TemplateViewModel, double>(doodadTemplate =>
                        {
                            // Moment selector selects this asset from the asset collection to include in the calculation
                            return doodadTemplate == asset ? 1.0 : 0.0;
                        }),
                        new Func<LevelBranchTemplateViewModel, int>(levelBranch => levelBranch.DoodadGenerationRange.GetAverage()),
                        cummulative);
            }
            else
                throw new Exception("Unhandled asset type ScenarioSimulationService");
        }

        /// <summary>
        /// METHOD USED TO SIMPLIFY CALCULATIONS. CAREFULLY READ EACH VARIABLE EXPLANATION TO SEE HOW TO CALCULATE THE MOMENTS.
        /// </summary>
        /// <param name="levels">Level collection</param>
        /// <param name="assetWeightSelector">Selects generation weight from the asset - (MUST CAST BEFORE HAND)</param>
        /// <param name="assetCollectionSelector">Selects asset collection from level branch</param>
        /// <param name="generationAssetAssetSelector">Selects the ASSET from the ASSET GENERATION TEMPLATE (TRICKY!!! PLEASE NOTE!)</param>
        /// <param name="momentSelector">Selects the probability mass WEIGHTING for the projected quantity (Example:  Enemy Experience)</param>
        /// <param name="generationExpectedValueSelector">Expected generation number (Example: Enemy Generation Range - USE THE AVERAGE)</param>
        /// <param name="separateBranches">Either combine into a single quantity - or separate per branch</param>
        /// <returns>IProjectionSetViewModel ready for viewing.</returns>
        private IProjectionSetViewModel CalculateProjectionSet(
                    IEnumerable<LevelTemplateViewModel> levels,
                    Func<TemplateViewModel, double> assetWeightSelector,
                    Func<LevelBranchTemplateViewModel, IEnumerable<TemplateViewModel>> assetCollectionSelector,
                    Func<TemplateViewModel, TemplateViewModel> generationAssetAssetSelector,
                    Func<TemplateViewModel, double> momentSelector,
                    Func<LevelBranchTemplateViewModel, int> generationExpectedValueSelector,
                    bool cummulative)
        {
            // Probability = Level Branch Generation Probability * Asset Generation Probability
            //
            // Expectation Value = Sum( weight * probability )
            //
            // Generation # = Expectation of (Random[Low, High]) = Range.GetAverage() for the asset
            //
            // Expected Value = (Generation #) * (Experience Expectation Value)
            //

            var projections = levels.Select((level, index) =>
            {
                // Calculate probabilities for each branch
                var branchProbabilities = level.LevelBranches.Select(x =>
                {
                    return new
                    {
                        Branch = x,
                        Probability = CalculateProbability(level.LevelBranches,
                                                            x,
                                                            z => (z as LevelBranchGenerationTemplateViewModel).GenerationWeight)
                    };
                });

                // Calculate probabilities for each enemy asset (WITHIN ITS ENEMY COLLECTION)
                var assetProbabilities = branchProbabilities.SelectMany(x =>
                {
                    return assetCollectionSelector(x.Branch.LevelBranch).Select(theAsset =>
                    {
                        return new
                        {
                            Branch = x.Branch,
                            BranchProbability = x.Probability,
                            Asset = theAsset,
                            AssetProbability = CalculateProbability(assetCollectionSelector(x.Branch.LevelBranch),
                                                                    theAsset,
                                                                    z => assetWeightSelector(z))
                        };
                    });
                });

                // Calculate the total probability (Level Branch Probability * Enemy Probability)
                var totalProbabilities = assetProbabilities.Select(x => new { Branch = x.Branch, Asset = generationAssetAssetSelector(x.Asset), Probability = x.AssetProbability * x.BranchProbability });

                // Calculate experience expectation value
                var moments = totalProbabilities.Select(x =>
                {
                    // NOTE*** GENERATION EXPECTED VALUE USED HERE
                    return new
                    {
                        Branch = x.Branch,
                        ValueMoment = generationExpectedValueSelector(x.Branch.LevelBranch) * momentSelector(x.Asset) * x.Probability,
                        ValueSecondMoment = generationExpectedValueSelector(x.Branch.LevelBranch) * Math.Pow(momentSelector(x.Asset), 2) * x.Probability
                    };
                });

                // For separate branches, group by branch and sum over the sub-quantity
                return new ProjectedQuantityViewModel()
                {
                    Level = index + 1,
                    Mean = moments.Sum(x => x.ValueMoment),
                    Variance = moments.Sum(x => x.ValueSecondMoment) - Math.Pow(moments.Sum(x => x.ValueMoment), 2),
                    SeriesName = level.Name + " Combined Branches"
                };
            });

            var projectionSet = new ProjectionSetViewModel();

            if (projections.Any())
            {
                // Accumulate results for cummulative projections
                if (cummulative)
                {
                    // Should be using moving average
                    var cummulativeQuantity = 0.0;
                    for (int i = 0; i < projections.Count(); i++)
                    {
                        cummulativeQuantity += projections.ElementAt(i).Mean;

                        projections.ElementAt(i).Mean = cummulativeQuantity;

                        // NOTE*** Not using the variance at the moment
                    }
                }

                projectionSet.Add(projections.First().SeriesName, projections);
            }

            return projectionSet;
        }

        private double CalculateProbability(IEnumerable<TemplateViewModel> collection, TemplateViewModel template, Func<TemplateViewModel, double> weightSelector)
        {
            // Calculate the sum of the weights
            var sumWeights = collection.Sum(x => weightSelector(x));

            // Get the asset weight
            var assetWeight = weightSelector(template);

            // Probability = w_i / Sum(w_i) = Asset Weight / Sum of Weights
            return assetWeight / sumWeights;
        }
    }
}

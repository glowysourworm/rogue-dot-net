using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Math;
using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;
using Rogue.NET.Core.Processing.Model.Algorithm;
using Rogue.NET.Core.Processing.Model.Algorithm.Component;
using Rogue.NET.Core.Processing.Model.Content.Calculator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Model.Scenario.Dynamic.Layout
{
    /// <summary>
    /// Component used for storing and calculating 
    /// </summary>
    public class CharacterMovement
    {
        readonly LayoutGrid _layoutGrid;
        readonly ContentGrid _contentGrid;
        

        // Character visibility calculations PER REGION (RE-CREATED EACH TIME CHARACTER ENTERS NEW REGION)
        Dictionary<NonPlayerCharacter, CharacterMovementPlanner> _searchDict;

        // Player - Content Visibility
        Dictionary<ScenarioObject, ScenarioObject> _contentVisibilityDict;

        // Store instances of the dijkstra path finder
        Dictionary<NonPlayerCharacter, DijkstraPathFinder> _pathFinderDict;

        // Primary visibility information for the Player
        Dictionary<GridLocation, GridLocation> _visibleLocations;
        Dictionary<GridLocation, GridLocation> _exploredLocations;
        Dictionary<GridLocation, GridLocation> _revealedLocations;

        // Expose the primary grid boundary
        public RegionBoundary Boundary { get { return _layoutGrid.Bounds; } }

        /// <summary>
        /// Constructor for the CharacterLayoutInformation should be called once per
        /// level and updated on each turn.
        /// </summary>
        public CharacterMovement(LayoutGrid layoutGrid, ContentGrid contentGrid)
        {
            _layoutGrid = layoutGrid;
            _contentGrid = contentGrid;

            _searchDict = new Dictionary<NonPlayerCharacter, CharacterMovementPlanner>();
            _contentVisibilityDict = new Dictionary<ScenarioObject, ScenarioObject>();

            _pathFinderDict = new Dictionary<NonPlayerCharacter, DijkstraPathFinder>();

            _visibleLocations = new Dictionary<GridLocation, GridLocation>();
            _exploredLocations = new Dictionary<GridLocation, GridLocation>();
            _revealedLocations = new Dictionary<GridLocation, GridLocation>();

            // Initialize Explored / Revealed locations
            _exploredLocations = _layoutGrid.FullMap
                                            .GetLocations()
                                            .Where(x => _layoutGrid[x].IsExplored)
                                            .ToDictionary(x => x, x => x);

            _revealedLocations = _layoutGrid.FullMap
                                            .GetLocations()
                                            .Where(x => _layoutGrid[x].IsRevealed)
                                            .ToDictionary(x => x, x => x);
        }

        public void Update(Player player, GridLocation playerLocation)
        {
            // Player - Calculate Revealed / Explored locations
            //

            // Clear current visibility
            _visibleLocations.Clear();
            _contentVisibilityDict.Clear();

            // Calculate visible locations
            VisibilityCalculator.CalculateVisibility(_layoutGrid, playerLocation, (column, row, isVisible) =>
            {
                // Visible Cells -> Explored / No Longer Revealed
                if (isVisible)
                {
                    // Set properties from the primary
                    var cell = _layoutGrid[column, row];

                    cell.IsExplored = true;
                    cell.IsRevealed = false;

                    // Update player collections (~ O(1) each call)
                    //
                    // Visible
                    if (!_visibleLocations.ContainsKey(cell.Location))
                        _visibleLocations.Add(cell.Location, cell.Location);

                    // Explored
                    if (!_exploredLocations.ContainsKey(cell.Location))
                        _exploredLocations.Add(cell.Location, cell.Location);

                    // Revealed
                    if (_revealedLocations.ContainsKey(cell.Location))
                        _revealedLocations.Remove(cell.Location);

                    // Set content visibility
                    foreach (var content in _contentGrid[column, row])
                    {
                        if (!_contentVisibilityDict.ContainsKey(content))
                            _contentVisibilityDict.Add(content, content);
                    }
                }
            });

            // TODO: REMOVE THIS 
            _revealedLocations = _layoutGrid.FullMap
                                            .GetLocations()
                                            .Where(location => _layoutGrid[location].IsRevealed)
                                            .ToDictionary(x => x, x => x);
        }

        public void Update(NonPlayerCharacter nonPlayerCharacter, GridLocation nonPlayerCharacterLocation)
        {
            // Add a new entry for this character
            if (!_searchDict.ContainsKey(nonPlayerCharacter))
            {
                // TODO:BEHAVIOR - Create linear scale mapping [0, 1] -> [Min, Max] search radius
                // TODO:BEHAVIOR - Move these calculations; and centralize rest period constant (X number of turns) globally
                var searchRadius = (int)(nonPlayerCharacter.BehaviorDetails.SearchRadiusRatio * MathFunctions.Max(_layoutGrid.Bounds.Width, _layoutGrid.Bounds.Height));
                var restPeriod = (int)(nonPlayerCharacter.BehaviorDetails.RestCoefficient * 30);

                switch (nonPlayerCharacter.BehaviorDetails.RestBehaviorType)
                {
                    case CharacterRestBehaviorType.HomeLocation:
                        _searchDict.Add(nonPlayerCharacter, new CharacterMovementPlanner(_layoutGrid, _contentGrid.GetHomeLocation(nonPlayerCharacter), searchRadius, restPeriod));
                        break;
                    case CharacterRestBehaviorType.CurrentLocation:
                        _searchDict.Add(nonPlayerCharacter, new CharacterMovementPlanner(_layoutGrid, searchRadius, restPeriod));
                        break;
                    default:
                        throw new Exception("Unhandled CharacterRestBehaviorType CharacterMovement.Update");
                }
            }

            // Fetch the current search planner
            var planner = _searchDict[nonPlayerCharacter];

            // Update visibility for the character
            planner.Update(nonPlayerCharacterLocation);
        }

        public bool IsPathToAdjacentLocationBlocked(GridLocation location1,
                                            GridLocation location2,
                                            bool includeBlockedByCharacters,
                                            CharacterAlignmentType excludedAlignmentType = CharacterAlignmentType.None)
        {
            var cell1 = _layoutGrid[location1];
            var cell2 = _layoutGrid[location2];

            if (cell1 == null || cell2 == null)
                return true;

            if (_layoutGrid.ImpassableTerrainMap[location2] != null)
                return true;

            // NOTE*** Optimized for performance *** GetAt<> is slow
            //
            // Check that the cell is occupied by a character of the other faction
            var contents1 = _contentGrid[cell1];
            var contents2 = _contentGrid[cell2];

            var character = contents2.FirstOrDefault(content => content is NonPlayerCharacter) as NonPlayerCharacter;

            if (character != null &&
                includeBlockedByCharacters &&
                character.AlignmentType != excludedAlignmentType)
                return true;

            var direction = GridCalculator.GetDirectionOfAdjacentLocation(location1, location2);

            switch (direction)
            {
                case Compass.N:
                case Compass.S:
                case Compass.E:
                case Compass.W:
                    return cell2.IsWall;

                case Compass.NE:
                case Compass.NW:
                case Compass.SE:
                case Compass.SW:
                    {
                        Compass cardinal1;
                        Compass cardinal2;

                        var diag1 = _layoutGrid.GetOffDiagonalCell1(location1, direction, out cardinal1);
                        var diag2 = _layoutGrid.GetOffDiagonalCell2(location1, direction, out cardinal2);

                        if (diag1 == null || diag2 == null)
                            return true;

                        // NOTE*** Optimized for performance *** GetAt<> is slow
                        //
                        var contentsDiag1 = _contentGrid[diag1];
                        var contentsDiag2 = _contentGrid[diag2];

                        var characters1 = contentsDiag1.Where(content => content is CharacterBase);
                        var characters2 = contentsDiag2.Where(content => content is CharacterBase);

                        bool b1 = (diag1 == null);
                        bool b2 = (diag2 == null);

                        if (diag1 != null)
                        {
                            b1 |= diag1.IsWall;
                            b1 |= cell2.IsWall;

                            if (includeBlockedByCharacters)
                            {
                                switch (excludedAlignmentType)
                                {
                                    case CharacterAlignmentType.PlayerAligned:
                                        b1 |= characters1.Any(character => (character is NonPlayerCharacter) &&
                                                                           (character as NonPlayerCharacter).AlignmentType == CharacterAlignmentType.EnemyAligned);
                                        break;
                                    case CharacterAlignmentType.EnemyAligned:
                                        b1 |= characters1.Any(character => (character is Player) ||
                                                                           ((character is NonPlayerCharacter) &&
                                                                            (character as NonPlayerCharacter).AlignmentType == CharacterAlignmentType.PlayerAligned));
                                        break;
                                    case CharacterAlignmentType.None:
                                        b1 |= characters1.Any();
                                        break;
                                    default:
                                        throw new Exception("Unhandled Alignment Type PathGrid.IsPathToAdjacentLocationBlocked");
                                }
                            }
                        }
                        if (diag2 != null)
                        {
                            b2 |= diag2.IsWall;
                            b2 |= cell2.IsWall;

                            if (includeBlockedByCharacters)
                            {
                                switch (excludedAlignmentType)
                                {
                                    case CharacterAlignmentType.PlayerAligned:
                                        b2 |= characters2.Any(character => (character is NonPlayerCharacter) &&
                                                                           (character as NonPlayerCharacter).AlignmentType == CharacterAlignmentType.EnemyAligned);
                                        break;
                                    case CharacterAlignmentType.EnemyAligned:
                                        b2 |= characters2.Any(character => (character is Player) ||
                                                                           ((character is NonPlayerCharacter) &&
                                                                            (character as NonPlayerCharacter).AlignmentType == CharacterAlignmentType.PlayerAligned));
                                        break;
                                    case CharacterAlignmentType.None:
                                        b2 |= characters2.Any();
                                        break;
                                    default:
                                        throw new Exception("Unhandled Alignment Type PathGrid.IsPathToAdjacentLocationBlocked");
                                }
                            }
                        }

                        // Both paths are blocked
                        return b1 || b2;
                    }
            }
            return false;
        }

        public GridLocation CalculateNextLocation(NonPlayerCharacter character)
        {
            return CalculateSearchLocation(character);
        }

        public bool IsVisibleTo(IGridLocator location, NonPlayerCharacter character)
        {
            // ~ O(1)
            return _searchDict[character].IsVisible(_layoutGrid[location].Location);
        }

        /// <summary>
        /// Returns true if the content is visible to the Player
        /// </summary>
        public bool IsVisible(ScenarioObject scenarioObject)
        {
            // ~ O(1)
            return _contentVisibilityDict.ContainsKey(scenarioObject);
        }

        public IEnumerable<GridLocation> GetVisibleLocations(CharacterBase character)
        {
            if (character is Player)
                return GetVisibleLocations();

            else
                return GetVisibleLocations(character as NonPlayerCharacter);
        }

        public IEnumerable<GridLocation> GetVisibleLocations(NonPlayerCharacter character)
        {
            return _searchDict[character].VisibleLocations;
        }

        public IEnumerable<GridLocation> GetVisibleLocations()
        {
            return _visibleLocations.Values;
        }

        public IEnumerable<GridLocation> GetExploredLocations()
        {
            return _exploredLocations.Values;
        }

        public IEnumerable<GridLocation> GetRevealedLocations()
        {
            return _revealedLocations.Values;
        }

        private GridLocation CalculateSearchLocation(NonPlayerCharacter character)
        {
            // Return current (rest) position if resting
            if (_searchDict[character].IsResting())
                return _contentGrid[character];

            // Fetch the next search location
            var goalLocation = _searchDict[character].GetNextSearchLocation(_contentGrid[character]);

            // Check to see if there is any more locations to search
            //
            // TODO: Change this to allow character to move to a new region (OR) reset the search grid
            if (goalLocation == null)
                return _contentGrid[character];

            // Setup the Dijkstra path finder
            var dijkstraMap = GetDijkstraPathFinder(character, new GridLocation[] { goalLocation });

            // Run the Dijkstra scan
            dijkstraMap.Run();

            return dijkstraMap.GetNextPathLocation(goalLocation);
        }

        private GridLocation CalculateAttackGroupLocation(NonPlayerCharacter character)
        {
            // Calculate the attack goals
            var attackGoals = _contentGrid.NonPlayerCharacters
                                          .Where(otherCharacter => otherCharacter.AlignmentType != character.AlignmentType)
                                          .Cast<CharacterBase>()
                                          .ToList();

            // Calculate other goals
            var groupGoals = _contentGrid.NonPlayerCharacters
                                         .Where(otherCharacter => otherCharacter.AlignmentType == character.AlignmentType &&
                                                                  otherCharacter != character)
                                         .Cast<CharacterBase>()
                                         .ToList();

            // Add the Player to the appropriate goals collection
            if (character.AlignmentType == CharacterAlignmentType.EnemyAligned)
                attackGoals.Add(_contentGrid.Characters
                                            .Where(otherCharacter => otherCharacter is Player)
                                            .First());

            else
                groupGoals.Add(_contentGrid.Characters
                                           .Where(otherCharacter => otherCharacter is Player)
                                           .First());

            var goalLocations = attackGoals.Union(groupGoals)
                                           .Select(content => _contentGrid[content])
                                           .Actualize();

            // Fetch the Dijkstra path finder
            var dijkstraMap = GetDijkstraPathFinder(character, goalLocations);

            // Run the Dijkstra scan
            dijkstraMap.Run();

            // PAYOFF = GOAL REWARD - MOVEMENT COST
            GridLocation maxPayoffLocation = null;
            double maxPayoff = double.MinValue;

            foreach (var goal in attackGoals)
            {
                // TODO:BEHAVIOR - FIND A WAY TO CALCULATE GOAL REWARDS
                var goalReward = 50;
                var payoff = goalReward - dijkstraMap.GetMovementCost(_contentGrid[goal]);

                if (payoff > maxPayoff)
                {
                    maxPayoff = payoff;
                    maxPayoffLocation = _contentGrid[goal];
                }
            }

            foreach (var goal in groupGoals)
            {
                // TODO:BEHAVIOR - FIND A WAY TO CALCULATE GOAL REWARDS
                var goalReward = 5;
                var payoff = goalReward - dijkstraMap.GetMovementCost(_contentGrid[goal]);

                if (payoff > maxPayoff)
                {
                    maxPayoff = payoff;
                    maxPayoffLocation = _contentGrid[goal];
                }
            }

            // Calculate the next path location for this goal
            if (maxPayoffLocation != null)
                return dijkstraMap.GetNextPathLocation(maxPayoffLocation);

            // DEFAULT - return the character location (doesn't move)
            else
                return _contentGrid[character];
        }

        private DijkstraPathFinder GetDijkstraPathFinder(NonPlayerCharacter character, IEnumerable<GridLocation> goalLocations)
        {
            DijkstraPathFinder dijkstraMap;

            // Store the Dijkstra path finder for this character
            if (_pathFinderDict.ContainsKey(character))
                dijkstraMap = _pathFinderDict[character];

            else
            {
                // Create Dijkstra map
                dijkstraMap = new DijkstraPathFinder(_layoutGrid, _contentGrid[character], goalLocations, (column1, row1, column2, row2) =>
                {
                    var cell1 = _layoutGrid[column1, row1];
                    var cell2 = _layoutGrid[column2, row2];

                    if (cell1 == null ||
                        cell2 == null)
                        return true;

                    if (_layoutGrid.WalkableMap[column2, row2] != null &&
                       !IsPathToAdjacentLocationBlocked(cell1.Location,
                                                        cell2.Location,
                                                        true,
                                                        character.AlignmentType))
                        return false;

                    else
                        return true;
                });

                // Store the Dijkstra path finder
                _pathFinderDict.Add(character, dijkstraMap);
            }

            // Setup the map for use with the new source / target locations
            dijkstraMap.Reset(_contentGrid[character], goalLocations);

            return dijkstraMap;
        }
    }
}

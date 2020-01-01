using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Processing.Model.Algorithm.Component;
using Rogue.NET.Core.Processing.Model.Content.Calculator;
using System;
using System.Linq;

namespace Rogue.NET.Core.Model.Scenario.Dynamic.Layout
{
    /// <summary>
    /// Stores collections of pre-calculated data for use when deciding character goals and movements (Path finding)
    /// </summary>
    public class PathGrid
    {
        readonly LayoutGrid _layoutGrid;
        readonly ContentGrid _contentGrid;

        public PathGrid(LayoutGrid layoutGrid, ContentGrid contentGrid)
        {
            _layoutGrid = layoutGrid;
            _contentGrid = contentGrid;
        }

        public GridLocation CalculateNextLocation(NonPlayerCharacter character)
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

            // Run the Dijkstra scan
            var dijkstraMap = new DijkstraPathFinder(_layoutGrid, _contentGrid[character], goalLocations, (column1, row1, column2, row2) =>
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
                                        b1 |= characters1.Any(character =>  (character is Player) ||
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
    }
}

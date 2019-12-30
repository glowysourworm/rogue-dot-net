using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Event;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content.Doodad;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration.Design;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

using CharacterBase = Rogue.NET.Core.Model.Scenario.Character.Character;

namespace Rogue.NET.Core.Model.Scenario.Content
{
    [Serializable]
    public class Level : ISerializable
    {
        readonly LevelContent _levelContent;
        readonly LayoutGrid _grid;

        /// <summary>
        /// The primary layout grid for the level
        /// </summary>
        public LayoutGrid Grid
        {
            get { return _grid; }
        }

        /// <summary>
        /// Primary Player reference
        /// </summary>
        public Player Player { get { return _levelContent.Player; } }

        /// <summary>
        /// Parameters left from level generation
        /// </summary>
        public LevelParameters Parameters { get; protected set; }

        public Level(LayoutTemplate layout,
                     LevelBranchTemplate levelBranch,
                     LayoutGrid grid,
                     int number)
        {
            _grid = grid;
            _levelContent = new LevelContent(grid);

            this.Parameters = new LevelParameters()
            {
                Number = number,
                LevelBranchName = levelBranch.Name,
                LayoutName = layout.Name,
                EnemyGenerationPerStep = levelBranch.MonsterGenerationPerStep
            };
        }

        public Level(SerializationInfo info, StreamingContext context)
        {
            _grid = (LayoutGrid)info.GetValue("Grid", typeof(LayoutGrid));
            this.Parameters = (LevelParameters)info.GetValue("Parameters", typeof(LevelParameters));

            var count = info.GetInt32("ContentCount");

            // Instantiate the level content
            _levelContent = new LevelContent(_grid);

            // Deserialize the content
            for (int i = 0; i < count; i++)
            {
                var scenarioObject = (ScenarioObject)info.GetValue("Content" + i.ToString(), typeof(ScenarioObject));
                var location = (GridLocation)info.GetValue("Location" + i.ToString(), typeof(GridLocation));

                _levelContent.AddContent(scenarioObject, location);
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Grid", _grid);
            info.AddValue("Parameters", this.Parameters);

            // Serialize the number of content entries
            if (_levelContent.Player != null)
                info.AddValue("ContentCount", _levelContent.AllContent.Count() - 1);

            else
                info.AddValue("ContentCount", _levelContent.AllContent.Count());

            var counter = 0;

            // Serialize the content
            foreach (var content in _levelContent.AllContent)
            {
                // *** SERIALIZE EVERYTHING EXCEPT THE PLAYER
                if (content is Player)
                    continue;

                // Store the object and its location
                info.AddValue("Content" + counter, content);
                info.AddValue("Location" + counter++, _levelContent[content.Id]);
            }
        }

        /// <summary>
        /// Loads level with player reference and any other extracted contents from the previous level
        /// </summary>
        public void Load(Player player, GridLocation location, IEnumerable<ScenarioObject> extractedContent)
        {
            // TODO: NEEDS FURTHER DESIGN WHEN CHANGING LEVELS. FOR NOW, JUST PUT CONTENT WHERE PLAYER IS STANDING.

            // Add extrated content (from previous level) to this level. Example: Friendly characters
            foreach (var scenarioObject in extractedContent)
                _levelContent.AddContent(scenarioObject, location);

            _levelContent.AddContent(player, location);
        }

        /// <summary>
        /// Unloads data and returns extracted content to be moved with the Player
        /// </summary>
        public IEnumerable<ScenarioObject> Unload()
        {
            return _levelContent.Unload();
        }

        public bool HasContent(string scenarioObjectId)
        {
            return _levelContent.Contains(scenarioObjectId);
        }

        public ScenarioObject GetContent(string scenarioObjectId)
        {
            return _levelContent.Get(scenarioObjectId);
        }

        /// <summary>
        /// Adds content at it's last saved location
        /// </summary>
        public bool AddContent(ScenarioObject scenarioObject, GridLocation location)
        {
            if (_grid.WalkableMap[location] == null)
                throw new Exception("Trying to add content to a non-walkable location");

            _levelContent.AddContent(scenarioObject, location);

            return true;
        }

        /// <summary>
        /// Adds content at a random non-occupied location in the level
        /// </summary>
        public bool AddContentRandom(IRandomSequenceGenerator randomSequenceGenerator, 
                                     ScenarioObject scenarioObject, 
                                     ContentRandomPlacementType contentPlacementType,
                                     IEnumerable<GridLocation> excludedLocations)
        {
            GridLocation randomLocation = null;

            switch (contentPlacementType)
            {
                case ContentRandomPlacementType.Random:
                    randomLocation = _grid.GetNonOccupiedLocation(LayoutGrid.LayoutLayer.Placement, randomSequenceGenerator, excludedLocations);
                    break;
                case ContentRandomPlacementType.RandomRegion:
                    randomLocation = _grid.GetNonOccupiedLocation(LayoutGrid.LayoutLayer.Room, randomSequenceGenerator, excludedLocations);
                    break;
                case ContentRandomPlacementType.RandomCorridor:
                    randomLocation = _grid.GetNonOccupiedLocation(LayoutGrid.LayoutLayer.Corridor, randomSequenceGenerator, excludedLocations);
                    break;
                default:
                    throw new Exception("Unhandled ContentRandomPlacementType Level.cs");
            }

            if (randomLocation == null)
                return false;

            // Add content to the level
            _levelContent.AddContent(scenarioObject, randomLocation);

            return true;
        }

        /// <summary>
        /// Adds content beneath the specified character
        /// </summary>
        public bool AddContentBeneath(CharacterBase character, ScenarioObject content)
        {
            var location = _levelContent[character.Id];

            // Add content to the level
            _levelContent.AddContent(content, location);

            return true;
        }

        /// <summary>
        /// Adds content to a random, adjacent, un-occupied location
        /// </summary>
        public bool AddContentAdjacent(IRandomSequenceGenerator randomSequenceGenerator, CharacterBase character, ScenarioObject content)
        {
            var location = _levelContent[character.Id];

            // Gets adjacent locations to the character
            var adjacentLocations = _grid.GetAdjacentLocations(location);

            // Filters out occupied locations
            var nonOccupiedLocations = adjacentLocations.Where(location => _grid.LayerContains(LayoutGrid.LayoutLayer.Placement, location) &&
                                                                          !_grid.IsOccupied(location));
            // Finally, select one of these at random
            var randomLocation = randomSequenceGenerator.GetRandomElement(nonOccupiedLocations);

            if (randomLocation == null)
                return false;

            // Add content to the level
            _levelContent.AddContent(content, randomLocation);

            return true;
        }

        /// <summary>
        /// Adds content group according to the group placement type
        /// </summary>
        public bool AddContentGroup(IRandomSequenceGenerator randomSequenceGenerator, 
                                    IEnumerable<ScenarioObject> scenarioObjects, 
                                    ContentGroupPlacementType placementType,
                                    IEnumerable<GridLocation> excludedLocations)
        {
            // Have to be able to query the layout grid for NON-OCCUPIED cells. So, the occupation of cells needs to be 
            // dually maintained - once for the content grid; and once for the layout grid.
            //
            // Then, the queries should be run on the non-occupied grid.

            // Take the ceiling of the sqrt; and square it to get an enclosing square.
            var squareEdge = (int)System.Math.Pow(System.Math.Ceiling(System.Math.Sqrt(scenarioObjects.Count())), 2);

            IEnumerable<GridLocation> locations = null;

            switch (placementType)
            {
                case ContentGroupPlacementType.Adjacent:
                    {
                        // Get a contiguous, random rectangle of non-occupied locations
                        var squareRegion = _grid.GetNonOccupiedRegionLocationGroup(LayoutGrid.LayoutLayer.Placement, squareEdge, squareEdge, 
                                                                                  randomSequenceGenerator, 
                                                                                  excludedLocations);

                        if (squareRegion != null)
                            locations = squareRegion.Locations;

                        else
                            return false;
                    }
                    break;
                case ContentGroupPlacementType.RandomlyDistant:
                    {
                        // Start with location near the edge of the walkable map
                        var location = _grid.GetNonOccupiedEdgeLocation(LayoutGrid.LayoutLayer.Placement, randomSequenceGenerator, excludedLocations);

                        var distantLocations = new List<GridLocation>() { location };

                        for (int i = 1; i < scenarioObjects.Count(); i++)
                        {
                            // Calculate next location based on the previous
                            location = _grid.GetNonOccupiedDistantLocations(LayoutGrid.LayoutLayer.Placement, distantLocations, randomSequenceGenerator, excludedLocations);

                            if (location != null)
                                distantLocations.Add(location);

                            // Calculation failed - just return false
                            else
                                return false;
                        }

                        locations = distantLocations;
                    }
                    break;
                case ContentGroupPlacementType.RandomlyDistantRoom:
                    {
                        // Start with location near the edge of the room map
                        var location = _grid.GetNonOccupiedEdgeLocation(LayoutGrid.LayoutLayer.Room, randomSequenceGenerator, excludedLocations);

                        var distantLocations = new List<GridLocation>() { location };

                        for (int i = 1; i < scenarioObjects.Count(); i++)
                        {
                            // Calculate next location based on the previous
                            location = _grid.GetNonOccupiedDistantLocations(LayoutGrid.LayoutLayer.Room, distantLocations, randomSequenceGenerator, excludedLocations);

                            if (location != null)
                                distantLocations.Add(location);

                            // Calculation failed - just return false
                            else
                                return false;
                        }

                        locations = distantLocations;
                    }
                    break;
                default:
                    throw new Exception("Unhandled ContentGroupPlacementType Level.cs");
            }

            if (!locations.Any())
                return false;

            // Add contents to level
            for (int i = 0; i < locations.Count(); i++)
            {
                var location = locations.ElementAt(i);
                var scenarioObject = scenarioObjects.ElementAt(i);

                _levelContent.AddContent(scenarioObject, location);
            }

            return true;
        }

        public void RemoveContent(string scenarioObjectId)
        {
            _levelContent.RemoveContent(scenarioObjectId);
        }

        public void MoveContent(ScenarioObject scenarioObject, GridLocation newLocation)
        {
            // Process as a Remove -> Add
            RemoveContent(scenarioObject.Id);
            AddContent(scenarioObject, newLocation);
        }

        #region Content Queries
        public DoodadNormal GetStairsUp()
        {
            return _levelContent.DoodadsNormal.FirstOrDefault(doodad => doodad.NormalType == DoodadNormalType.StairsUp);
        }
        public DoodadNormal GetStairsDown()
        {
            return _levelContent.DoodadsNormal.FirstOrDefault(doodad => doodad.NormalType == DoodadNormalType.StairsDown);
        }
        public DoodadNormal GetSavePoint()
        {
            return _levelContent.DoodadsNormal.FirstOrDefault(doodad => doodad.NormalType == DoodadNormalType.SavePoint);
        }
        public bool HasStairsUp()
        {
            return _levelContent.DoodadsNormal.Any(doodad => doodad.NormalType == DoodadNormalType.StairsUp);
        }
        public bool HasStairsDown()
        {
            return _levelContent.DoodadsNormal.Any(doodad => doodad.NormalType == DoodadNormalType.StairsDown);
        }
        public bool HasSavePoint()
        {
            return _levelContent.DoodadsNormal.Any(doodad => doodad.NormalType == DoodadNormalType.SavePoint);
        }

        public IEnumerable<ScenarioObject> AllContent { get { return _levelContent.AllContent; } }
        public IEnumerable<Consumable> Consumables { get { return _levelContent.Consumables; } }
        public IEnumerable<DoodadMagic> Doodads { get { return _levelContent.Doodads; } }
        public IEnumerable<DoodadNormal> DoodadsNormal { get { return _levelContent.DoodadsNormal; } }
        public IEnumerable<Enemy> Enemies { get { return _levelContent.Enemies; } }
        public IEnumerable<Equipment> Equipment { get { return _levelContent.Equipment; } }
        public IEnumerable<Friendly> Friendlies { get { return _levelContent.Friendlies; } }
        public IEnumerable<NonPlayerCharacter> NonPlayerCharacters { get { return _levelContent.NonPlayerCharacters; } }
        public IEnumerable<TemporaryCharacter> TemporaryCharacters { get { return _levelContent.TemporaryCharacters; } }

        public GridLocation GetLocation(ScenarioObject scenarioObject)
        {
            return _levelContent[scenarioObject.Id];
        }

        /// <summary>
        /// Returns the First-Or-Default object of type T located at the cellPoint
        /// </summary>
        public T GetAt<T>(GridLocation location) where T : ScenarioObject
        {
            return _levelContent[location].Where(scenarioObject => scenarioObject is T)
                                          .Cast<T>()
                                          .FirstOrDefault();
        }

        public IEnumerable<T> GetManyAt<T>(GridLocation location) where T : ScenarioObject
        {
            return _levelContent[location].Where(scenarioObject => scenarioObject is T)
                                          .Cast<T>()
                                          .Actualize();
        }

        public IEnumerable<T> GetManyAt<T>(IEnumerable<GridLocation> locations) where T : ScenarioObject
        {
            return locations.SelectMany(location => GetManyAt<T>(location))
                            .Actualize();
        }

        /// <summary>
        /// Checks level contents to see if cell is occupied. (NOTE** Includes the player location)
        /// </summary>
        public bool IsCellOccupied(GridLocation location)
        {
            return _grid.IsOccupied(location);
        }

        /// <summary>
        /// Checks level contents to see if cell is occupied by a character. (NOTE*** Includes the player location)
        /// </summary>
        public bool IsCellOccupiedByCharacter(GridLocation location)
        {
            return _levelContent[location].Any(scenarioObject => scenarioObject is CharacterBase);
        }
        #endregion
    }
}

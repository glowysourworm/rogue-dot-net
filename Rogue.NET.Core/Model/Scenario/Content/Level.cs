using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Serialization.Component.Interface;
using Rogue.NET.Common.Serialization.Interface;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content.Doodad;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.Scenario.Dynamic.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration.Design;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using Rogue.NET.Core.Processing.Model.Generator.Interface;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

using CharacterBase = Rogue.NET.Core.Model.Scenario.Character.CharacterBase;

namespace Rogue.NET.Core.Model.Scenario.Content
{
    [Serializable]
    public class Level : IRecursiveSerializable
    {
        /// <summary>
        /// Primary layout storage component for the level
        /// </summary>
        public LayoutGrid Grid { get; private set; }

        /// <summary>
        /// Primary content storage / query component for the level
        /// </summary>
        public ContentGrid Content { get; private set; }

        /// <summary>
        /// Player-memorized content for the level (SERIALIZED)
        /// </summary>
        public ContentGrid MemorizedContent { get; private set; }

        /// <summary>
        /// Calculated visibility and path finding data for the level (NON-SERIALIZED) (CONSIDER SERIALIZING TO
        /// PRESERVE CHARACTER BEHAVIOR STATE)
        /// </summary>
        public CharacterMovement Movement { get; private set; }

        /// <summary>
        /// Calculated aura data for the level (NON-SERIALIZED)
        /// </summary>
        public AuraGrid AuraGrid { get; private set; }

        /// <summary>
        /// Parameters left over from level generation
        /// </summary>
        public LevelParameters Parameters { get; protected set; }

        public Level(LayoutTemplate layout,
                     LevelBranchTemplate levelBranch,
                     LayoutGrid grid,
                     int number)
        {
            this.Grid = grid;
            this.Content = new ContentGrid(grid);
            this.MemorizedContent = new ContentGrid(grid);
            this.Movement = new CharacterMovement(grid, this.Content);
            this.AuraGrid = new AuraGrid(this.Movement);

            this.Parameters = new LevelParameters()
            {
                Number = number,
                LevelBranchName = levelBranch.Name,
                LayoutName = layout.Name,
                EnemyGenerationPerStep = levelBranch.MonsterGenerationPerStep
            };
        }

        public Level(IPropertyReader reader)
        {
            this.Grid = reader.Read<LayoutGrid>("Grid");
            this.Parameters = reader.Read<LevelParameters>("Parameters");

            var count = reader.Read<int>("ContentCount");
            var homeLocationCount = reader.Read<int>("HomeLocationCount");
            var memorizedCount = reader.Read<int>("MemorizedContentCount");

            // Instantiate the level content and memorized content
            this.Content = new ContentGrid(this.Grid);
            this.MemorizedContent = new ContentGrid(this.Grid);
            this.Movement = new CharacterMovement(this.Grid, this.Content);

            // Deserialize the content
            for (int i = 0; i < count; i++)
            {
                var scenarioObject = reader.Read<ScenarioObject>("Content" + i.ToString());
                var location = reader.Read<GridLocation>("Location" + i.ToString());

                // Add the content to the container
                this.Content.AddContent(scenarioObject, location);

                // Grid does NOT SERIALIZE OCCUPIED DATA. THIS IS SET HERE MANUALLY.
                this.Grid.SetOccupied(location, true);
            }

            // Deserialize the home locations
            for (int i = 0; i < homeLocationCount; i++)
            {
                // Store this character's Id along with its home location
                var contentId = reader.Read<string>("HomeLocationCharacterId" + i.ToString());
                var location = reader.Read<GridLocation>("HomeLocation" + i.ToString());
                var character = this.Content.Get(contentId) as NonPlayerCharacter;

                // OVERWRITE THE HOME LOCATION SET BY AddContent(...)
                this.Content.SetHomeLocation(character, location);
            }

            // Deserialize the memorized content
            for (int i = 0; i < memorizedCount; i++)
            {
                var scenarioObject = reader.Read<ScenarioObject>("MemorizedContent" + i.ToString());
                var location = reader.Read<GridLocation>("MemorizedLocation" + i.ToString());

                // MATCH UP ANY REFERENCES STILL HELD BY THE LEVEL CONTENT GRID
                if (this.Content.Contains(scenarioObject.Id))
                    scenarioObject = this.Content.Get(scenarioObject.Id);

                // Add the content to the container
                this.MemorizedContent.AddContent(scenarioObject, location);
            }
        }

        public void GetProperties(IPropertyWriter writer)
        {
            writer.Write("Grid", this.Grid);
            writer.Write("Parameters", this.Parameters);

            // Serialize the number of content entries
            writer.Write("ContentCount", this.Content.AllContent.Where(content => !(content is Player)).Count());
            writer.Write("HomeLocationCount", this.Content.NonPlayerCharacters.Count());
            writer.Write("MemorizedContentCount", this.MemorizedContent.AllContent.Count());

            var counter = 0;

            // Serialize the content
            foreach (var content in this.Content.AllContent)
            {
                // *** SERIALIZE EVERYTHING EXCEPT THE PLAYER
                if (content is Player)
                    continue;

                // Store the object and its location
                writer.Write("Content" + counter, content);
                writer.Write("Location" + counter++, this.Content[content.Id]);
            }

            counter = 0;

            // Serialize the home locations of non-player characters
            foreach (var content in this.Content.NonPlayerCharacters)
            {
                // Store this character's Id along with its home location
                writer.Write("HomeLocationCharacterId" + counter, content.Id);
                writer.Write("HomeLocation" + counter++, this.Content.GetHomeLocation(content));
            }

            counter = 0;

            // Serialize memorized content - THIS WILL BE DUPLICATED DATA. KEPT THIS WAY BECAUSE
            //                               LEVEL CONTENTS THAT ARE PICKED UP WILL BE LOST REFERENCES.
            //                               SO, THE REFERENCES ARE MATCHED DURING DESERIALIZATION FOR
            //                               ANY DUPLICATES STILL IN THE LEVEL CONTENT GRID.
            foreach (var content in this.MemorizedContent.AllContent)
            {
                // Store the object and its location
                writer.Write("MemorizedContent" + counter, content);
                writer.Write("MemorizedLocation" + counter++, this.MemorizedContent[content.Id]);
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
                this.Content.AddContent(scenarioObject, location);

            this.Content.AddContent(player, location);
        }

        /// <summary>
        /// Unloads data and returns extracted content to be moved with the Player
        /// </summary>
        public IEnumerable<ScenarioObject> Unload()
        {
            return this.Content.Unload();
        }

        /// <summary>
        /// Adds content at it's last saved location
        /// </summary>
        public bool AddContent(ScenarioObject scenarioObject, GridLocation location)
        {
            if (this.Grid.WalkableMap[location] == null)
                throw new Exception("Trying to add content to a non-walkable location");

            // Add content to the container
            this.Content.AddContent(scenarioObject, location);

            // Set this location as occupied
            this.Grid.SetOccupied(location, true);

            return true;
        }

        /// <summary>
        /// Adds content at a random non-occupied location in the level
        /// </summary>
        public bool AddContentRandom(IRandomSequenceGenerator randomSequenceGenerator,
                                     ScenarioObject scenarioObject,
                                     ContentRandomPlacementType contentPlacementType,
                                     IEnumerable<GridLocation> excludedLocations = null)
        {
            GridLocation randomLocation = null;

            switch (contentPlacementType)
            {
                case ContentRandomPlacementType.Random:
                    randomLocation = this.Grid.GetNonOccupiedLocation(LayoutGrid.LayoutLayer.Placement, randomSequenceGenerator, excludedLocations);
                    break;
                case ContentRandomPlacementType.RandomRegion:
                    randomLocation = this.Grid.GetNonOccupiedLocation(LayoutGrid.LayoutLayer.ConnectionRoom, randomSequenceGenerator, excludedLocations);
                    break;
                case ContentRandomPlacementType.RandomCorridor:
                    randomLocation = this.Grid.GetNonOccupiedLocation(LayoutGrid.LayoutLayer.Corridor, randomSequenceGenerator, excludedLocations);
                    break;
                default:
                    throw new Exception("Unhandled ContentRandomPlacementType Level.cs");
            }

            if (randomLocation == null)
                return false;

            // Add content to the level -> Set Occupied Layout Grid
            AddContent(scenarioObject, randomLocation);

            return true;
        }

        /// <summary>
        /// Adds content beneath the specified character
        /// </summary>
        public bool AddContentBeneath(CharacterBase character, ScenarioObject content)
        {
            var location = this.Content[character.Id];

            // Add content to the level -> Set Occupied Layout Grid
            AddContent(content, location);

            return true;
        }

        /// <summary>
        /// Adds content to a random, adjacent, un-occupied, walkable location
        /// </summary>
        public bool AddContentAdjacent(IRandomSequenceGenerator randomSequenceGenerator, CharacterBase character, ScenarioObject content)
        {
            var location = this.Content[character];

            // Gets adjacent locations to the character
            var adjacentLocations = this.Grid.GetAdjacentLocations(location);

            // Filters out occupied locations
            var nonOccupiedLocations = adjacentLocations.Where(location => this.Grid.LayerContains(LayoutGrid.LayoutLayer.Walkable, location) &&
                                                                          !this.Grid.IsOccupied(location));
            // Finally, select one of these at random
            var randomLocation = randomSequenceGenerator.GetRandomElement(nonOccupiedLocations);

            if (randomLocation == null)
                return false;

            // Add content to the level -> Set Occupied Layout Grid
            AddContent(content, randomLocation);

            return true;
        }

        /// <summary>
        /// Adds content group according to the group placement type
        /// </summary>
        public bool AddContentGroup(IRandomSequenceGenerator randomSequenceGenerator,
                                    IEnumerable<ScenarioObject> scenarioObjects,
                                    ContentGroupPlacementType placementType,
                                    IEnumerable<GridLocation> excludedLocations = null)
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
                        var squareRegion = this.Grid.GetNonOccupiedRegionLocationGroup(LayoutGrid.LayoutLayer.Placement, squareEdge, squareEdge,
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
                        var location = this.Grid.GetNonOccupiedEdgeLocation(LayoutGrid.LayoutLayer.Placement, randomSequenceGenerator, excludedLocations);

                        var distantLocations = new List<GridLocation>() { location };

                        for (int i = 1; i < scenarioObjects.Count(); i++)
                        {
                            // Calculate next location based on the previous
                            location = this.Grid.GetNonOccupiedDistantLocations(LayoutGrid.LayoutLayer.Placement, distantLocations, randomSequenceGenerator, excludedLocations);

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
                        var location = this.Grid.GetNonOccupiedEdgeLocation(LayoutGrid.LayoutLayer.ConnectionRoom, randomSequenceGenerator, excludedLocations);

                        var distantLocations = new List<GridLocation>() { location };

                        for (int i = 1; i < scenarioObjects.Count(); i++)
                        {
                            // Calculate next location based on the previous
                            location = this.Grid.GetNonOccupiedDistantLocations(LayoutGrid.LayoutLayer.ConnectionRoom, distantLocations, randomSequenceGenerator, excludedLocations);

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

                // Add content to the level -> Set Occupied Layout Grid
                AddContent(scenarioObject, location);
            }

            return true;
        }

        public void RemoveContent(string scenarioObjectId)
        {
            // Get location of the object
            var location = this.Content[scenarioObjectId];

            // Remove content from the container
            this.Content.RemoveContent(scenarioObjectId);

            // Set non-occupied in the layout grid
            this.Grid.SetOccupied(location, false);
        }

        public void MoveContent(ScenarioObject scenarioObject, GridLocation newLocation)
        {
            // Process as a Remove -> Add
            RemoveContent(scenarioObject.Id);
            AddContent(scenarioObject, newLocation);
        }

        /// <summary>
        /// Update "memorized" content collection. This will erase anything previously at the
        /// provided location.
        /// </summary>
        public bool UpdateMemorizedContent(IEnumerable<GridLocation> visibleLocations)
        {
            foreach (var location in visibleLocations)
            {
                // Remove any existing content from visible location
                var memorizedContentIds = this.MemorizedContent[location].Select(scenarioObject => scenarioObject.Id)
                                                                     .Actualize();

                // (Modifies collection)
                foreach (var contentId in memorizedContentIds)
                    this.MemorizedContent.RemoveContent(contentId);

                // Query any contents from the level content grid
                var levelContent = this.Content[location].Where(scenarioObject => scenarioObject is ItemBase ||
                                                                                  scenarioObject is DoodadBase).Actualize();

                // Add these to the memorized content
                foreach (var content in levelContent)
                    this.MemorizedContent.AddContent(content, location);
            }

            return true;
        }

        #region Content Queries
        public Player GetPlayer()
        {
            return this.Content
                       .Characters
                       .OfType<Player>()
                       .First();
        }
        public DoodadNormal GetStairsUp()
        {
            return this.Content.DoodadsNormal.FirstOrDefault(doodad => doodad.NormalType == DoodadNormalType.StairsUp);
        }
        public DoodadNormal GetStairsDown()
        {
            return this.Content.DoodadsNormal.FirstOrDefault(doodad => doodad.NormalType == DoodadNormalType.StairsDown);
        }
        public DoodadNormal GetSavePoint()
        {
            return this.Content.DoodadsNormal.FirstOrDefault(doodad => doodad.NormalType == DoodadNormalType.SavePoint);
        }
        public bool HasStairsUp()
        {
            return this.Content.DoodadsNormal.Any(doodad => doodad.NormalType == DoodadNormalType.StairsUp);
        }
        public bool HasStairsDown()
        {
            return this.Content.DoodadsNormal.Any(doodad => doodad.NormalType == DoodadNormalType.StairsDown);
        }
        public bool HasSavePoint()
        {
            return this.Content.DoodadsNormal.Any(doodad => doodad.NormalType == DoodadNormalType.SavePoint);
        }

        public bool IsOccupied(GridLocation location)
        {
            return this.Grid.IsOccupied(location);
        }
        public bool IsOccupiedByCharacter(GridLocation location)
        {
            return this.Content[location].Any(scenarioObject => scenarioObject is CharacterBase);
        }

        public GridLocation GetPointInDirection(GridLocation location, Compass direction)
        {
            switch (direction)
            {
                case Compass.N: return this.Grid[location.Column, location.Row - 1]?.Location ?? null;
                case Compass.S: return this.Grid[location.Column, location.Row + 1]?.Location ?? null;
                case Compass.E: return this.Grid[location.Column + 1, location.Row]?.Location ?? null;
                case Compass.W: return this.Grid[location.Column - 1, location.Row]?.Location ?? null;
                case Compass.NE: return this.Grid[location.Column + 1, location.Row - 1]?.Location ?? null;
                case Compass.NW: return this.Grid[location.Column - 1, location.Row - 1]?.Location ?? null;
                case Compass.SW: return this.Grid[location.Column - 1, location.Row + 1]?.Location ?? null;
                case Compass.SE: return this.Grid[location.Column + 1, location.Row + 1]?.Location ?? null;
                case Compass.Null:
                default:
                    return location;
            }
        }

        public GridLocation GetFreeAdjacentLocation(CharacterBase character, IRandomSequenceGenerator randomSequenceGenerator)
        {
            // Get non-occupied locations near the character with range 1
            var adjacentLocations = this.Grid.GetNonOccupiedLocationsNear(LayoutGrid.LayoutLayer.Walkable, this.Content[character], 1);

            // Return random element from these locations
            return randomSequenceGenerator.GetRandomElement(adjacentLocations);
        }

        public GridLocation GetFreeAdjacentMovementLocation(CharacterBase character, IRandomSequenceGenerator randomSequenceGenerator)
        {
            // Get adjacent locations
            var freeLocations = this.Grid.GetNonOccupiedLocationsNear(LayoutGrid.LayoutLayer.Walkable, this.Content[character], 1);

            var alignmentType = character is Player ? CharacterAlignmentType.PlayerAligned : CharacterAlignmentType.EnemyAligned;

            // Check for path blocking
            var openLocations = freeLocations.Where(location => this.Movement
                                                                    .IsPathToAdjacentLocationBlocked(this.Content[character], location, true, alignmentType))
                                             .Actualize();

            return randomSequenceGenerator.GetRandomElement(openLocations);
        }

        public IEnumerable<GridLocation> GetLocationsInRange(GridLocation location, int range)
        {
            // Query for locations near the character from the walkable layer
            return this.Grid.GetNonOccupiedLocationsNear(LayoutGrid.LayoutLayer.Walkable, location, range);
        }

        public IEnumerable<GridLocation> GetVisibleLocationsInRange(NonPlayerCharacter character, int range)
        {
            // Query for locations near the character from the walkable layer
            var locationsNear = this.Grid.GetNonOccupiedLocationsNear(LayoutGrid.LayoutLayer.Walkable, this.Content[character], range);

            // Cross reference this collection with the visibility grid
            return locationsNear.Where(location => this.Movement.IsVisibleTo(location, character))
                                .Actualize();
        }
        #endregion
    }
}

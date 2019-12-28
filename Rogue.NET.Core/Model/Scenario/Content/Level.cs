using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content.Doodad;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration.Design;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;

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
        readonly LevelContent _content;
        readonly LayoutGrid _grid;

        // NOTE*** Player NOT serialized
        Player _player;

        /// <summary>
        /// The primary layout grid for the level
        /// </summary>
        public LayoutGrid Grid
        {
            get { return _grid; }
        }

        public LevelParameters Parameters { get; protected set; }

        public Level(LayoutTemplate layout,
                     LevelBranchTemplate levelBranch,
                     LayoutGrid grid,
                     int number)
        {
            _grid = grid;
            _content = new LevelContent(grid);

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
            _content = (LevelContent)info.GetValue("Content", typeof(LevelContent));
            this.Parameters = (LevelParameters)info.GetValue("Parameters", typeof(LevelParameters));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Grid", _grid);
            info.AddValue("Content", _content);
            info.AddValue("Parameters", this.Parameters);
        }

        /// <summary>
        /// Loads level with player reference and any other extracted contents from the previous level
        /// </summary>
        public void Load(Player player, IEnumerable<ScenarioObject> extractedContent)
        {
            _player = player;

            // Add extrated content (from previous level) to this level. Example: Friendly characters
            foreach (var scenarioObject in extractedContent)
                _content.AddContent(scenarioObject);
        }

        /// <summary>
        /// Unloads data and returns extracted content to be moved with the Player
        /// </summary>
        public IEnumerable<ScenarioObject> Unload()
        {
            return _content.Unload();
        }

        public bool HasContent(string scenarioObjectId)
        {
            return _content.Contains(scenarioObjectId);
        }

        public ScenarioObject GetContent(string scenarioObjectId)
        {
            return _content[scenarioObjectId];
        }

        public void AddContent(ScenarioObject scenarioObject)
        {
            _content.AddContent(scenarioObject);
        }

        public void RemoveContent(string scenarioObjectId)
        {
            _content.RemoveContent(scenarioObjectId);
        }

        #region Content Queries
        public DoodadNormal GetStairsUp()
        {
            return _content.DoodadsNormal.FirstOrDefault(doodad => doodad.NormalType == DoodadNormalType.StairsUp);
        }
        public DoodadNormal GetStairsDown()
        {
            return _content.DoodadsNormal.FirstOrDefault(doodad => doodad.NormalType == DoodadNormalType.StairsDown);
        }
        public DoodadNormal GetSavePoint()
        {
            return _content.DoodadsNormal.FirstOrDefault(doodad => doodad.NormalType == DoodadNormalType.SavePoint);
        }
        public bool HasStairsUp()
        {
            return _content.DoodadsNormal.Any(doodad => doodad.NormalType == DoodadNormalType.StairsUp);
        }
        public bool HasStairsDown()
        {
            return _content.DoodadsNormal.Any(doodad => doodad.NormalType == DoodadNormalType.StairsDown);
        }
        public bool HasSavePoint()
        {
            return _content.DoodadsNormal.Any(doodad => doodad.NormalType == DoodadNormalType.SavePoint);
        }

        public IEnumerable<ScenarioObject> AllContent { get { return _content.AllContent; } }
        public IEnumerable<Consumable> Consumables { get { return _content.Consumables; } }
        public IEnumerable<DoodadMagic> Doodads { get { return _content.Doodads; } }
        public IEnumerable<DoodadNormal> DoodadsNormal { get { return _content.DoodadsNormal; } }
        public IEnumerable<Enemy> Enemies { get { return _content.Enemies; } }
        public IEnumerable<Equipment> Equipment { get { return _content.Equipment; } }
        public IEnumerable<Friendly> Friendlies { get { return _content.Friendlies; } }
        public IEnumerable<NonPlayerCharacter> NonPlayerCharacters { get { return _content.NonPlayerCharacters; } }
        public IEnumerable<TemporaryCharacter> TemporaryCharacters { get { return _content.TemporaryCharacters; } }

        /// <summary>
        /// Returns the First-Or-Default object of type T located at the cellPoint
        /// </summary>
        public T GetAt<T>(GridLocation location) where T : ScenarioObject
        {
            return _content[location.Column, location.Row].Where(scenarioObject => scenarioObject is T)
                                                          .Cast<T>()
                                                          .FirstOrDefault();
        }

        public IEnumerable<T> GetManyAt<T>(GridLocation location) where T : ScenarioObject
        {
            return _content[location.Column, location.Row].Where(scenarioObject => scenarioObject is T)
                                                          .Cast<T>()
                                                          .Actualize();
        }

        /// <summary>
        /// Checks level contents to see if cell is occupied. (NOTE** Includes the player location)
        /// </summary>
        public bool IsCellOccupied(GridLocation location)
        {
            return location.Equals(_player.Location) || _content[location.Column, location.Row].Any();
        }

        /// <summary>
        /// Checks level contents to see if cell is occupied by a character. (NOTE*** Includes the player location)
        /// </summary>
        public bool IsCellOccupiedByCharacter(GridLocation location)
        {
            return location.Equals(_player.Location) || _content[location.Column, location.Row].Any(scenarioObject => scenarioObject is CharacterBase);
        }
        #endregion
    }
}

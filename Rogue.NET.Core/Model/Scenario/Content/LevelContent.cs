using Rogue.NET.Core.Model.Event;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Content.Doodad;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;
using Rogue.NET.Core.Processing.Model.Extension;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Rogue.NET.Core.Model.Scenario
{
    public class LevelContent
    {
        // Gives the contents by location[column, row]
        IList<ScenarioObject>[,] _levelContentGrid;

        // Gives the contents as a dictionary
        Dictionary<string, ScenarioObject> _levelContentDict;
        Dictionary<string, GridLocation> _levelLocationDict;

        // TEMPORARY PLAYER REFERENCE - DOES NOT GET SERIALIZED
        Player _player;

        IList<NonPlayerCharacter> _nonPlayerCharacters;
        IList<Enemy> _enemies;
        IList<Friendly> _friendlies;
        IList<TemporaryCharacter> _temporaryCharacters;
        IList<Equipment> _equipment;
        IList<Consumable> _consumables;
        IList<DoodadMagic> _doodadMagics;
        IList<DoodadNormal> _doodadNormals;

        // TEMPORARY PLAYER REFERENCE - DOES NOT GET SERIALIZED
        public Player Player { get { return _player; } }
        public IEnumerable<ScenarioObject> AllContent
        {
            get { return _levelContentDict.Values; }
        }
        public IEnumerable<NonPlayerCharacter> NonPlayerCharacters
        {
            get { return _nonPlayerCharacters; }
            protected set { _nonPlayerCharacters = new List<NonPlayerCharacter>(value); }
        }
        public IEnumerable<Enemy> Enemies
        {
            get { return _enemies; }
            protected set { _enemies = new List<Enemy>(value); }
        }
        public IEnumerable<Friendly> Friendlies
        {
            get { return _friendlies; }
            protected set { _friendlies = new List<Friendly>(value); }
        }
        public IEnumerable<TemporaryCharacter> TemporaryCharacters
        {
            get { return _temporaryCharacters; }
            protected set { _temporaryCharacters = new List<TemporaryCharacter>(value); }
        }
        public IEnumerable<Equipment> Equipment
        {
            get { return _equipment; }
            protected set { _equipment = new List<Equipment>(value); }
        }
        public IEnumerable<Consumable> Consumables
        {
            get { return _consumables; }
            protected set { _consumables = new List<Consumable>(value); }
        }
        public IEnumerable<DoodadMagic> Doodads
        {
            get { return _doodadMagics; }
            protected set { _doodadMagics = new List<DoodadMagic>(value); }
        }
        public IEnumerable<DoodadNormal> DoodadsNormal
        {
            get { return _doodadNormals; }
            protected set { _doodadNormals = new List<DoodadNormal>(value); }
        }

        #region (public) Getters / Indexers

        /// <summary>
        /// Indexer for all content at the specified location
        /// </summary>
        public IEnumerable<ScenarioObject> this[IGridLocator location]
        {
            get { return _levelContentGrid[location.Column, location.Row]; }
        }

        /// <summary>
        /// Returns the location of the specified object id
        /// </summary>
        public GridLocation this[string scenarioObjectId]
        {
            get { return _levelLocationDict[scenarioObjectId]; }
        }

        /// <summary>
        /// Returns the object for the specified object id
        /// </summary>
        public ScenarioObject Get(string scenarioObjectId)
        {
            return _levelContentDict[scenarioObjectId];
        }

        #endregion

        public LevelContent(LayoutGrid grid)
        {
            Initialize(grid.Bounds.Width, 
                       grid.Bounds.Height, 
                       new Dictionary<string, ScenarioObject>(), 
                       new Dictionary<string, GridLocation>());
        }

        private void Initialize(int width, int height, Dictionary<string, ScenarioObject> contentDict, Dictionary<string, GridLocation> locationDict)
        {
            _levelContentGrid = new List<ScenarioObject>[width, height];
            _levelContentDict = new Dictionary<string, ScenarioObject>();
            _levelLocationDict = new Dictionary<string, GridLocation>();

            this.NonPlayerCharacters = new List<NonPlayerCharacter>();
            this.Enemies = new List<Enemy>();
            this.Friendlies = new List<Friendly>();
            this.TemporaryCharacters = new List<TemporaryCharacter>();
            this.Doodads = new List<DoodadMagic>();
            this.Equipment = new List<Equipment>();
            this.DoodadsNormal = new List<DoodadNormal>();
            this.Consumables = new List<Consumable>();

            _levelContentGrid.Iterate((column, row) =>
            {
                _levelContentGrid[column, row] = new List<ScenarioObject>();
            });

            // Add level contents from the dictionary
            foreach (var element in contentDict)
            {
                var scenarioObject = contentDict[element.Key];
                var location = locationDict[element.Key];

                AddContent(scenarioObject, location);
            }
        }

        public bool Contains(string scenarioObjectId)
        {
            return _levelContentDict.ContainsKey(scenarioObjectId);
        }

        public void AddContent(ScenarioObject scenarioObject, GridLocation location)
        {
            if (_levelContentDict.ContainsKey(scenarioObject.Id))
                throw new Exception("Trying to add duplicate Scenario Object to Level");

            if (scenarioObject is Player && _player != null)
                throw new Exception("Trying to add Player reference twice LevelContent.cs");

            if (scenarioObject is Player)
                _player = scenarioObject as Player;

            else if (scenarioObject is NonPlayerCharacter)
                _nonPlayerCharacters.Add(scenarioObject as NonPlayerCharacter);

            else if (scenarioObject is Enemy)
                _enemies.Add(scenarioObject as Enemy);

            else if (scenarioObject is Friendly)
                _friendlies.Add(scenarioObject as Friendly);

            else if (scenarioObject is TemporaryCharacter)
                _temporaryCharacters.Add(scenarioObject as TemporaryCharacter);

            else if (scenarioObject is Consumable)
                _consumables.Add(scenarioObject as Consumable);

            else if (scenarioObject is Equipment)
                _equipment.Add(scenarioObject as Equipment);

            else if (scenarioObject is DoodadMagic)
                _doodadMagics.Add(scenarioObject as DoodadMagic);

            else if (scenarioObject is DoodadNormal)
                _doodadNormals.Add(scenarioObject as DoodadNormal);

            else
                throw new Exception("Trying to add unknown type to Level");

            // Maintain collections
            _levelContentDict.Add(scenarioObject.Id, scenarioObject);
            _levelLocationDict.Add(scenarioObject.Id, location);

            // Maintain 2D array
            _levelContentGrid[location.Column, location.Row].Add(scenarioObject);
        }

        public void RemoveContent(string scenarioObjectId)
        {
            if (!_levelContentDict.ContainsKey(scenarioObjectId))
                throw new Exception("Trying to remove non-existent Scenario Object from Level");

            var scenarioObject = _levelContentDict[scenarioObjectId];
            var location = _levelLocationDict[scenarioObjectId];

            if (scenarioObject is Player)
                _player = null;

            else if (scenarioObject is NonPlayerCharacter)
                _nonPlayerCharacters.Remove(scenarioObject as NonPlayerCharacter);

            else if (scenarioObject is Enemy)
                _enemies.Remove(scenarioObject as Enemy);

            else if (scenarioObject is Friendly)
                _friendlies.Remove(scenarioObject as Friendly);

            else if (scenarioObject is TemporaryCharacter)
                _temporaryCharacters.Remove(scenarioObject as TemporaryCharacter);

            else if (scenarioObject is Consumable)
                _consumables.Remove(scenarioObject as Consumable);

            else if (scenarioObject is Equipment)
                _equipment.Remove(scenarioObject as Equipment);

            else if (scenarioObject is DoodadMagic)
                _doodadMagics.Remove(scenarioObject as DoodadMagic);

            else if (scenarioObject is DoodadNormal)
                _doodadNormals.Remove(scenarioObject as DoodadNormal);

            else
                throw new Exception("Trying to remove unknown type from Level");            

            // Maintain private collections
            _levelContentDict.Remove(scenarioObject.Id);
            _levelLocationDict.Remove(scenarioObject.Id);
            _levelContentGrid[location.Column, location.Row].Remove(scenarioObject);
        }

        /// <summary>
        /// Unloads data and returns extracted content to be moved with the Player. The Player reference is removed from 
        /// this instance of the LevelContent.
        /// </summary>
        public IEnumerable<ScenarioObject> Unload()
        {
            // REMOVE PLAYER FROM LEVEL - NULLIFIES THE REFERENCE
            RemoveContent(_player.Id);

            // Remove Temporary Characters
            var temporaryCharacters = _temporaryCharacters.ToList();
            foreach (var character in temporaryCharacters)
                RemoveContent(character.Id);

            // Remove Friendlies in Player Party
            var friendlies = _friendlies.ToList();
            foreach (var friendly in friendlies)
                RemoveContent(friendly.Id);

            return friendlies;
        }
    }
}

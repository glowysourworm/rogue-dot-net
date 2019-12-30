using Rogue.NET.Core.Model.Event;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Content.Doodad;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Processing.Model.Extension;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Rogue.NET.Core.Model.Scenario
{
    [Serializable]
    public class LevelContent : ISerializable
    {
        // Event forwarding from scenario object location change
        public event EventHandler<LocationChangedEventArgs> ScenarioObjectLocationChanged;

        // Gives the contents by location[column, row]
        IList<ScenarioObject>[,] _levelContentGrid;

        // Gives the contents as a dictionary
        IDictionary<string, ScenarioObject> _levelContentDict;

        IList<NonPlayerCharacter> _nonPlayerCharacters;
        IList<Enemy> _enemies;
        IList<Friendly> _friendlies;
        IList<TemporaryCharacter> _temporaryCharacters;
        IList<Equipment> _equipment;
        IList<Consumable> _consumables;
        IList<DoodadMagic> _doodadMagics;
        IList<DoodadNormal> _doodadNormals;

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

        /// <summary>
        /// Indexer for all content at the specified location
        /// </summary>
        public IEnumerable<ScenarioObject> this[int column, int row]
        {
            get { return _levelContentGrid[column, row]; }
        }

        /// <summary>
        /// Indexer for the content object with the specified id
        /// </summary>
        public ScenarioObject this[string scenarioObjectId]
        {
            get { return _levelContentDict[scenarioObjectId]; }
        }

        public LevelContent(LayoutGrid grid)
        {
            Initialize(grid.Bounds.Width, grid.Bounds.Height, new Dictionary<string, ScenarioObject>());
        }

        public LevelContent(SerializationInfo info, StreamingContext context)
        {
            var width = info.GetInt32("Width");
            var height = info.GetInt32("Height");

            var contentDict = (Dictionary<string, ScenarioObject>)info.GetValue("Content", typeof(Dictionary<string, ScenarioObject>));

            Initialize(width, height, contentDict);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Width", _levelContentGrid.GetLength(0));
            info.AddValue("Height", _levelContentGrid.GetLength(1));
            info.AddValue("Content", _levelContentDict);
        }

        private void Initialize(int width, int height, Dictionary<string, ScenarioObject> contentDict)
        {
            _levelContentGrid = new List<ScenarioObject>[width, height];
            _levelContentDict = new Dictionary<string, ScenarioObject>();

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
            foreach (var scenarioObject in contentDict.Values)
                AddContent(scenarioObject);
        }

        public bool Contains(string scenarioObjectId)
        {
            return _levelContentDict.ContainsKey(scenarioObjectId);
        }

        public void AddContent(ScenarioObject scenarioObject)
        {
            if (_levelContentDict.ContainsKey(scenarioObject.Id))
                throw new Exception("Trying to add duplicate Scenario Object to Level");

            if (scenarioObject is NonPlayerCharacter)
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

            // If object has an empty location it will be changed later on - which has an event hook below
            if (scenarioObject.Location != null)
                _levelContentGrid[scenarioObject.Location.Column, scenarioObject.Location.Row].Add(scenarioObject);

            scenarioObject.LocationChangedEvent += OnScenarioObjectLocationChanged;
        }
        public void RemoveContent(string scenarioObjectId)
        {
            if (!_levelContentDict.ContainsKey(scenarioObjectId))
                throw new Exception("Trying to remove non-existent Scenario Object from Level");

            var scenarioObject = _levelContentDict[scenarioObjectId];

            if (scenarioObject is NonPlayerCharacter)
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

            // If the CellPoint is empty then the object is being removed before it's mapped (by the Generators). So,
            // this is safe to do. The CellPoint should never be set to Empty by any of the in-game components (Logic)
            if (scenarioObject.Location != null)
                _levelContentGrid[scenarioObject.Location.Column, scenarioObject.Location.Row].Remove(scenarioObject);

            scenarioObject.LocationChangedEvent -= OnScenarioObjectLocationChanged;
        }

        /// <summary>
        /// Unloads data and returns extracted content to be moved with the Player
        /// </summary>
        public IEnumerable<ScenarioObject> Unload()
        {
            foreach (var scenarioObject in _levelContentDict.Values)
            {
                scenarioObject.LocationChangedEvent -= OnScenarioObjectLocationChanged;
            }

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

        private void OnScenarioObjectLocationChanged(object sender, LocationChangedEventArgs e)
        {
            if (e.OldLocation != null)
                _levelContentGrid[e.OldLocation.Column, e.OldLocation.Row].Remove(e.ScenarioObject);

            if (e.NewLocation != null)
                _levelContentGrid[e.NewLocation.Column, e.NewLocation.Row].Add(e.ScenarioObject);

            if (this.ScenarioObjectLocationChanged != null)
                this.ScenarioObjectLocationChanged(this, e);
        }
    }
}

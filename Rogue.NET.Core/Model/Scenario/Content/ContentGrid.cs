using Rogue.NET.Common.Extension;
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

namespace Rogue.NET.Core.Model.Scenario
{
    public class ContentGrid
    {
        // Gives the contents by location[column, row]
        IList<ScenarioObject>[,] _levelContentGrid;

        // Gives the contents as a dictionary
        Dictionary<string, ScenarioObject> _levelContentDict;
        Dictionary<string, GridLocation> _levelLocationDict;

        IList<CharacterBase> _characters;
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
        public IEnumerable<CharacterBase> Characters
        {
            get { return _characters; }
            protected set { _characters = new List<CharacterBase>(value); }
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

        public IEnumerable<ScenarioObject> this[IGridLocator location]
        {
            get { return _levelContentGrid[location.Column, location.Row]; }
        }
        public IEnumerable<ScenarioObject> this[int column, int row]
        {
            get { return _levelContentGrid[column, row]; }
        }
        public GridLocation this[ScenarioObject scenarioObject]
        {
            get { return _levelLocationDict[scenarioObject.Id]; }
        }
        public GridLocation this[string scenarioObjectId]
        {
            get { return _levelLocationDict[scenarioObjectId]; }
        }
        public bool Contains(string scenarioObjectId)
        {
            return _levelContentDict.ContainsKey(scenarioObjectId);
        }
        public ScenarioObject Get(string scenarioObjectId)
        {
            return _levelContentDict[scenarioObjectId];
        }

        /// <summary>
        /// Returns the First-Or-Default object of type T located at the cellPoint
        /// </summary>
        public T GetAt<T>(GridLocation location) where T : ScenarioObject
        {
            return this[location].OfType<T>()
                                 .FirstOrDefault();
        }

        public IEnumerable<T> GetManyAt<T>(GridLocation location) where T : ScenarioObject
        {
            return this[location].OfType<T>()
                                 .Actualize();
        }

        public IEnumerable<T> GetManyAt<T>(IEnumerable<GridLocation> locations) where T : ScenarioObject
        {
            return locations.SelectMany(location => GetManyAt<T>(location))
                            .Actualize();
        }

        #endregion

        public ContentGrid(LayoutGrid grid)
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

            this.Characters = new List<CharacterBase>();
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

        public void AddContent(ScenarioObject scenarioObject, GridLocation location)
        {
            if (_levelContentDict.ContainsKey(scenarioObject.Id))
                throw new Exception("Trying to add duplicate Scenario Object to Level");

            // Maintain private collections
            if (scenarioObject is CharacterBase)
                _characters.Add(scenarioObject as CharacterBase);

            if (scenarioObject is NonPlayerCharacter)
                _nonPlayerCharacters.Add(scenarioObject as NonPlayerCharacter);

            if (scenarioObject is Enemy)
                _enemies.Add(scenarioObject as Enemy);

            if (scenarioObject is Friendly)
                _friendlies.Add(scenarioObject as Friendly);

            if (scenarioObject is TemporaryCharacter)
                _temporaryCharacters.Add(scenarioObject as TemporaryCharacter);

            if (scenarioObject is Consumable)
                _consumables.Add(scenarioObject as Consumable);

            if (scenarioObject is Equipment)
                _equipment.Add(scenarioObject as Equipment);

            if (scenarioObject is DoodadMagic)
                _doodadMagics.Add(scenarioObject as DoodadMagic);

            if (scenarioObject is DoodadNormal)
                _doodadNormals.Add(scenarioObject as DoodadNormal);

            _levelContentDict.Add(scenarioObject.Id, scenarioObject);
            _levelLocationDict.Add(scenarioObject.Id, location);
            _levelContentGrid[location.Column, location.Row].Add(scenarioObject);
        }

        public void RemoveContent(string scenarioObjectId)
        {
            if (!_levelContentDict.ContainsKey(scenarioObjectId))
                throw new Exception("Trying to remove non-existent Scenario Object from Level");

            var scenarioObject = _levelContentDict[scenarioObjectId];
            var location = _levelLocationDict[scenarioObjectId];

            // Maintain private collections
            if (scenarioObject is CharacterBase)
                _characters.Remove(scenarioObject as CharacterBase);

            if (scenarioObject is NonPlayerCharacter)
                _nonPlayerCharacters.Remove(scenarioObject as NonPlayerCharacter);

            if (scenarioObject is Enemy)
                _enemies.Remove(scenarioObject as Enemy);

            if (scenarioObject is Friendly)
                _friendlies.Remove(scenarioObject as Friendly);

            if (scenarioObject is TemporaryCharacter)
                _temporaryCharacters.Remove(scenarioObject as TemporaryCharacter);

            if (scenarioObject is Consumable)
                _consumables.Remove(scenarioObject as Consumable);

            if (scenarioObject is Equipment)
                _equipment.Remove(scenarioObject as Equipment);

            if (scenarioObject is DoodadMagic)
                _doodadMagics.Remove(scenarioObject as DoodadMagic);

            if (scenarioObject is DoodadNormal)
                _doodadNormals.Remove(scenarioObject as DoodadNormal);
            
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

using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Event;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Content.Doodad;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Core.Model.Scenario.Content.Layout;

using System;
using System.Linq;
using System.Collections.Generic;

using CharacterBase = Rogue.NET.Core.Model.Scenario.Character.Character;
using Rogue.NET.Common.Extension;

namespace Rogue.NET.Core.Model.Scenario
{
    [Serializable]
    public class Level
    {
        /// <summary>
        /// Name of the level branch that was used to generate this level
        /// </summary>
        public string LevelBranchName { get; protected set; }
        public LevelGrid Grid { get; protected set; }
        public LayoutType Type { get; protected set; }
        public LayoutConnectionType ConnectionType { get; protected set; }

        public int Number { get; protected set; }
        public bool HasStairsUp { get { return this.StairsUp != null; } }
        public bool HasStairsDown { get { return this.StairsDown != null; } }
        public bool HasSavePoint { get { return this.SavePoint != null; } }

        public DoodadNormal StairsUp { get; protected set; }
        public DoodadNormal StairsDown { get; protected set; }
        public DoodadNormal SavePoint { get; protected set; }

        public string WallColor { get; protected set; }
        public string DoorColor { get; protected set; }

        IList<NonPlayerCharacter> _nonPlayerCharacters;
        IList<Equipment> _equipment;
        IList<Consumable> _consumables;
        IList<DoodadMagic> _doodadMagics;
        IList<DoodadNormal> _doodadNormals;

        // Player is not part of the Level - these are maintained to provide fast access to 
        // all content and locations
        IList<ScenarioObject> _levelContent;

        // Gives the contents by location[column, row]
        IList<ScenarioObject>[,] _levelContentGrid;

        // Gives the contents as an array
        ScenarioObject[] _levelContentArray;

        // Gives the contents as a dictionary
        IDictionary<string, ScenarioObject> _levelContentDict;

        public IEnumerable<NonPlayerCharacter> NonPlayerCharacters
        {
            get { return _nonPlayerCharacters; }
            protected set { _nonPlayerCharacters = new List<NonPlayerCharacter>(value); }
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

        public Level(string levelBranchName,
                     LevelGrid grid,
                     LayoutType layoutType,
                     LayoutConnectionType layoutConnectionType,
                     int number,
                     string wallColor,
                     string doorColor)
        {
            Initialize(levelBranchName, 
                       grid, 
                       layoutType, 
                       layoutConnectionType, 
                       number, 
                       wallColor, 
                       doorColor);
        }

        private void Initialize(
                     string levelBranchName,
                     LevelGrid grid,
                     LayoutType layoutType,
                     LayoutConnectionType layoutConnectionType,
                     int number,
                     string wallColor,
                     string doorColor)
        {
            this.LevelBranchName = levelBranchName;

            this.Type = layoutType;
            this.ConnectionType = layoutConnectionType;
            this.Grid = grid;
            this.Number = number;
            this.WallColor = wallColor;
            this.DoorColor = doorColor;

            this.StairsDown = null;
            this.StairsUp = null;
            this.SavePoint = null;

            this.NonPlayerCharacters = new List<NonPlayerCharacter>();
            this.Doodads = new List<DoodadMagic>();
            this.Equipment = new List<Equipment>();
            this.DoodadsNormal = new List<DoodadNormal>();
            this.Consumables = new List<Consumable>();

            _levelContent = new List<ScenarioObject>();
            _levelContentGrid = new List<ScenarioObject>[grid.Bounds.CellWidth, grid.Bounds.CellHeight];
            _levelContentArray = new ScenarioObject[] { };
            _levelContentDict = new Dictionary<string, ScenarioObject>();

            RebuildContentGrid();
        }
        
        public void AddStairsDown(DoodadNormal stairsDown)
        {
            this.StairsDown = stairsDown;

            AddContent(stairsDown);
        }
        public void AddStairsUp(DoodadNormal stairsUp)
        {
            this.StairsUp = stairsUp;

            AddContent(stairsUp);
        }
        public void AddSavePoint(DoodadNormal savePoint)
        {
            this.SavePoint = savePoint;

            AddContent(savePoint);
        }
        public void AddContent(ScenarioObject scenarioObject)
        {
            if (_levelContent.Contains(scenarioObject))
                throw new Exception("Trying to add duplicate Scenario Object to Level");

            if (scenarioObject is NonPlayerCharacter)
                _nonPlayerCharacters.Add(scenarioObject as NonPlayerCharacter);

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
            _levelContent.Add(scenarioObject);

            // If object has an empty location it will be changed later on - which has an event hook below
            if (scenarioObject.Location != GridLocation.Empty)
                _levelContentGrid[scenarioObject.Location.Column, scenarioObject.Location.Row].Add(scenarioObject);

            scenarioObject.LocationChangedEvent += OnScenarioObjectLocationChanged;

            // Maintain array
            MaintainLevelContentsArray();
        }
        public void RemoveContent(ScenarioObject scenarioObject)
        {
            if (!_levelContent.Contains(scenarioObject))
                throw new Exception("Trying to remove non-existent Scenario Object from Level");

            if (scenarioObject is NonPlayerCharacter)
                _nonPlayerCharacters.Remove(scenarioObject as NonPlayerCharacter);

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
            _levelContent.Remove(scenarioObject);

            // If the CellPoint is empty then the object is being removed before it's mapped (by the Generators). So,
            // this is safe to do. The CellPoint should never be set to Empty by any of the in-game components (Logic)
            if (scenarioObject.Location != GridLocation.Empty)
                _levelContentGrid[scenarioObject.Location.Column, scenarioObject.Location.Row].Remove(scenarioObject);

            scenarioObject.LocationChangedEvent -= OnScenarioObjectLocationChanged;

            // Maintain array
            MaintainLevelContentsArray();
        }

        /// <summary>
        /// Unloads data and returns extracted content to be moved with the Player
        /// </summary>
        public IEnumerable<ScenarioObject> Unload()
        {
            //foreach (var scenarioObject in _levelContent)
            //{
            //    scenarioObject.LocationChangedEvent -= OnScenarioObjectLocationChanged;
            //}

            // Remove Temporary Characters
            var temporaryCharacters = _nonPlayerCharacters.Where(x => x is TemporaryCharacter).ToList();
            foreach (var character in temporaryCharacters)
                RemoveContent(character);

            // Remove Friendlies in Player Party
            var friendlies = _levelContent.Where(x => x is Friendly)
                                          .Cast<Friendly>()
                                          .Where(x => x.InPlayerParty)
                                          .Actualize();

            foreach (var friendly in friendlies)
                RemoveContent(friendly);

            return friendlies;
        }

        private void MaintainLevelContentsArray()
        {
            _levelContentArray = _levelContent.ToArray();
            _levelContentDict = _levelContent.ToDictionary(x => x.Id, x => x);
        }

        public ScenarioObject[] GetContents()
        {
            return _levelContentArray;
        }
        public ScenarioObject GetContent(string id)
        {
            return _levelContentDict[id];
        }

        public bool HasContent(string id)
        {
            return _levelContentDict.ContainsKey(id);
        }

        /// <summary>
        /// Checks level contents to see if cell is occupied. (NOTE** Must provide player location as well)
        /// </summary>
        /// <param name="cellPoint">point in question</param>
        /// <param name="playerLocation">player location (not provided by Level)</param>
        public bool IsCellOccupied(GridLocation location, GridLocation playerLocation)
        {
            return (_levelContentGrid[location.Column, location.Row].Count > 0) || (location == playerLocation);
        }
        public bool IsCellOccupiedByCharacter(GridLocation location, GridLocation playerLocation)
        {
            return _levelContentGrid[location.Column, location.Row].Any(x => x is CharacterBase) || (location == playerLocation);
        }        

        /// <summary>
        /// Returns the First-Or-Default object of type T located at the cellPoint
        /// </summary>
        public T GetAt<T>(GridLocation location) where T : ScenarioObject
        {
            var scenarioObjects = _levelContentGrid[location.Column, location.Row];

            return (T)scenarioObjects.Where(x => x is T).Cast<T>().FirstOrDefault();
        }

        public IEnumerable<T> GetManyAt<T>(GridLocation location) where T : ScenarioObject
        {
            return _levelContentGrid[location.Column, location.Row].Where(x => x is T).Cast<T>();
        }

        private void RebuildContentGrid()
        {
            for (int i = 0; i < _levelContentGrid.GetLength(0); i++)
            {
                for (int j = 0; j < _levelContentGrid.GetLength(1); j++)
                    _levelContentGrid[i, j] = new List<ScenarioObject>();
            }

            foreach (var scenarioObject in _levelContent)
                _levelContentGrid[scenarioObject.Location.Column, scenarioObject.Location.Row].Add(scenarioObject);
        }
        private void OnScenarioObjectLocationChanged(object sender, LocationChangedEventArgs e)
        {
            if (e.OldLocation != GridLocation.Empty)
                _levelContentGrid[e.OldLocation.Column, e.OldLocation.Row].Remove(e.ScenarioObject);

            if (e.NewLocation != GridLocation.Empty)
                _levelContentGrid[e.NewLocation.Column, e.NewLocation.Row].Add(e.ScenarioObject);
        }
    }
}

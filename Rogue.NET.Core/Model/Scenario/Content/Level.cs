using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Content.Doodad;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Core.Model.Scenario.Content.Layout;

using System;
using System.Linq;
using System.Collections.Generic;
using Rogue.NET.Core.Model.Scenario.Dynamic;
using Rogue.NET.Core.Model.Event;

namespace Rogue.NET.Core.Model.Scenario
{
    [Serializable]
    public class Level : IDisposable
    {
        public LevelGrid Grid { get; protected set; }
        public LayoutType Type { get; protected set; }

        public int Number { get; protected set; }

        public bool HasStairsUp { get { return this.StairsUp != null; } }
        public bool HasStairsDown { get { return this.StairsDown != null; } }
        public bool HasSavePoint { get { return this.SavePoint != null; } }

        public DoodadNormal StairsUp { get; protected set; }
        public DoodadNormal StairsDown { get; protected set; }
        public DoodadNormal SavePoint { get; protected set; }

        IList<Enemy> _enemies;
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

        public IEnumerable<Enemy> Enemies
        {
            get { return _enemies; }
            protected set { _enemies = new List<Enemy>(value); }
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

        //Statistics
        public int StepsTaken { get; set; }
        public int MonsterScore { get; set; }   //Monster Killed * Experience for that monster
        public Dictionary<string, int> MonstersKilled { get; set; }
        public Dictionary<string, int> ItemsFound { get; set; }

        public Level() { } 
        public Level(LevelGrid grid, LayoutType layoutType, int number)
        {
            this.Type = layoutType;
            this.Grid = grid;
            this.Number = number;
            this.MonstersKilled = new Dictionary<string, int>();
            this.ItemsFound = new Dictionary<string, int>();

            this.StairsDown = null;
            this.StairsUp = null;
            this.SavePoint = null;

            this.Enemies = new List<Enemy>();
            this.Doodads = new List<DoodadMagic>();
            this.Equipment = new List<Equipment>();
            this.DoodadsNormal = new List<DoodadNormal>();
            this.Consumables = new List<Consumable>();

            _levelContent = new List<ScenarioObject>();
            _levelContentGrid = new List<ScenarioObject>[grid.GetBounds().Right, grid.GetBounds().Bottom];
            _levelContentArray = new ScenarioObject[] { };

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
            this.StairsDown = savePoint;

            AddContent(savePoint);
        }
        public void AddContent(ScenarioObject scenarioObject)
        {
            if (_levelContent.Contains(scenarioObject))
                throw new Exception("Trying to add duplicate Scenario Object to Level");

            if (scenarioObject is Enemy)
                _enemies.Add(scenarioObject as Enemy);

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
            if (scenarioObject.Location != CellPoint.Empty)
                _levelContentGrid[scenarioObject.Location.Column, scenarioObject.Location.Row].Add(scenarioObject);

            scenarioObject.LocationChangedEvent += OnScenarioObjectLocationChanged;

            // Maintain array
            _levelContentArray = _levelContent.ToArray();
        }
        public void RemoveContent(ScenarioObject scenarioObject)
        {
            if (!_levelContent.Contains(scenarioObject))
                throw new Exception("Trying to remove non-existent Scenario Object from Level");

            if (scenarioObject is Enemy)
                _enemies.Remove(scenarioObject as Enemy);

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
            if (scenarioObject.Location != CellPoint.Empty)
                _levelContentGrid[scenarioObject.Location.Column, scenarioObject.Location.Row].Remove(scenarioObject);

            scenarioObject.LocationChangedEvent -= OnScenarioObjectLocationChanged;

            _levelContentArray = _levelContent.ToArray();
        }

        public ScenarioObject[] GetContents()
        {
            return _levelContentArray;
        }
        /// <summary>
        /// Checks level contents to see if cell is occupied. (NOTE** Must provide player location as well)
        /// </summary>
        /// <param name="cellPoint">point in question</param>
        /// <param name="playerLocation">player location (not provided by Level)</param>
        /// <returns></returns>
        public bool IsCellOccupied(CellPoint cellPoint, CellPoint playerLocation)
        {
            return _levelContentGrid[cellPoint.Column, cellPoint.Row].Count > 0 || cellPoint == playerLocation;
        }
        public bool IsCellOccupiedByEnemy(CellPoint cellPoint)
        {
            return _levelContentGrid[cellPoint.Column, cellPoint.Row].Any(x => x is Enemy);
        }        

        /// <summary>
        /// Returns the First-Or-Default object of type T located at the cellPoint
        /// </summary>
        public T GetAtPoint<T>(CellPoint cellPoint) where T : ScenarioObject
        {
            var scenarioObjects = _levelContentGrid[cellPoint.Column, cellPoint.Row];

            return (T)scenarioObjects.FirstOrDefault(x => x is T);
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
            if (e.OldLocation != CellPoint.Empty)
                _levelContentGrid[e.OldLocation.Column, e.OldLocation.Row].Remove(e.ScenarioObject);

            if (e.NewLocation != CellPoint.Empty)
                _levelContentGrid[e.NewLocation.Column, e.NewLocation.Row].Add(e.ScenarioObject);
        }

        #region IDisposable
        public void Dispose()
        {
            foreach (var scenarioObject in _levelContent)
            {
                scenarioObject.LocationChangedEvent -= OnScenarioObjectLocationChanged;
            }

            _levelContent.Clear();
        }
        #endregion
    }
}

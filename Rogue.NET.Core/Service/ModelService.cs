using Rogue.NET.Core.Logic.Content.Interface;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Service.Interface;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Logic.Interface;
using Rogue.NET.Core.Logic.Algorithm.Interface;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.IO;

namespace Rogue.NET.Core.Service
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IModelService))]
    public class ModelService : IModelService
    {
        readonly ILayoutEngine _layoutEngine;
        readonly IRayTracer _rayTracer;
        readonly ICharacterProcessor _characterProcessor;

        // Explored location collection calculated each time the player moves; and kept up to date
        IEnumerable<CellPoint> _exploredLocations;

        // Visible location collection calculated each time the player moves
        IEnumerable<CellPoint> _visibleLocations;

        // Effected locations between player moves (required for processing on the UI)
        IEnumerable<CellPoint> _effectedLocations;

        // Collection of targeted enemies
        IList<Enemy> _targetedEnemies;

        [ImportingConstructor]
        public ModelService(ILayoutEngine layoutEngine, IRayTracer rayTracer, ICharacterProcessor characterProcessor)
        {
            _layoutEngine = layoutEngine;
            _rayTracer = rayTracer;
            _characterProcessor = characterProcessor;

            _exploredLocations = new List<CellPoint>();
            _visibleLocations = new List<CellPoint>();
            _effectedLocations = new List<CellPoint>();
            _targetedEnemies = new List<Enemy>();
        }

        public void Load(
            Player player, 
            PlayerStartLocation startLocation,
            Level level, 
            IDictionary<string, ScenarioMetaData> encyclopedia, 
            ScenarioConfigurationContainer configuration)
        {
            this.Level = level;
            this.Player = player;
            this.ScenarioEncyclopedia = encyclopedia;
            this.ScenarioConfiguration = configuration;

            switch (startLocation)
            {
                case PlayerStartLocation.SavePoint:
                    if (level.HasSavePoint)
                        player.Location = level.SavePoint.Location;
                    else
                        player.Location = level.StairsUp.Location;
                    break;
                case PlayerStartLocation.StairsUp:
                    player.Location = level.StairsUp.Location;
                    break;
                case PlayerStartLocation.Random:
                    player.Location = _layoutEngine.GetRandomLocation(level, true);
                    break;
            }

            UpdateVisibleLocations();
            UpdateContents();
        }

        public void Unload()
        {
            this.Level = null;
            this.Player = null;
            this.ScenarioConfiguration = null;
            this.ScenarioEncyclopedia = null;

            _exploredLocations = new List<CellPoint>();
            _visibleLocations = new List<CellPoint>();
            _effectedLocations = new List<CellPoint>();
            _targetedEnemies = new List<Enemy>();
        }

        public Level Level { get; private set; }

        public Player Player { get; private set; }

        public IDictionary<string, ScenarioMetaData> ScenarioEncyclopedia { get; private set; }

        public ScenarioConfigurationContainer ScenarioConfiguration { get; private set; }

        public string GetDisplayName(string rogueName)
        {
            return this.ScenarioEncyclopedia[rogueName].IsIdentified ? rogueName : ModelConstants.UN_IDENTIFIED_DISPLAY_NAME;
        }
        public IEnumerable<Enemy> GetTargetedEnemies()
        {
            return _targetedEnemies;
        }
        public void ClearTargetedEnemies()
        {
            _targetedEnemies.Clear();
        }
        public IEnumerable<Enemy> GetVisibleEnemies()
        {
            return this.Level
                       .Enemies
                       .Where(x => _visibleLocations.Contains(x.Location));
        }
        public IEnumerable<ScenarioObject> GetVisibleContents()
        {
            return this.Level
                       .GetContents()
                       .Where(x => _visibleLocations.Contains(x.Location));
        }
        public IEnumerable<CellPoint> GetVisibleLocations()
        {
            return _visibleLocations;
        }
        public IEnumerable<CellPoint> GetExploredLocations()
        {
            return _exploredLocations;
        }

        public void SetTargetedEnemy(Enemy enemy)
        {
            if (!_targetedEnemies.Contains(enemy))
                _targetedEnemies.Add(enemy);
        }
        public void UpdateContents()
        {
            foreach (var location in _effectedLocations)
            {
                var scenarioObject = this.Level.GetAtPoint<ScenarioObject>(location);

                if (scenarioObject != null)
                {
                    var cell = this.Level.Grid.GetCell(location);

                    scenarioObject.IsExplored = cell.IsPhysicallyVisible;
                    scenarioObject.IsPhysicallyVisible = cell.IsPhysicallyVisible;

                    // Set this based on whether the cell is physically visible. Once the cell is seen
                    // the IsRevealed flag gets reset. So, if it's set and the cell isn't visible then
                    // don't reset it just yet.
                    scenarioObject.IsRevealed = scenarioObject.IsRevealed || !cell.IsPhysicallyVisible;
                }
            }

        }
        public void UpdateVisibleLocations()
        {
            var lightRadius = _characterProcessor.GetAuraRadius(this.Player);

            // If blind - no visible locations
            var visibleLocations = this.Player.Alteration.GetStates().Any(z => z == CharacterStateType.Blind) ?
                                   _layoutEngine.GetAdjacentLocations(this.Level.Grid, this.Player.Location) : 
                                   _rayTracer.GetVisibleLocations(this.Level.Grid, this.Player.Location, (int)lightRadius);

            // Reset flag for currently visible locations
            foreach (var cell in _visibleLocations.Select(x => this.Level.Grid[x.Column, x.Row]))
            {
                cell.IsPhysicallyVisible = false;
            }

            // Set flags for newly calculated visible locations
            foreach (var cell in visibleLocations.Select(x => this.Level.Grid[x.Column, x.Row]))
            {
                cell.IsPhysicallyVisible = true;
                cell.IsExplored = true;

                //No longer have to highlight revealed cells
                cell.IsRevealed = false;
            }

            // Update all effected cell locations
            _effectedLocations = visibleLocations.Union(_visibleLocations).Distinct();

            // Update visible cell locations
            _visibleLocations = visibleLocations;

            // Update Explored locations
            _exploredLocations = this.Level
                         .Grid
                         .GetCells()
                         .Where(x => x.IsExplored)
                         .Select(x => x.Location)
                         .ToList();
        }
    }
}

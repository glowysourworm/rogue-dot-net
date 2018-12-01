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
using Rogue.NET.Core.Model.Scenario.Character.Extension;
using Rogue.NET.Core.Model.Generator.Interface;
using Rogue.NET.Core.Model.Scenario.Content.Extension;

namespace Rogue.NET.Core.Service
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IModelService))]
    public class ModelService : IModelService
    {
        readonly IRayTracer _rayTracer;
        readonly IRandomSequenceGenerator _randomSequenceGenerator;

        // Line of sight collection calculated each time player moves. This is used for enemy calculations.
        IEnumerable<CellPoint> _lineOfSightLocations;

        // Explored location collection calculated each time the player moves; and kept up to date
        IEnumerable<CellPoint> _exploredLocations;

        // Visible location collection calculated each time the player moves
        IEnumerable<CellPoint> _visibleLocations;

        // Revealed locations calculated on player move
        IEnumerable<CellPoint> _revealedLocations;

        // Collection of targeted enemies
        IList<Enemy> _targetedEnemies;

        // Enemy to have slain the player
        private Enemy _finalEnemy;

        [ImportingConstructor]
        public ModelService(IRayTracer rayTracer, IRandomSequenceGenerator randomSequenceGenerator)
        {
            _rayTracer = rayTracer;
            _randomSequenceGenerator = randomSequenceGenerator;

            _lineOfSightLocations = new List<CellPoint>();
            _exploredLocations = new List<CellPoint>();
            _visibleLocations = new List<CellPoint>();
            _revealedLocations = new List<CellPoint>();
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
                case PlayerStartLocation.StairsDown:
                    player.Location = level.StairsDown.Location;
                    break;
                case PlayerStartLocation.Random:
                    player.Location = level.GetRandomLocation(true, _randomSequenceGenerator);
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
            _lineOfSightLocations = new List<CellPoint>();
            _visibleLocations = new List<CellPoint>();
            _revealedLocations = new List<CellPoint>();
            _targetedEnemies = new List<Enemy>();
        }

        public Level Level { get; private set; }

        public Player Player { get; private set; }

        public IDictionary<string, ScenarioMetaData> ScenarioEncyclopedia { get; private set; }

        public ScenarioConfigurationContainer ScenarioConfiguration { get; private set; }

        public string GetDisplayName(string rogueName)
        {
            return this.ScenarioEncyclopedia[rogueName].IsIdentified ? rogueName : ModelConstants.UnIdentifiedDisplayName;
        }
        public IEnumerable<Enemy> GetTargetedEnemies()
        {
            return _targetedEnemies;
        }
        public void ClearTargetedEnemies()
        {
            _targetedEnemies.Clear();
        }
        public Enemy GetFinalEnemy()
        {
            return _finalEnemy;
        }
        public void SetFinalEnemy(Enemy enemy)
        {
            _finalEnemy = enemy;
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
        public IEnumerable<CellPoint> GetLineOfSightLocations()
        {
            return _lineOfSightLocations;
        }
        public IEnumerable<CellPoint> GetRevealedLocations()
        {
            return _revealedLocations;
        }

        public void SetTargetedEnemy(Enemy enemy)
        {
            if (!_targetedEnemies.Contains(enemy))
                _targetedEnemies.Add(enemy);
        }
        public void UpdateContents()
        {
            foreach (var scenarioObject in this.Level.GetContents())
            {
                var cell = this.Level.Grid.GetCell(scenarioObject.Location);

                scenarioObject.IsExplored = cell.IsPhysicallyVisible;
                scenarioObject.IsPhysicallyVisible = cell.IsPhysicallyVisible;

                // Set this based on whether the cell is physically visible. Once the cell is seen
                // the IsRevealed flag gets reset. So, if it's set and the cell isn't visible then
                // don't reset it just yet.
                scenarioObject.IsRevealed = scenarioObject.IsRevealed && !cell.IsPhysicallyVisible;
            }
        }
        public void UpdateVisibleLocations()
        {
            var lightRadius = this.Player.GetAuraRadius();

            // Perform Visibility Calculation - if player is blind then use light radius of 1.
            IEnumerable<CellPoint> lineOfSightLocations = null;
            IEnumerable<CellPoint> visibleLocations = 
                _rayTracer.CalculateVisibility(
                    this.Level.Grid, 
                    this.Player.Location, 
                    this.Player.IsBlind() ? 1 : (int)lightRadius, 
                    out lineOfSightLocations);

            // Reset flag for line of sight locations
            foreach (var cell in _lineOfSightLocations.Select(x => this.Level.Grid[x.Column, x.Row]))
            {
                cell.IsLineOfSight = false;
            }

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

            // Set flags for newly calculated line of sight locations
            foreach (var cell in lineOfSightLocations.Select(x => this.Level.Grid[x.Column, x.Row]))
            {
                cell.IsLineOfSight = true;
            }

            // Update visible cell locations
            _visibleLocations = visibleLocations;

            // Update line of sight locations
            _lineOfSightLocations = lineOfSightLocations;

            // Update Explored locations
            _exploredLocations = this.Level
                         .Grid
                         .GetCells()
                         .Where(x => x.IsExplored)
                         .Select(x => x.Location)
                         .ToList();

            // Update Revealed locations
            _revealedLocations = this.Level
                                     .Grid
                                     .GetCells()
                                     .Where(x => x.IsRevealed)
                                     .Select(x => x.Location)
                                     .ToList();
        }
    }
}

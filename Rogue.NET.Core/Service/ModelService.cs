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

namespace Rogue.NET.Core.Service
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IModelService))]
    public class ModelService : IModelService
    {
        readonly ILayoutEngine _layoutEngine;
        readonly IRayTracer _rayTracer;
        readonly ICharacterProcessor _characterProcessor;

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

            _visibleLocations = new List<CellPoint>();
            _effectedLocations = new List<CellPoint>();
            _targetedEnemies = new List<Enemy>();
        }

        public Level CurrentLevel { get; private set; }

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
            return this.CurrentLevel
                       .Enemies
                       .Where(x => _visibleLocations.Contains(x.Location));
        }
        public IEnumerable<CellPoint> GetVisibleLocations()
        {
            return _visibleLocations;
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
                var scenarioObject = this.CurrentLevel.GetAtPoint<ScenarioObject>(location);

                if (scenarioObject != null)
                {
                    var cell = this.CurrentLevel.Grid.GetCell(location);

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
                                   _layoutEngine.GetAdjacentLocations(this.Player.Location) : 
                                   _rayTracer.GetVisibleLocations(this.CurrentLevel.Grid, this.Player.Location, (int)lightRadius);

            // Reset flag for currently visible locations
            foreach (var cell in _visibleLocations.Select(x => this.CurrentLevel.Grid[x.Column, x.Row]))
            {
                cell.IsPhysicallyVisible = false;
            }

            // Set flags for newly calculated visible locations
            foreach (var cell in visibleLocations.Select(x => this.CurrentLevel.Grid[x.Column, x.Row]))
            {
                cell.IsPhysicallyVisible = true;
                cell.IsExplored = true;

                //No longer have to highlight revealed cells
                cell.IsRevealed = false;
            }

            //return all affected cells
            _effectedLocations = visibleLocations.Union(_visibleLocations).Distinct();
        }
    }
}

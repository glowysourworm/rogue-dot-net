using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Character.Extension;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Processing.Model.Content.Enum;
using Rogue.NET.Core.Processing.Service.Interface;
using System;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rogue.NET.Core.Processing.Service
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(ITargetingService))]
    public class TargetingService : ITargetingService
    {
        readonly IModelService _modelService;

        TargetType _targetType;
        GridLocation _targetLocation;
        GridLocation _targetTrackerLocation;
        CharacterBase _targetCharacter;

        [ImportingConstructor]
        public TargetingService(IModelService modelService)
        {
            _modelService = modelService;

            Clear();
        }

        public void Clear()
        {
            _targetType = TargetType.None;
            _targetLocation = null;
            _targetTrackerLocation = null;
            _targetCharacter = null;
        }

        public void EndTargeting()
        {
            if (IsValidTarget(_targetTrackerLocation))
            {
                CaptureTarget(_targetTrackerLocation);
            }
            else
            {
                Clear();
            }
        }

        public bool MoveTarget(Compass direction)
        {
            var location = _modelService.Level.GetPointInDirection(_targetTrackerLocation, direction);

            if (IsValidTarget(location))
            {
                _targetTrackerLocation = location;
                return true;
            }

            return false;
        }

        public void CycleTarget(Compass direction)
        {
            // Query for player visible locations
            var visibleLocations = _modelService.Level.Visibility.GetVisibleLocations();

            // Calculate enemies in range that are visible
            var enemiesInRange = _modelService.Level.Content.GetManyAt<Enemy>(visibleLocations).ToList();

            // Filter out invisible enemies
            if (!_modelService.Player.Alteration.CanSeeInvisible())
            {
                enemiesInRange = enemiesInRange.Where(x => !x.Is(CharacterStateType.Invisible)).ToList();
            }

            var targetedEnemy = _targetCharacter as Enemy;

            if (targetedEnemy != null)
            {
                int targetedEnemyIndex = enemiesInRange.IndexOf(targetedEnemy);
                switch (direction)
                {
                    case Compass.E:
                        {
                            if (targetedEnemyIndex + 1 == enemiesInRange.Count)
                                targetedEnemy = enemiesInRange[0];
                            else
                                targetedEnemy = enemiesInRange[targetedEnemyIndex + 1];
                        }
                        break;
                    case Compass.W:
                        {
                            if (targetedEnemyIndex - 1 == -1)
                                targetedEnemy = enemiesInRange[enemiesInRange.Count - 1];
                            else
                                targetedEnemy = enemiesInRange[targetedEnemyIndex - 1];
                        }
                        break;
                    default:
                        targetedEnemy = enemiesInRange[0];
                        break;
                }
            }
            else
            {
                if (enemiesInRange.Count > 0)
                    targetedEnemy = enemiesInRange[0];
            }

            // Set tracker on the selected enemy and update
            if (targetedEnemy != null)
                _targetTrackerLocation = _modelService.GetContentLocation(targetedEnemy);
        }

        public void StartTargeting(GridLocation location)
        {
            if (IsValidTarget(location))
                _targetTrackerLocation = location;

            else
                throw new Exception("Trying to start targeting from invalid location");
        }

        public CharacterBase GetTargetedCharacter()
        {
            return _targetCharacter;
        }

        public GridLocation GetTargetLocation()
        {
            return _targetLocation;
        }

        public TargetType GetTargetType()
        {
            return _targetType;
        }

        // Returns true if the location was valid
        private void CaptureTarget(GridLocation location)
        {
            var character = _modelService.Level.Content.GetAt<CharacterBase>(location);
            var gridCell = _modelService.Level.Grid[location.Column, location.Row];

            // Clear target tracker
            _targetTrackerLocation = null;

            if (gridCell == null)
            {
                _targetType = TargetType.None;
                _targetLocation = null;
                _targetCharacter = null;
            }

            else if (character != null)
            {
                _targetType = TargetType.Character;
                _targetLocation = null;
                _targetCharacter = character;
            }

            else
            {
                _targetType = TargetType.Location;
                _targetLocation = location;
                _targetCharacter = null;
            }
        }

        private bool IsValidTarget(GridLocation location)
        {
            if (location == null)
                return false;

            var gridCell = _modelService.Level.Grid[location.Column, location.Row];

            // Not valid location
            if (gridCell == null)
                return false;

            // Must be a visible location
            if (!_modelService.Level
                              .Visibility
                              .GetVisibleLocations()
                              .Contains(location))
                return false;

            return true;
        }

        public TargetType GetTrackedTargetType()
        {
            if (_targetTrackerLocation == null)
                return TargetType.None;

            var character = _modelService.Level.Content.GetAt<CharacterBase>(_targetTrackerLocation);
            var gridCell = _modelService.Level.Grid[_targetTrackerLocation.Column, _targetTrackerLocation.Row];

            if (gridCell == null)
                return TargetType.None;

            else if (character != null)
                return TargetType.Character;

            else if (gridCell.Location != null)
                return TargetType.Location;

            return TargetType.None;
        }

        public GridLocation GetTrackedTargetLocation()
        {
            return _targetTrackerLocation;
        }
    }
}

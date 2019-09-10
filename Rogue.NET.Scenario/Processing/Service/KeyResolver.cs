using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Processing.Command.Backend.CommandData;
using Rogue.NET.Core.Processing.Command.Frontend.Data;
using Rogue.NET.Core.Processing.Command.Frontend.Enum;
using Rogue.NET.Core.Processing.Command.View.CommandData;
using Rogue.NET.Model;
using Rogue.NET.Scenario.Processing.Service.Interface;
using System.ComponentModel.Composition;
using System.Windows.Input;

namespace Rogue.NET.Scenario.Processing.Service
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IKeyResolver))]
    public class KeyResolver : IKeyResolver
    {
        readonly CommandPreferencesViewModel _preferences;

        [ImportingConstructor]
        public KeyResolver()
        {
            _preferences = CommandPreferencesViewModel.GetDefaults();
        }

        public LevelCommandData ResolveLevelCommand(Key key, bool shift, bool ctrl, bool alt)
        {
            //Searching
            if (key == _preferences.Search)
                return new LevelCommandData(LevelCommandType.Search, Compass.Null, "");
            //Skill Usage
            else if (key == _preferences.Skill)
                return new LevelCommandData(LevelCommandType.InvokeSkill, Compass.Null, "");
            //Doodad Usage
            else if (key == _preferences.Doodad)
                return new LevelCommandData(LevelCommandType.InvokeDoodad, Compass.Null, "");
            //Fire Range Weapon
            else if (key == _preferences.Fire)
                return new LevelCommandData(LevelCommandType.Fire, Compass.Null, "");

            //Debug*******
#if DEBUG
            else if (key == Key.N)
                return shift ? new LevelCommandData(LevelCommandType.DebugSimulateNext, Compass.Null, "") :
                               new LevelCommandData(LevelCommandType.DebugNext, Compass.Null, "");
            else if (key == Key.W)
                return new LevelCommandData(LevelCommandType.DebugExperience, Compass.Null, "");
            else if (key == Key.Q)
                return new LevelCommandData(LevelCommandType.DebugIdentifyAll, Compass.Null, "");
            else if (key == Key.E)
                return new LevelCommandData(LevelCommandType.DebugRevealAll, Compass.Null, "");
#endif
            //Debug*******

            else
            {
                if (shift)
                    return ResolveCompassLevelAction(LevelCommandType.ToggleDoor, key);
                else if (ctrl)
                    return ResolveCompassLevelAction(LevelCommandType.Attack, key);
                else
                    return ResolveCompassLevelAction(LevelCommandType.Move, key);
            }
        }

        public PlayerCommandData ResolvePlayerCommand(Key key, bool shift, bool ctrl, bool alt)
        {
            //Cycle Skill
            if (key == _preferences.Skill && shift)
                return new PlayerCommandData(PlayerCommandType.CycleSkillSet, "");

            return null;
        }

        public ViewCommandData ResolveViewCommand(Key key, bool shift, bool ctrl, bool alt)
        {
            // Revolving Displays
            //
            // Equipment
            if (key == _preferences.ShowPlayerSubpanelEquipment && shift)
                return new ViewCommandData(ViewActionType.ShowPlayerSubpanelEquipment);

            // Consumables
            else if (key == _preferences.ShowPlayerSubpanelConsumables && shift)
                return new ViewCommandData(ViewActionType.ShowPlayerSubpanelConsumables);

            // Skills
            else if (key == _preferences.ShowPlayerSubpanelSkills && shift)
                return new ViewCommandData(ViewActionType.ShowPlayerSubpanelSkills);

            // Stats
            else if (key == _preferences.ShowPlayerSubpanelStats && shift)
                return new ViewCommandData(ViewActionType.ShowPlayerSubpanelStats);

            // Alterations
            else if (key == _preferences.ShowPlayerSubpanelAlterations && shift)
                return new ViewCommandData(ViewActionType.ShowPlayerSubpanelAlterations);

            return null;
        }

        public FrontendCommandData ResolveFrontendCommand(Key key, bool shift, bool ctrl, bool alt)
        {
            var direction = ResolveDirection(key);

            if (key == _preferences.Target)
                return new FrontendCommandData(FrontendCommandType.StartTargeting, Compass.Null);

            else if (IsDirectionKey(key) && shift)
                return new FrontendCommandData(FrontendCommandType.CycleTarget, direction);

            else if (IsDirectionKey(key))
                return new FrontendCommandData(FrontendCommandType.MoveTarget, direction);

            else if (key == _preferences.SelectTarget)
                return new FrontendCommandData(FrontendCommandType.SelectTarget, Compass.Null);

            else if (key == _preferences.EndTargeting)
                return new FrontendCommandData(FrontendCommandType.EndTargeting, Compass.Null);

            return null;
        }

        private Compass ResolveDirection(Key key)
        {
            if (key == _preferences.NorthWest)
                return Compass.NW;

            else if (key == _preferences.North)
                return Compass.N;

            else if (key == _preferences.NorthEast)
                return Compass.NE;

            else if (key == _preferences.West)
                return Compass.W;

            else if (key == _preferences.East)
                return Compass.E;

            else if (key == _preferences.SouthWest)
                return Compass.SW;

            else if (key == _preferences.South)
                return Compass.S;

            else if (key == _preferences.SouthEast)
                return Compass.SE;

            return Compass.Null;
        }

        private bool IsDirectionKey(Key key)
        {
            if (key == _preferences.North ||
                key == _preferences.South ||
                key == _preferences.East ||
                key == _preferences.West ||
                key == _preferences.NorthWest ||
                key == _preferences.NorthEast ||
                key == _preferences.SouthEast ||
                key == _preferences.SouthWest)
                return true;

            return false;
        }

        private LevelCommandData ResolveCompassLevelAction(LevelCommandType action, Key key)
        {
            if (key == _preferences.NorthWest)
                return new LevelCommandData(action, Compass.NW, "");
            else if (key == _preferences.North)
                return new LevelCommandData(action, Compass.N, "");
            else if (key == _preferences.NorthEast)
                return new LevelCommandData(action, Compass.NE, "");
            else if (key == _preferences.West)
                return new LevelCommandData(action, Compass.W, "");
            else if (key == _preferences.East)
                return new LevelCommandData(action, Compass.E, "");
            else if (key == _preferences.SouthWest)
                return new LevelCommandData(action, Compass.SW, "");
            else if (key == _preferences.South)
                return new LevelCommandData(action, Compass.S, "");
            else if (key == _preferences.SouthEast)
                return new LevelCommandData(action, Compass.SE, "");

            return null;
        }
    }
}

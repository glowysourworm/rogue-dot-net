using Prism.Events;
using Rogue.NET.Common.EventArgs;
using Rogue.NET.Common.Events.Scenario;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Model;
using Rogue.NET.Scenario.Service.Interface;
using System.ComponentModel.Composition;
using System.Windows.Input;

namespace Rogue.NET.Scenario.Service
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IKeyResolver))]
    public class KeyResolver : IKeyResolver
    {
        CommandPreferencesViewModel _preferences = null;

        [ImportingConstructor]
        public KeyResolver(IEventAggregator eventAggregator)
        {
            _preferences = CommandPreferencesViewModel.GetDefaults();
        }

        public Compass ResolveDirectionKey(Key k)
        {
            if (k == _preferences.NorthWest)
                return Compass.NW;
            else if (k == _preferences.North)
                return Compass.N;
            else if (k == _preferences.NorthEast)
                return Compass.NE;
            else if (k == _preferences.West)
                return Compass.W;
            else if (k == _preferences.East)
                return Compass.E;
            else if (k == _preferences.SouthWest)
                return Compass.SW;
            else if (k == _preferences.South)
                return Compass.S;
            else if (k == _preferences.SouthEast)
                return Compass.SE;

            return Compass.Null;
        }
        public Compass ResolveDirectionArrow(Key k)
        {
            switch (k)
            {
                case Key.Up:
                    return Compass.N;
                case Key.Down:
                    return Compass.S;
                case Key.Right:
                    return Compass.E;
                case Key.Left:
                    return Compass.W;
            }
            return Compass.Null;
        }
        public UserCommandEventArgs ResolveKeys(Key key, bool shift, bool ctrl, bool alt)
        {
            //Searching
            if (key == _preferences.Search)
                return new UserCommandEventArgs(LevelAction.Search, Compass.Null, "");
            //Target
            else if (key == _preferences.Target)
                return new UserCommandEventArgs(LevelAction.Target, Compass.E, "");
            //Cycle Skill
            else if (key == _preferences.Skill && shift)
                return new UserCommandEventArgs(LevelAction.CycleSkillSet, Compass.Null, "");
            //Skill Usage
            else if (key == _preferences.Skill)
                return new UserCommandEventArgs(LevelAction.InvokeSkill, Compass.Null, "");
            //Doodad Usage
            else if (key == _preferences.Doodad)
                return new UserCommandEventArgs(LevelAction.InvokeDoodad, Compass.Null, "");
            //Fire Range Weapon
            else if (key == _preferences.Fire)
                return new UserCommandEventArgs(LevelAction.Fire, Compass.Null, "");
            //Renounce Religion
            else if (key == _preferences.RenounceReligion)
                return new UserCommandEventArgs(LevelAction.RenounceReligion, Compass.Null, "");

            //Debug*******
#if DEBUG
            else if (key == Key.N)
                return new UserCommandEventArgs(LevelAction.DebugNext, Compass.Null, "");
            else if (key == Key.W)
                return new UserCommandEventArgs(LevelAction.DebugExperience, Compass.Null, "");
            else if (key == Key.Q)
                return new UserCommandEventArgs(LevelAction.DebugIdentifyAll, Compass.Null, "");
#endif
            //Debug*******

            else
            {
                if (shift)
                    return ProcessCompassLevelAction(LevelAction.ToggleDoor, key);
                else if (ctrl)
                    return ProcessCompassLevelAction(LevelAction.Attack, key);
                else
                    return ProcessCompassLevelAction(LevelAction.Move, key);
            }
        }
        private UserCommandEventArgs ProcessCompassLevelAction(LevelAction action, Key key)
        {
            if (key == _preferences.NorthWest)
                return new UserCommandEventArgs(action, Compass.NW, "");
            else if (key == _preferences.North)
                return new UserCommandEventArgs(action, Compass.N, "");
            else if (key == _preferences.NorthEast)
                return new UserCommandEventArgs(action, Compass.NE, "");
            else if (key == _preferences.West)
                return new UserCommandEventArgs(action, Compass.W, "");
            else if (key == _preferences.East)
                return new UserCommandEventArgs(action, Compass.E, "");
            else if (key == _preferences.SouthWest)
                return new UserCommandEventArgs(action, Compass.SW, "");
            else if (key == _preferences.South)
                return new UserCommandEventArgs(action, Compass.S, "");
            else if (key == _preferences.SouthEast)
                return new UserCommandEventArgs(action, Compass.SE, "");

            return null;
        }
    }
}

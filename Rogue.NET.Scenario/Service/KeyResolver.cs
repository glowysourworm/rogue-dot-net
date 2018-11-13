using Prism.Events;
using Rogue.NET.Common.EventArgs;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Model;
using Rogue.NET.Scenario.Service.Interface;
using System.ComponentModel.Composition;
using System.Windows.Input;

namespace Rogue.NET.Scenario.Service
{
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
        public LevelCommandEventArgs ResolveKeys(Key key, bool shift, bool ctrl, bool alt)
        {
            //Searching
            if (key == _preferences.Search)
                return new LevelCommandEventArgs(LevelAction.Search, Compass.Null, "");
            //Target
            else if (key == _preferences.Target)
                return new LevelCommandEventArgs(LevelAction.Target, Compass.E, "");
            //Skill Usage
            else if (key == _preferences.Skill)
                return new LevelCommandEventArgs(shift ? LevelAction.CycleActiveSkill : LevelAction.InvokeSkill, Compass.Null, "");
            //Doodad Usage
            else if (key == _preferences.Doodad)
                return new LevelCommandEventArgs(LevelAction.InvokeDoodad, Compass.Null, "");
            //Fire Range Weapon
            else if (key == _preferences.Fire)
                return new LevelCommandEventArgs(LevelAction.Fire, Compass.Null, "");

//Debug*******
#if DEBUG
            else if (key == Key.N)
                return new LevelCommandEventArgs(LevelAction.DebugNext, Compass.Null, "");
            else if (key == Key.W)
                return new LevelCommandEventArgs(LevelAction.DebugExperience, Compass.Null, "");
            else if (key == Key.Q)
                return new LevelCommandEventArgs(LevelAction.DebugIdentifyAll, Compass.Null, "");
            else if (key == Key.A)
                return new LevelCommandEventArgs(LevelAction.DebugSkillUp, Compass.Null, "");
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
        private LevelCommandEventArgs ProcessCompassLevelAction(LevelAction action, Key key)
        {
            if (key == _preferences.NorthWest)
                return new LevelCommandEventArgs(action, Compass.NW, "");
            else if (key == _preferences.North)
                return new LevelCommandEventArgs(action, Compass.N, "");
            else if (key == _preferences.NorthEast)
                return new LevelCommandEventArgs(action, Compass.NE, "");
            else if (key == _preferences.West)
                return new LevelCommandEventArgs(action, Compass.W, "");
            else if (key == _preferences.East)
                return new LevelCommandEventArgs(action, Compass.E, "");
            else if (key == _preferences.SouthWest)
                return new LevelCommandEventArgs(action, Compass.SW, "");
            else if (key == _preferences.South)
                return new LevelCommandEventArgs(action, Compass.S, "");
            else if (key == _preferences.SouthEast)
                return new LevelCommandEventArgs(action, Compass.SE, "");

            return null;
        }
    }
}

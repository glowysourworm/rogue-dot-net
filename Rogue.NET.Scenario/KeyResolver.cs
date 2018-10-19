using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.PubSubEvents;
using Rogue.NET.Common;
using Rogue.NET.Common.EventArgs;
using Rogue.NET.Common.Events.Scenario;
using Rogue.NET.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Rogue.NET.Scenario
{
    public class KeyResolver
    {
        CommandPreferences _preferences = null;

        public KeyResolver(IEventAggregator eventAggregator)
        {
            ReloadFromFile();

            eventAggregator.GetEvent<CommandPreferencesChangedEvent>().Subscribe((e) =>
            {
                _preferences = ResourceManager.GetCommandPreferences();
            });
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
        public LevelCommandEventArgs ResolveKeys(Key k, bool shift, bool ctrl, bool alt)
        {
            //Searching
            if (k == _preferences.Search)
                return new LevelCommandEventArgs(LevelAction.Search, Compass.Null, "");
            //Target
            else if (k == _preferences.Target)
                return new LevelCommandEventArgs(LevelAction.Target, Compass.E, "");
            //Skill Usage
            else if (k == _preferences.Skill)
                return new LevelCommandEventArgs(shift ? LevelAction.CycleActiveSkill : LevelAction.InvokeSkill, Compass.Null, "");
            //Doodad Usage
            else if (k == _preferences.Doodad)
                return new LevelCommandEventArgs(LevelAction.InvokeDoodad, Compass.Null, "");
            //Fire Range Weapon
            else if (k == _preferences.Fire)
                return new LevelCommandEventArgs(LevelAction.Fire, Compass.Null, "");

//Debug*******
#if DEBUG
            else if (k == Key.N)
                return new LevelCommandEventArgs(LevelAction.DebugNext, Compass.Null, "");
            else if (k == Key.W)
                return new LevelCommandEventArgs(LevelAction.DebugExperience, Compass.Null, "");
            else if (k == Key.Q)
                return new LevelCommandEventArgs(LevelAction.DebugIdentifyAll, Compass.Null, "");
            else if (k == Key.A)
                return new LevelCommandEventArgs(LevelAction.DebugSkillUp, Compass.Null, "");
#endif
            //Debug*******

            else
            {
                if (ctrl && shift)
                    return ProcessCompassLevelAction(LevelAction.Close, k);
                else if (shift)
                    return ProcessCompassLevelAction(LevelAction.Open, k);
                else if (ctrl)
                    return ProcessCompassLevelAction(LevelAction.Attack, k);
                else
                    return ProcessCompassLevelAction(LevelAction.Move, k);
            }
        }
        private LevelCommandEventArgs ProcessCompassLevelAction(LevelAction action, Key k)
        {
            if (k == _preferences.NorthWest)
                return new LevelCommandEventArgs(action, Compass.NW, "");
            else if (k == _preferences.North)
                return new LevelCommandEventArgs(action, Compass.N, "");
            else if (k == _preferences.NorthEast)
                return new LevelCommandEventArgs(action, Compass.NE, "");
            else if (k == _preferences.West)
                return new LevelCommandEventArgs(action, Compass.W, "");
            else if (k == _preferences.East)
                return new LevelCommandEventArgs(action, Compass.E, "");
            else if (k == _preferences.SouthWest)
                return new LevelCommandEventArgs(action, Compass.SW, "");
            else if (k == _preferences.South)
                return new LevelCommandEventArgs(action, Compass.S, "");
            else if (k == _preferences.SouthEast)
                return new LevelCommandEventArgs(action, Compass.SE, "");

            return null;
        }

        public void ReloadFromFile()
        {
            _preferences = ResourceManager.GetCommandPreferences();
        }
    }
}

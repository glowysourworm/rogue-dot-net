using Prism.Events;
using Rogue.NET.Core.Event.Scenario.Level.EventArgs;
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
                return new LevelCommandEventArgs(LevelActionType.Search, Compass.Null, "");
            //Target
            else if (key == _preferences.Target)
                return new LevelCommandEventArgs(LevelActionType.Target, Compass.E, "");
            //Cycle Skill
            else if (key == _preferences.Skill && shift)
                return new PlayerCommandEventArgs(PlayerActionType.CycleSkillSet, "");
            //Skill Usage
            else if (key == _preferences.Skill)
                return new LevelCommandEventArgs(LevelActionType.InvokeSkill, Compass.Null, "");
            //Doodad Usage
            else if (key == _preferences.Doodad)
                return new LevelCommandEventArgs(LevelActionType.InvokeDoodad, Compass.Null, "");
            //Fire Range Weapon
            else if (key == _preferences.Fire)
                return new LevelCommandEventArgs(LevelActionType.Fire, Compass.Null, "");
            //Renounce Religion
            else if (key == _preferences.RenounceReligion)
                return new LevelCommandEventArgs(LevelActionType.RenounceReligion, Compass.Null, "");

            // Revolving Displays
            //
            // Equipment
            else if (key == _preferences.ShowPlayerSubpanelEquipment && shift)
                return new ViewCommandEventArgs(ViewActionType.ShowPlayerSubpanelEquipment);

            // Consumables
            else if (key == _preferences.ShowPlayerSubpanelConsumables && shift)
                return new ViewCommandEventArgs(ViewActionType.ShowPlayerSubpanelConsumables);

            // Skills
            else if (key == _preferences.ShowPlayerSubpanelSkills && shift)
                return new ViewCommandEventArgs(ViewActionType.ShowPlayerSubpanelSkills);

            // Stats
            else if (key == _preferences.ShowPlayerSubpanelStats && shift)
                return new ViewCommandEventArgs(ViewActionType.ShowPlayerSubpanelStats);

            // Alterations
            else if (key == _preferences.ShowPlayerSubpanelAlterations && shift)
                return new ViewCommandEventArgs(ViewActionType.ShowPlayerSubpanelAlterations);

            //Debug*******
#if DEBUG
            else if (key == Key.N)
                return shift ? new LevelCommandEventArgs(LevelActionType.DebugSimulateNext, Compass.Null, "") :
                               new LevelCommandEventArgs(LevelActionType.DebugNext, Compass.Null, "");
            else if (key == Key.W)
                return new LevelCommandEventArgs(LevelActionType.DebugExperience, Compass.Null, "");
            else if (key == Key.Q)
                return new LevelCommandEventArgs(LevelActionType.DebugIdentifyAll, Compass.Null, "");
            else if (key == Key.E)
                return new LevelCommandEventArgs(LevelActionType.DebugRevealAll, Compass.Null, "");
#endif
            //Debug*******

            else
            {
                if (shift)
                    return ProcessCompassLevelAction(LevelActionType.ToggleDoor, key);
                else if (ctrl)
                    return ProcessCompassLevelAction(LevelActionType.Attack, key);
                else
                    return ProcessCompassLevelAction(LevelActionType.Move, key);
            }
        }
        private UserCommandEventArgs ProcessCompassLevelAction(LevelActionType action, Key key)
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

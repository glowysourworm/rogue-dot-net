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
                return new UserCommandEventArgs(LevelActionType.Search, Compass.Null, "");
            //Target
            else if (key == _preferences.Target)
                return new UserCommandEventArgs(LevelActionType.Target, Compass.E, "");
            //Cycle Skill
            else if (key == _preferences.Skill && shift)
                return new UserCommandEventArgs(LevelActionType.CycleSkillSet, Compass.Null, "");
            //Skill Usage
            else if (key == _preferences.Skill)
                return new UserCommandEventArgs(LevelActionType.InvokeSkill, Compass.Null, "");
            //Doodad Usage
            else if (key == _preferences.Doodad)
                return new UserCommandEventArgs(LevelActionType.InvokeDoodad, Compass.Null, "");
            //Fire Range Weapon
            else if (key == _preferences.Fire)
                return new UserCommandEventArgs(LevelActionType.Fire, Compass.Null, "");
            //Renounce Religion
            else if (key == _preferences.RenounceReligion)
                return new UserCommandEventArgs(LevelActionType.RenounceReligion, Compass.Null, "");

            // Revolving Displays
            //
            // Equipment
            else if (key == _preferences.ShowPlayerSubpanelEquipment && shift)
                return new UserCommandEventArgs(ViewActionType.ShowPlayerSubpanelEquipment);

            // Consumables
            else if (key == _preferences.ShowPlayerSubpanelConsumables && shift)
                return new UserCommandEventArgs(ViewActionType.ShowPlayerSubpanelConsumables);

            // Skills
            else if (key == _preferences.ShowPlayerSubpanelSkills && shift)
                return new UserCommandEventArgs(ViewActionType.ShowPlayerSubpanelSkills);

            // Stats
            else if (key == _preferences.ShowPlayerSubpanelStats && shift)
                return new UserCommandEventArgs(ViewActionType.ShowPlayerSubpanelStats);

            // Alterations
            else if (key == _preferences.ShowPlayerSubpanelAlterations && shift)
                return new UserCommandEventArgs(ViewActionType.ShowPlayerSubpanelAlterations);

            // Religion
            else if (key == _preferences.ShowPlayerSubpanelReligion && shift)
                return new UserCommandEventArgs(ViewActionType.ShowPlayerSubpanelReligion);

            //Debug*******
#if DEBUG
            else if (key == Key.N)
                return shift ? new UserCommandEventArgs(LevelActionType.DebugSimulateNext, Compass.Null, "") :
                               new UserCommandEventArgs(LevelActionType.DebugNext, Compass.Null, "");
            else if (key == Key.W)
                return new UserCommandEventArgs(LevelActionType.DebugExperience, Compass.Null, "");
            else if (key == Key.Q)
                return new UserCommandEventArgs(LevelActionType.DebugIdentifyAll, Compass.Null, "");
            else if (key == Key.E)
                return new UserCommandEventArgs(LevelActionType.DebugRevealAll, Compass.Null, "");
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

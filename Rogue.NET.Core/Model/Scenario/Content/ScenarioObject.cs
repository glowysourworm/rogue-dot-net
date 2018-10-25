using Rogue.NET.Core.Event;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Layout;

using System;

namespace Rogue.NET.Core.Model.Scenario.Content
{
    [Serializable]
    public abstract class ScenarioObject : ScenarioImage
    {
        CellPoint _location;

        /// <summary>
        /// Event fired when the Location reference is changed. (Note*** CellPoint references 
        /// are maintained solely by the LevelGrid. No other references should be allowed that aren't
        /// contianed there.
        /// </summary>
        public event EventHandler<LocationChangedEventArgs> LocationChangedEvent;

        public CellPoint Location
        {
            get { return _location; }
            set
            {
                if (_location != value)
                {
                    var oldLocation = _location;
                    
                    _location = value;

                    if (this.LocationChangedEvent != null)
                        this.LocationChangedEvent(this, new LocationChangedEventArgs()
                        {
                            ScenarioObject = this,
                            OldLocation = oldLocation,
                            NewLocation = value
                        });
                }
            }
        }

        public bool IsExplored { get; set; }
        public bool IsHidden { get; set; }
        public bool IsRevealed { get; set; }
        public bool IsPhysicallyVisible { get; set; }

        public ScenarioObject()
        {
            this.RogueName = "Unnamed";
        }
        public ScenarioObject(string name, ImageResources icon)
            : base(name, icon)
        {
            this.Location = new CellPoint();
        }
        public ScenarioObject(string name, string symbol, string color)
            : base(name, symbol, color)
        {
            this.Location = new CellPoint();
        }
        public ScenarioObject(string name, SmileyMoods mood, string body, string line, string aura)
            : base(name, mood, body, line, aura)
        {
            this.Location = new CellPoint();
        }

        public override string ToString()
        {
            return this.RogueName;
        }
    }
}

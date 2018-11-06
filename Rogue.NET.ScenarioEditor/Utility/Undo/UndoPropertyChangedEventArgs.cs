using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.ScenarioEditor.Utility.Undo
{
    public class UndoPropertyChangedEventArgs : PropertyChangedEventArgs
    {
        public string PropertyChangingEventId { get; private set; }

        public UndoPropertyChangedEventArgs(string propertyChangingEventId, string propertyName) : base(propertyName)
        {
            this.PropertyChangingEventId = propertyChangingEventId;
        }
    }
}

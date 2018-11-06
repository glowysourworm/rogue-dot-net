using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.ScenarioEditor.Utility.Undo
{
    public class UndoPropertyChangingEventArgs : PropertyChangingEventArgs
    {
        public string Id { get; private set; }

        public UndoPropertyChangingEventArgs(string propertyName) : base(propertyName)
        {
            this.Id = System.Guid.NewGuid().ToString();
        }
    }
}

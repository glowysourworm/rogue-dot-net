using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.ScenarioEditor.Events
{
    public class AddAlteredCharacterStateEventArgs : EventArgs
    {
        public string Name { get; set; }
        public ImageResources Icon { get; set; }
        public CharacterStateType BaseType { get; set; }
    }

    public class AddAlteredCharacterStateEvent : RogueEvent<AddAlteredCharacterStateEventArgs>
    {
    }
}

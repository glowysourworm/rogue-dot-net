using Microsoft.Practices.Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.ScenarioEditor.Events
{
    public class LoadConstructionEvent : CompositePresentationEvent<LoadConstructionEvent>
    {
        public string ConstructionName { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Scenario.Content.ViewModel.Attribute
{
    /// <summary>
    /// Attribute for specifying some UI parameters for displaying a type or enum
    /// </summary>
    public class UIDisplayAttribute : System.Attribute
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}

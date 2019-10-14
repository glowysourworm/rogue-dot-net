using Rogue.NET.Core.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Rogue.NET.Core.Processing.Service.Cache
{
    /// <summary>
    /// Class for storing scenario data (something like a file header) when it's read into the cache
    /// </summary>
    public class ScenarioInfo
    {
        public string Name { get; set; }
        public SmileyExpression SmileyExpression { get; set; }
        public Color SmileyBodyColor { get; set; }
        public Color SmileyLineColor { get; set; }
    }
}

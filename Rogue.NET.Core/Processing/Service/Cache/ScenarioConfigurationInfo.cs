using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using System.Collections.Generic;
using System.Windows.Media;

namespace Rogue.NET.Core.Processing.Service.Cache
{
    public class ScenarioConfigurationInfo
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public SmileyExpression SmileyExpression { get; set; }
        public Color SmileyBodyColor { get; set; }
        public Color SmileyLineColor { get; set; }
        public IEnumerable<PlayerTemplate> PlayerTemplates { get; set; }
    }
}

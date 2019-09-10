using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Processing.Command.Frontend.Enum;

namespace Rogue.NET.Core.Processing.Command.Frontend.Data
{
    public class FrontendCommandData
    {
        public FrontendCommandType Type { get; set; }
        public Compass Direction { get; set; }

        public FrontendCommandData(FrontendCommandType type, Compass direction)
        {
            this.Type = type;
            this.Direction = direction;
        }
    }
}
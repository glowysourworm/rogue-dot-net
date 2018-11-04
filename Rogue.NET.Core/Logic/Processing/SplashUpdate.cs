using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Core.Logic.Processing.Interface;

namespace Rogue.NET.Core.Logic.Processing
{
    public class SplashUpdate : ISplashUpdate
    {
        public SplashEventType SplashType { get; set; }
        public SplashAction SplashAction { get; set; }
    }
}

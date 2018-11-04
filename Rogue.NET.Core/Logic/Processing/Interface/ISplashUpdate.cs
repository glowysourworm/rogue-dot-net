using Rogue.NET.Core.Logic.Processing.Enum;

namespace Rogue.NET.Core.Logic.Processing.Interface
{
    public interface ISplashUpdate
    {
        SplashEventType SplashType { get; set; }
        SplashAction SplashAction { get; set; }
    }
}

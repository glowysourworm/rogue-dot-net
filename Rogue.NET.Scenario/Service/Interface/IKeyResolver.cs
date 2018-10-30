using Rogue.NET.Common.EventArgs;
using Rogue.NET.Core.Model.Enums;
using System.Windows.Input;

namespace Rogue.NET.Scenario.Service.Interface
{
    public interface IKeyResolver
    {
        Compass ResolveDirectionKey(Key key);
        Compass ResolveDirectionArrow(Key key);
        LevelCommandEventArgs ResolveKeys(Key k, bool shift, bool ctrl, bool alt);
    }
}

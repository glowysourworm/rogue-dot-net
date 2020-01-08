using Rogue.NET.Core.Model.Scenario.Content.Layout;

using System.Windows.Media;

namespace Rogue.NET.Core.Media.SymbolEffect.Interface
{
    public interface ISymbolEffectFilter
    {
        void ApplyEffect(DrawingGroup drawing, SymbolEffectBase effect);
    }
}

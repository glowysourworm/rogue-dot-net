using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Rogue.NET.Core.Media.SymbolEffect
{
    public abstract class SymbolEffectBase
    {
        public abstract Color ApplyFilter(Color inputColor);
    }
}

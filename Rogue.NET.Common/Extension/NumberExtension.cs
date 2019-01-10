using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Common.Extension
{
    public static class NumberExtension
    {
        public static string Sign(this double number)
        {
            return number >= 0 ? "+" : "-";
        }
    }
}

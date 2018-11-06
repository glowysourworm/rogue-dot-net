using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.ScenarioEditor.Utility
{
    public static class NameGenerator
    {
        public static string Get(IEnumerable<string> names, string prefix)
        {
            var ctr = 1;
            var name = prefix;
            while (names.Contains(prefix))
                prefix = name + " (" + ctr++.ToString() + ")";

            return prefix;
        }
    }
}

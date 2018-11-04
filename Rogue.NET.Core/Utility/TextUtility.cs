using System.Text.RegularExpressions;

namespace Rogue.NET.Core.Utility
{
    public static class TextUtility
    {
        public static string CamelCaseToTitleCase(string str)
        {
            return Regex.Replace(str, "(\\B[A-Z])", " $1");
        }
    }
}

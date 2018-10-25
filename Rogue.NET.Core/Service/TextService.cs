using Rogue.NET.Core.Service.Interface;

using System.ComponentModel.Composition;
using System.Text.RegularExpressions;

namespace Rogue.NET.Core.Service
{
    [Export(typeof(ITextService))]
    public class TextService : ITextService
    {
        public string CamelCaseToTitleCase(string str)
        {
            return Regex.Replace(str, "(\\B[A-Z])", " $1");
        }
    }
}

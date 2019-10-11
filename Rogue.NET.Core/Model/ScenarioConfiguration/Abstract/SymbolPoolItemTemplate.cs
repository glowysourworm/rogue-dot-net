using Rogue.NET.Core.Model.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Abstract
{
    [Serializable]
    public class SymbolPoolItemTemplate : Template
    {
        string _symbolPoolCategory;
        List<SymbolDetailsTemplate> _symbols;

        public string SymbolPoolCategory
        {
            get { return _symbolPoolCategory; }
            set { this.RaiseAndSetIfChanged(ref _symbolPoolCategory, value); }
        }
        public List<SymbolDetailsTemplate> Symbols
        {
            get { return _symbols; }
            set { this.RaiseAndSetIfChanged(ref _symbols, value); }
        }

        public SymbolPoolItemTemplate()
        {
            this.Symbols = new List<SymbolDetailsTemplate>();
        }
    }
}

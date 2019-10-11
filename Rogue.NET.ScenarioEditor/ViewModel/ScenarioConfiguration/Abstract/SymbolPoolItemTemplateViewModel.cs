using Rogue.NET.Core.Model.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract
{
    public class SymbolPoolItemTemplateViewModel : TemplateViewModel
    {
        string _symbolPoolCategory;
        ObservableCollection<SymbolDetailsTemplateViewModel> _symbols;

        public string SymbolPoolCategory
        {
            get { return _symbolPoolCategory; }
            set { this.RaiseAndSetIfChanged(ref _symbolPoolCategory, value); }
        }
        public ObservableCollection<SymbolDetailsTemplateViewModel> Symbols
        {
            get { return _symbols; }
            set { this.RaiseAndSetIfChanged(ref _symbols, value); }
        }

        public SymbolPoolItemTemplateViewModel()
        {
            this.Symbols = new ObservableCollection<SymbolDetailsTemplateViewModel>();
        }
    }
}

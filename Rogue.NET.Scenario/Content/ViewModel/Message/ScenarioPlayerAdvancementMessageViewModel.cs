using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Scenario.Content.ViewModel.Message
{
    public class ScenarioPlayerAdvancementMessageViewModel : ScenarioMessageViewModel
    {
        public ObservableCollection<AttributeChangeViewModel> AttributeChanges { get; set; }
    }
}

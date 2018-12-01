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
        string _playerName;
        int _playerLevel;

        public string PlayerName
        {
            get { return _playerName; }
            set { this.RaiseAndSetIfChanged(ref _playerName, value); }
        }
        public int PlayerLevel
        {
            get { return _playerLevel; }
            set { this.RaiseAndSetIfChanged(ref _playerLevel, value); }
        }

        public ObservableCollection<AttributeChangeViewModel> AttributeChanges { get; set; }

        public ScenarioPlayerAdvancementMessageViewModel()
        {
            this.AttributeChanges = new ObservableCollection<AttributeChangeViewModel>();
        }
    }
}

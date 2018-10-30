using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Rogue.NET.Core.Graveyard
{
    public class LevelData : INotifyPropertyChanged
    {
        public int Seed { get; set; }
        public Level Level { get; set; }
        public Player Player { get; set; }
        public PlayerStartLocation StartLocation { get; set; }
        public ScenarioConfigurationContainer Config { get; set; }
        public ObservableCollection<DialogMessage> DialogMessages { get; set; }
        public Dictionary<string, ScenarioMetaData> Encyclopedia { get; set; }
        public ObservableCollection<Enemy> TargetedEnemies { get; set; }
        public string ObjectiveDescription { get; set; }

        string _displayTitle = "";
        public string DisplayTitle
        {
            get { return _displayTitle; }
            set
            {
                _displayTitle = value;
                OnPropertyChanged("DisplayTitle");
            }
        }

        public class DialogMessage
        {
            public string Message { get; set; }
            public DateTime Timestamp { get; set; }
        }

        /// <summary>
        /// Create objects used during game play but not serialized
        /// </summary>
        public LevelData()
        {
            this.DialogMessages = new ObservableCollection<DialogMessage>();
            this.TargetedEnemies = new ObservableCollection<Enemy>();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}

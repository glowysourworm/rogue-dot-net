using Rogue.NET.Common;
using Rogue.NET.Common.Collections;
using Rogue.NET.Model.Scenario;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Model
{
    public class LevelData : INotifyPropertyChanged
    {
        public int Seed { get; set; }
        public Level Level { get; set; }
        public Player Player { get; set; }
        public PlayerStartLocation StartLocation { get; set; }
        public ScenarioConfiguration Config { get; set; }
        public SerializableObservableCollection<DialogMessage> DialogMessages { get; set; }
        public SerializableObservableCollection<Consumable> ShopConsumables { get; set; }
        public SerializableObservableCollection<Equipment> ShopEquipment { get; set; }
        public SerializableDictionary<string, ScenarioMetaData> Encyclopedia { get; set; }
        public SerializableObservableCollection<Enemy> TargetedEnemies { get; set; }
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
            this.DialogMessages = new SerializableObservableCollection<DialogMessage>();
            this.TargetedEnemies = new SerializableObservableCollection<Enemy>();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}

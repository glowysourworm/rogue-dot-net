using System;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Data;
using Rogue.NET.Common;
using Rogue.NET.Common.Collections;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Rogue.NET.Model.Scenario
{
    public class ScenarioContainer
    {
        public List<Level> LoadedLevels { get; set; }
        public SerializableObservableCollection<Equipment> ShopEquipment { get; set; }
        public SerializableObservableCollection<Consumable> ShopConsumables { get; set; }
        public Player Player1 { get; set; }
        public int Seed { get; set; }
        public int CurrentLevel { get; set; }
        public int TotalTicks { get; set; }
        public bool SurvivorMode { get; set; }
        public bool ObjectiveAcheived { get; set; }
        public ScenarioConfiguration StoredConfig { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime CompletedTime { get; set; }

        //Store Item Metadata
        public SerializableDictionary<string, ScenarioMetaData> ItemEncyclopedia { get; set; }

        public ScenarioContainer()
        {
            this.Player1 = new Player();
            this.LoadedLevels = new List<Level>();
            this.CurrentLevel = 1;
            this.ShopConsumables = new SerializableObservableCollection<Consumable>();
            this.ShopEquipment = new SerializableObservableCollection<Equipment>();
            this.ItemEncyclopedia = new SerializableDictionary<string, ScenarioMetaData>();
            this.ObjectiveAcheived = false;
        }
    }
}


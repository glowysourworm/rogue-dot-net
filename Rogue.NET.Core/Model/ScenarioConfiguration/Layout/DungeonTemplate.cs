using ProtoBuf;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Layout
{
    [Serializable]
    [ProtoContract(AsReferenceDefault = true)]
    public class DungeonTemplate : Template
    {
        private int _numberOfLevels;
        private double _monsterGenerationBase;
        private double _partyRoomGenerationRate;
        private string _objectiveDescription;

        [ProtoMember(1)]
        public int NumberOfLevels
        {
            get { return _numberOfLevels; }
            set
            {
                if (_numberOfLevels != value)
                {
                    _numberOfLevels = value;
                    OnPropertyChanged("NumberOfLevels");
                }
            }
        }
        [ProtoMember(2)]
        public double MonsterGenerationBase
        {
            get { return _monsterGenerationBase; }
            set
            {
                if (_monsterGenerationBase != value)
                {
                    _monsterGenerationBase = value;
                    OnPropertyChanged("MonsterGenerationBase");
                }
            }
        }
        [ProtoMember(3)]
        public double PartyRoomGenerationRate
        {
            get { return _partyRoomGenerationRate; }
            set
            {
                if (_partyRoomGenerationRate != value)
                {
                    _partyRoomGenerationRate = value;
                    OnPropertyChanged("PartyRoomGenerationRate");
                }
            }
        }
        [ProtoMember(4)]
        public string ObjectiveDescription
        {
            get { return _objectiveDescription; }
            set
            {
                if (_objectiveDescription != value)
                {
                    _objectiveDescription = value;
                    OnPropertyChanged("ObjectiveDescription");
                }
            }
        }

        [ProtoMember(5, AsReference = true)]
        public List<LayoutTemplate> LayoutTemplates { get; set; }

        public DungeonTemplate()
        {
            this.LayoutTemplates = new List<LayoutTemplate>();

            this.NumberOfLevels = 100;
            this.MonsterGenerationBase = 0.01;
            this.PartyRoomGenerationRate = 0.1;
        }
    }
}

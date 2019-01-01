using ProtoBuf;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using System;
using System.ComponentModel;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Abstract
{
    [Serializable]
    [ProtoContract(AsReferenceDefault = true, SkipConstructor = true)]
    [ProtoInclude(3, typeof(ProbabilityEquipmentTemplate))]
    [ProtoInclude(4, typeof(ProbabilityConsumableTemplate))]
    [ProtoInclude(5, typeof(DungeonObjectTemplate))]
    [ProtoInclude(6, typeof(BehaviorTemplate))]
    [ProtoInclude(7, typeof(BehaviorDetailsTemplate))]
    [ProtoInclude(8, typeof(BrushTemplate))]
    [ProtoInclude(9, typeof(AnimationTemplate))]
    [ProtoInclude(10, typeof(AlterationEffectTemplate))]
    [ProtoInclude(11, typeof(AlterationCostTemplate))]
    [ProtoInclude(12, typeof(SymbolDetailsTemplate))]
    [ProtoInclude(13, typeof(DungeonTemplate))]
    [ProtoInclude(14, typeof(LayoutTemplate))]
    [ProtoInclude(15, typeof(GradientStopTemplate))]
    public class Template : INotifyPropertyChanged
    {
        public Template()
        {
            this.Name = "New Template";
            this.Guid = System.Guid.NewGuid().ToString();
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        private string _name;
        private string _guid;

        [ProtoMember(1)]
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged("Name");
                }
            }
        }
        [ProtoMember(2)]
        public string Guid
        {
            get { return _guid; }
            set
            {
                if (_guid != value)
                {
                    _guid = value;
                    OnPropertyChanged("Guid");
                }
            }
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return _guid.GetHashCode();
        }
    }
}

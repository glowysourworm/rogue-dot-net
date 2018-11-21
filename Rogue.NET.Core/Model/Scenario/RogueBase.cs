using System;
using System.Runtime.Serialization;

namespace Rogue.NET.Core.Model.Scenario
{
    [Serializable]
    public abstract class RogueBase
    {
        public string Id { get; protected set; }
        public string RogueName { get; set; }
        
        public RogueBase()
        {
            this.Id = Guid.NewGuid().ToString();
        }
        public RogueBase(string name)
        {
            this.Id = Guid.NewGuid().ToString();
            this.RogueName = name;
        }
        //public RogueBase(SerializationInfo info, StreamingContext context)
        //{
        //    this.Id = info.GetString("Id");
        //    this.RogueName = info.GetString("RogueName");
        //}

        //public void GetObjectData(SerializationInfo info, StreamingContext context)
        //{
        //    info.AddValue("Id", this.Id);
        //    info.AddValue("RogueName", this.RogueName);
        //}
    }
}

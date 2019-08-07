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
        public RogueBase(string id)
        {
            this.Id = id;
        }
        public RogueBase(string id, string name)
        {
            this.Id = id;
            this.RogueName = name;
        }
    }
}

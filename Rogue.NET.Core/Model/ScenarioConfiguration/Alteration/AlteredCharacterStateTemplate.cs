using ProtoBuf;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Alteration
{
    [Serializable]
    [ProtoContract(AsReferenceDefault = true)]
    public class AlteredCharacterStateTemplate : DungeonObjectTemplate
    {
        [ProtoMember(1)]
        public CharacterStateType BaseType { get; set; }

        public AlteredCharacterStateTemplate()
        {
            this.BaseType = CharacterStateType.Normal;
        }
    }
}

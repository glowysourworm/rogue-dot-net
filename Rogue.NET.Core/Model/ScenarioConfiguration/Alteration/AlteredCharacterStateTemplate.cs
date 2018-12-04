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
    public class AlteredCharacterStateTemplate : DungeonObjectTemplate
    {
        public CharacterStateType BaseType { get; set; }

        public AlteredCharacterStateTemplate()
        {
            this.BaseType = CharacterStateType.Normal;
        }
    }
}

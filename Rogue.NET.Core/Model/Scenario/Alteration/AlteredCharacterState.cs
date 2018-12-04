using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Model.Scenario.Alteration
{
    [Serializable]
    public class AlteredCharacterState : ScenarioImage
    {
        public CharacterStateType BaseType { get; set; }

        public AlteredCharacterState()
        {
            this.BaseType = CharacterStateType.Normal;
        }
    }
}

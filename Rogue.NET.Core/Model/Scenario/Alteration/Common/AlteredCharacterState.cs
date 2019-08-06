using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content;
using System;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Common
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

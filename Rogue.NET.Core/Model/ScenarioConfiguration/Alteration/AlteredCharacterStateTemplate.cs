using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System;

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

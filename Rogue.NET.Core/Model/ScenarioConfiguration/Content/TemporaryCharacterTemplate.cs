using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Content
{
    [Serializable]
    public class TemporaryCharacterTemplate : NonPlayerCharacterTemplate
    {
        // NOTE*** The purpose of a temporary character is to be created in 
        //         the level with some specified life time. So, there's no
        //         lifetime counter here; but there will be alteration effects
        //         that have one.
        //

        public TemporaryCharacterTemplate()
        {
        }
    }
}

using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content;

namespace Rogue.NET.Core.Logic.Interface
{
    public interface IReligionEngine : IRogueEngine
    {
        LevelContinuationAction RenounceReligion(bool forceRenunciation);
        void Affiliate(Religion religion);
        void IdentifyReligion(Religion religionName);
    }
}

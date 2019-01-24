using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Religion;

namespace Rogue.NET.Core.Logic.Interface
{
    public interface IReligionEngine : IRogueEngine
    {
        LevelContinuationAction RenounceReligion(bool forceRenunciation);
        void Affiliate(string religionName, double affiliationLevel);
        void IdentifyReligion(string religionName);
    }
}

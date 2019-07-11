using Rogue.NET.Core.Model.Enums;

namespace Rogue.NET.Core.Logic.Interface
{
    public interface IReligionEngine : IRogueEngine
    {
        LevelContinuationAction RenounceReligion(bool forceRenunciation);
        void Affiliate(string religionName);
        void IdentifyReligion(string religionName);
    }
}

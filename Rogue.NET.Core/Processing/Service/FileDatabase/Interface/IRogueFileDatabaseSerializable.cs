using Rogue.NET.Core.Processing.Service.Interface;

namespace Rogue.NET.Core.Processing.Service.FileDatabase.Interface
{
    /// <summary>
    /// Interface to work with RogueFileDatabase - mark classes with this interface to implement
    /// store / retrieve methods for individually serialized pieces of the object.
    /// </summary>
    public interface IRogueFileDatabaseSerializable
    {
        /// <summary>
        /// Injects the serializer to store records for the entry
        /// </summary>
        public void SaveRecords(IRogueFileDatabaseSerializer serializer);
    }
}

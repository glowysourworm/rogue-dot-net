namespace Rogue.NET.Core.Logic.Processing.Interface
{
    /// <summary>
    /// Specification for continued model processing. This is intended to live on a queue and
    /// be processed in appropriate order to continue work on the model. Example: Process N enemy
    /// reactions after an amination is played. There would be N ILevelProcessingAction objects
    /// on the queue.
    /// </summary>
    public interface ILevelProcessingAction
    {
        LevelProcessingActionType Type { get; set; }
        string CharacterId { get; set; }
    }
}


namespace Rogue.NET.Common.Extension.Prism.EventAggregator
{
    public interface IRogueEventAggregator
    {
        TEventType GetEvent<TEventType>() where TEventType : RogueEventBase;
    }
}

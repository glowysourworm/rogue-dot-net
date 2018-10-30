using Prism.Events;

namespace Rogue.NET.Model.Events
{
    public class PlayerDiedEventArgs : System.EventArgs
    {
        public string PlayerName { get; set; }
        public string DiedOf { get; set; }
    }
    public class PlayerDiedEvent : PubSubEvent<PlayerDiedEventArgs>
    {

    }
}

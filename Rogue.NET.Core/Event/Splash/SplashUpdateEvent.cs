using Prism.Events;

namespace Rogue.NET.Common.Events.Splash
{
    public class SplashUpdateEventArgs : System.EventArgs
    {
        /// <summary>
        /// sets loading message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Sets progress bar if there is one
        /// </summary>
        public double Progress { get; set; }
    }
    public class SplashUpdateEvent : PubSubEvent<SplashUpdateEventArgs>
    {

    }
}

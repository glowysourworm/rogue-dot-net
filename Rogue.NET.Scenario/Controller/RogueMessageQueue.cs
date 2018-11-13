using System.Collections.Generic;

namespace Rogue.NET.Scenario.Controller
{
    /// <summary>
    /// Represents a pair of input / output queues used for message exchange.
    /// </summary>
    public class RogueMessageQueue<T>
    {
        public Queue<T> InputQueue { get; private set; }
        public Queue<T> OutputQueue { get; private set; }

        public RogueMessageQueue()
        {
            this.InputQueue = new Queue<T>();
            this.OutputQueue = new Queue<T>();
        }
    }
}

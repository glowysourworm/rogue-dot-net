using System.Collections.Generic;

namespace Rogue.NET.Core.Service.Interface
{
    public interface IScenarioMessageService
    {
        /// <summary>
        /// Blocks message forwarding to the UI
        /// </summary>
        void Block();

        /// <summary>
        /// Unblocks message forwarding to the UI
        /// </summary>
        /// <param name="send">Option to send saved messages</param>
        void UnBlock(bool send);

        void Publish(string message);

        void Publish(string message, params string[] format);

        void PublishPlayerAdvancement(string header, IEnumerable<string> messages);
    }
}

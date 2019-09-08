using Rogue.NET.Core.Processing.Event.Backend.EventData.ScenarioMessage.Enum;
using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace Rogue.NET.Core.Processing.Event.Backend.EventData.ScenarioMessage
{
    public class PlayerAdvancementMessageData : ScenarioMessageData
    {
        public PlayerAdvancementMessageData(ScenarioMessagePriority priority) : base(priority)
        {
        }

        public string PlayerName { get; set; }

        public int PlayerLevel { get; set; }

        /// <summary>
        /// Set of changes to player attributes (Example: HPMax, 0.5)
        /// </summary>
        public IList<Tuple<string, double, Color>> AttributeChanges { get; set; }
    }
}

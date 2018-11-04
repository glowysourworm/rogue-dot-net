using Rogue.NET.Core.Service.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Rogue.NET.Core.Service
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IScenarioMessageService))]
    public class ScenarioMessageService : IScenarioMessageService
    {
        public void Block()
        {
        }

        public void Publish(string message)
        {
        }

        public void Publish(string message, params string[] format)
        {
        }

        public void PublishPlayerAdvancement(string header, IEnumerable<string> messages)
        {
        }

        public void UnBlock(bool send)
        {
        }
    }
}

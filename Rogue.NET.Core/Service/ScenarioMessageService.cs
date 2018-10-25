using Rogue.NET.Core.Model.Common;
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
            throw new NotImplementedException();
        }

        public void Publish(string message)
        {
            throw new NotImplementedException();
        }

        public void Publish(string message, params string[] format)
        {
            throw new NotImplementedException();
        }

        public void PublishAnimation(string spellId, string sourceCharacterId, IEnumerable<string> targetCharacterIds, AnimationReturnAction returnAction)
        {
            throw new NotImplementedException();
        }

        public void PublishPlayerAdvancement(string header, IEnumerable<string> messages)
        {
            throw new NotImplementedException();
        }

        public void UnBlock(bool send)
        {
            throw new NotImplementedException();
        }
    }
}

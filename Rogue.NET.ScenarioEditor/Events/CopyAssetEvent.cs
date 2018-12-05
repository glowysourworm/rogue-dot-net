using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.ViewModel.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.ScenarioEditor.Events
{
    public class CopyAssetEventArgs : EventArgs
    {
        public string AssetName { get; set; }
        public string AssetType { get; set; }
        public string AssetNewName { get; set; }
    }
    public class CopyAssetEvent : RogueEvent<CopyAssetEventArgs>
    {
    }
}

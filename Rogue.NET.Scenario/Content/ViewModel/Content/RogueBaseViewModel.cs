using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core.Model.Scenario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Scenario.Content.ViewModel.Content
{
    public class RogueBaseViewModel : NotifyViewModel
    {
        string _id;
        string _rogueName;

        public string Id
        {
            get { return _id; }
            private set { this.RaiseAndSetIfChanged(ref _id, value); }
        }
        public string RogueName
        {
            get { return _rogueName; }
            private set { this.RaiseAndSetIfChanged(ref _rogueName, value); }
        }

        public RogueBaseViewModel(RogueBase rogueBase)
        {
            this.Id = rogueBase.Id;
            this.RogueName = rogueBase.RogueName;
        }
    }
}

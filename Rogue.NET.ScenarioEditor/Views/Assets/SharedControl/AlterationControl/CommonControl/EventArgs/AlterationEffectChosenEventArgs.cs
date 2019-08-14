using System;

namespace Rogue.NET.ScenarioEditor.Views.Assets.SharedControl.AlterationControl.CommonControl.EventArgs
{
    public class AlterationEffectChosenEventArgs : System.EventArgs
    {
        public Type AlterationEffectType { get; set; }
        public Type AlterationEffectViewType { get; set; }
    }
}

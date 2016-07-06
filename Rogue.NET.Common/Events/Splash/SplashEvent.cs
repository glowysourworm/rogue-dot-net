using Microsoft.Practices.Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Common.Events.Splash
{
    public class SplashEvent : CompositePresentationEvent<SplashEvent>
    {
        public SplashAction SplashAction { get; set; }
        public SplashEventType SplashType { get; set; }
    }

    public enum SplashEventType
    {
        Splash,
        NewScenario,
        CommandPreferences,
        Help,
        Objective,
        Save,
        Open,
        Identify,
        Uncurse,
        EnchantArmor,
        EnchantWeapon,
        Imbue,
        Dialog
        //...
    }

    public enum SplashAction
    {
        Show,
        Hide
    }
}

using Prism.Events;

namespace Rogue.NET.Common.Events.Splash
{
    public class SplashEventArgs : System.EventArgs
    {
        public SplashAction SplashAction { get; set; }
        public SplashEventType SplashType { get; set; }
    }

    public class SplashEvent : PubSubEvent<SplashEventArgs>
    {

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

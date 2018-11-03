using Rogue.NET.Common.Utility;
using Rogue.NET.PrismExtension;
using System;
using System.Windows;
using System.Windows.Media;

namespace Rogue.NET
{
    public partial class RogueApplication : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var centaur = new FontFamily(new Uri("pack://application:,,,/Rogue.NET.Common;Component/Resource/Fonts/CENTAUR.TTF#Centaur"), "Centaur");
            var ilShakeFest = new FontFamily(new Uri("pack://application:,,,/Rogue.NET.Common;Component/Resource/Fonts/ILShakeFest.ttf#ILShakeFest"), "ILShakeFest");
            var fontAwesome = new FontFamily(new Uri("pack://application:,,,/Rogue.NET.Common;Component/Resource/Fonts/fontawesome-webfont.ttf#FontAwesome"), "FontAwesome");

            this.Resources.Add("CentaurFont", centaur);
            this.Resources.Add("ILShakeFestFont", ilShakeFest);
            this.Resources.Add("FontAwesome", fontAwesome);

            var bootstrapper = new RogueBootstrapper();
            bootstrapper.Run();
        }
        protected override void OnExit(ExitEventArgs e)
        {
            Taskbar.Show();

            base.OnExit(e);
        }
    }
}

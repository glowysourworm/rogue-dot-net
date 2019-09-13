using Rogue.NET.Common.Utility;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.PrismExtension;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Rogue.NET
{
    public partial class RogueApplication : Application
    {
        public RogueApplication()
        {
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var centaur = new FontFamily(new Uri("pack://application:,,,/Rogue.NET.Common;Component/Resource/Fonts/CENTAUR.TTF#Centaur"), "Centaur");
            var ilShakeFest = new FontFamily(new Uri("pack://application:,,,/Rogue.NET.Common;Component/Resource/Fonts/ILShakeFest.ttf#ILShakeFest"), "ILShakeFest");
            var fontAwesome = new FontFamily(new Uri("pack://application:,,,/Rogue.NET.Common;Component/Resource/Fonts/fontawesome-webfont.ttf#FontAwesome"), "FontAwesome");
            var theme = new ResourceDictionary();

            theme.Source = new Uri("pack://application:,,,/Rogue.NET;component/Themes/ExpressionDark.xaml");

            this.Resources.Add("CentaurFont", centaur);
            this.Resources.Add("ILShakeFestFont", ilShakeFest);
            this.Resources.Add("FontAwesome", fontAwesome);

            this.Resources.MergedDictionaries.Add(theme);

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

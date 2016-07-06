using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Rogue.NET
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Taskbar.Hide();

            this.DispatcherUnhandledException += (obj, ev) => { this.Shutdown(); };

            // The boostrapper will create the Shell instance, so the App.xaml does not have a StartupUri.
            Bootstrapper bootstrapper = new Bootstrapper();
            bootstrapper.Run();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Taskbar.Show();

            base.OnExit(e);
        }
    }
}

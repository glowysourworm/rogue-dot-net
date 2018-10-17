using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Rogue.NET.Unity;

namespace Rogue.NET
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Taskbar.Hide();

            Application.Current.DispatcherUnhandledException += (obj, ev) => { Taskbar.Show(); MessageBox.Show(ev.Exception.Message); };
            AppDomain.CurrentDomain.UnhandledException += (obj, ev) => { Taskbar.Show(); MessageBox.Show(ev.ExceptionObject.ToString()); };
            AppDomain.CurrentDomain.ProcessExit += (obj, ev) => { Taskbar.Show(); };

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

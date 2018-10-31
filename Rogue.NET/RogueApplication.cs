using Rogue.NET.Common.Utility;
using Rogue.NET.PrismExtension;
using System;
using System.Windows;
using System.Windows.Media;

namespace Rogue.NET
{
    public class RogueApplication : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //Taskbar.Hide();

            //Application.Current.DispatcherUnhandledException += (obj, ev) => { Taskbar.Show(); MessageBox.Show(ev.Exception.Message); };
            //AppDomain.CurrentDomain.UnhandledException += (obj, ev) => { Taskbar.Show(); MessageBox.Show(ev.ExceptionObject.ToString()); };
            //AppDomain.CurrentDomain.ProcessExit += (obj, ev) => { Taskbar.Show(); };

            var fontFamilyCentaur = new FontFamily(new Uri(@"pack://application:,,,/Rogue.NET.Common;Component/Resource/Fonts/CENTAUR.TTF#Centaur"), "Centaur");
            var fontFamilyILShakeFest = new FontFamily(new Uri(@"pack://application:,,,/Rogue.NET.Common;Component/Resource/Fonts/ILShakeFest.ttf#ILShakeFest"), "ILShakeFest");
            var fontFamilyFontAwesome = new FontFamily(new Uri(@"pack://application:,,,/Rogue.NET.Common;Component/Resource/Fonts/fontawesome-webfont.ttf#FontAwesome"), "FontAwesome");

            var themeExpressionDark = new ResourceDictionary();
            themeExpressionDark.Source = new Uri("pack://application:,,,/Rogue.NET;Component/Themes/ExpressionDark.xaml");

            this.Resources.Add("CentaurFont", fontFamilyCentaur);
            this.Resources.Add("ILShakeFest", fontFamilyILShakeFest);
            this.Resources.Add("FontAwesome", fontFamilyFontAwesome);

            this.Resources.MergedDictionaries.Add(themeExpressionDark);

            var bootstrapper = new RogueBootstrapper();
            bootstrapper.Run();
        }
        protected override void OnExit(ExitEventArgs e)
        {
            Taskbar.Show();

            base.OnExit(e);
        }

        //protected override IContainerExtension CreateContainerExtension()
        //{
        //    return null;
        //}

        //protected override void RegisterTypes(IContainerRegistry containerRegistry)
        //{
        //    containerRegistry.Register<Shell>();
        //}

        //protected override Window CreateShell()
        //{
        //    return this.Container.Resolve<Shell>();
        //}

        //protected override void InitializeModules()
        //{
        //    base.InitializeModules();

        //    // initialize startup display
        //    //_eventAggregator.GetEvent<SplashEvent>().Publish(new SplashEventArgs()
        //    //{
        //    //    SplashAction = SplashAction.Hide,
        //    //    SplashType = SplashEventType.Splash
        //    //});
        //    //_regionManager.RequestNavigate("MainRegion", "IntroView");
        //    //Application.Current.MainWindow.Show();
        //}

        //protected override AggregateCatalog CreateAggregateCatalog()
        //{
        //    var catalogs = new AssemblyCatalog[]
        //    {
        //        new AssemblyCatalog(Assembly.GetAssembly(typeof(RogueModule))),
        //        new AssemblyCatalog(Assembly.GetAssembly(typeof(ScenarioEditorModule))),
        //        new AssemblyCatalog(Assembly.GetAssembly(typeof(ScenarioModule))),
        //        new AssemblyCatalog(Assembly.GetAssembly(typeof(CoreModule))),
        //        new AssemblyCatalog(Assembly.GetAssembly(typeof(NotifyViewModel))) // Common
        //    };
        //    return new AggregateCatalog(catalogs);
        //}

        //protected override RegionAdapterMappings ConfigureRegionAdapterMappings()
        //{
        //    var mappings = base.ConfigureRegionAdapterMappings();

        //    mappings.RegisterMapping(typeof(Border), this.Container.GetExportedValue<BorderRegionAdapter>());
        //    mappings.RegisterMapping(typeof(TransitionPresenter), this.Container.GetExportedValue<TransitionPresenterRegionAdapater>());

        //    return mappings;
        //}
    }
}

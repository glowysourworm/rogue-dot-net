using Prism.Events;
using Prism.Mef.Modularity;
using Prism.Modularity;
using Prism.Regions;
using Rogue.NET.Core.Event.Splash;
using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Splash;
using Rogue.NET.Splash.ViewModel;
using Rogue.NET.View;
using Rogue.NET.ViewModel;
using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Media;

namespace Rogue.NET
{
    [ModuleExport("Rogue", typeof(RogueModule))]
    public class RogueModule : IModule
    {
        readonly IEventAggregator _eventAggregator;
        readonly IRegionManager _regionManager;

        Window _splashWindow;

        [ImportingConstructor]
        public RogueModule(IEventAggregator eventAggregator, IRegionManager regionManager)
        {
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            _eventAggregator.GetEvent<SplashEvent>().Subscribe((e) =>
            {
                if (e.SplashAction == SplashAction.Hide)
                    HideSplash();
                else
                    ShowSplash(e.SplashType);
            });
        }

        private void HideSplash()
        {
            if (_splashWindow != null)
            {
                _splashWindow.Close();
                _splashWindow = null;
            }
        }

        private void ShowSplash(SplashEventType type)
        {
            if (_splashWindow != null)
                HideSplash();

            _splashWindow = CreateSplashWindow(_eventAggregator, type);
            _splashWindow.Show();
        }

        private Window CreateSplashWindow(IEventAggregator eventAggregator, SplashEventType splashEventType)
        {
            var splashWindow = new Window()
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                SizeToContent = SizeToContent.WidthAndHeight,
                AllowsTransparency = true,
                WindowStyle = WindowStyle.None,
                ResizeMode = ResizeMode.NoResize,
                Background = Brushes.Transparent,
                BorderBrush = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                Margin = new Thickness(0),
                FontFamily = new FontFamily(new Uri(@"pack://application:,,,/Rogue.NET.Common;Component/Resource/Fonts/CENTAUR.TTF#Centaur"), "Centaur"),
                Topmost = true
            };

            // Passing these in to take care of dependency injection. Another way is to create
            // multiple region managers and have a separate Shell window.
            switch (splashEventType)
            {
                case SplashEventType.Splash:
                    splashWindow.Content = new SplashView(new SplashViewModel(eventAggregator));
                    break;
                case SplashEventType.NewScenario:
                    splashWindow.Content = new CreatingScenarioView(new CreatingScenarioViewModel(eventAggregator));
                    break;
                case SplashEventType.CommandPreferences:
                    splashWindow.Content = new CommandPreferencesView();
                    break;
                case SplashEventType.Help:
                    splashWindow.Content = new HelpView();
                    break;
                case SplashEventType.Objective:
                    splashWindow.Content = new ObjectiveView(eventAggregator);
                    break;
                case SplashEventType.Save:
                    splashWindow.Content = new SaveView();
                    break;
                case SplashEventType.Open:
                    splashWindow.Content = new OpenScenarioView();
                    break;
                case SplashEventType.Identify:
                    splashWindow.Content = new IdentifyView(eventAggregator);
                    break;
                case SplashEventType.Uncurse:
                    splashWindow.Content = new UncurseView(eventAggregator);
                    break;
                case SplashEventType.EnchantArmor:
                    splashWindow.Content = new EnchantView(eventAggregator);
                    break;
                case SplashEventType.EnchantWeapon:
                    splashWindow.Content = new EnchantView(eventAggregator);
                    break;
                case SplashEventType.Imbue:
                    splashWindow.Content = new ImbueView(eventAggregator);
                    break;
                case SplashEventType.Dialog:
                    splashWindow.Content = new DialogView();
                    break;
                default:
                    throw new Exception("Unknwon Splash View Type");
            }

            return splashWindow;
        }
    }
}

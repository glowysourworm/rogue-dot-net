using Prism;
using Prism.Events;
using Rogue.NET.Common.Events.Scenario;
using Rogue.NET.Common.Events.Splash;
using Rogue.NET.Model;
using Rogue.NET.Splash.Views;
using System.ComponentModel;
using System.Windows;

namespace Rogue.NET.Splash
{
    public class SplashController
    {
        readonly IEventAggregator _eventAggregator;

        public SplashController(
            IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            Initialize();
        }

        private void Initialize()
        {
            
            _eventAggregator.GetEvent<SplashEvent>().Subscribe((e) =>
            {
                switch (e.SplashAction)
                {
                    case SplashAction.Hide:
                        // handled inside splash thread
                        break;
                    case SplashAction.Show:
                        {
                            switch (e.SplashType)
                            {
                                case SplashEventType.Splash:
                                    ShowSplash();
                                    break;
                                case SplashEventType.NewScenario:
                                    ShowScenarioGeneration();
                                    break;
                                case SplashEventType.CommandPreferences:
                                    ShowCommandPreferences();
                                    break;
                                case SplashEventType.Help:
                                    ShowHelp();
                                    break;
                                case SplashEventType.Objective:
                                    ShowObjective();
                                    break;
                                case SplashEventType.Save:
                                    ShowSave();
                                    break;
                                case SplashEventType.Open:
                                    ShowOpen();
                                    break;
                                case SplashEventType.EnchantArmor:
                                    ShowEnchant();
                                    break;
                                case SplashEventType.EnchantWeapon:
                                    ShowEnchant();
                                    break;
                                case SplashEventType.Identify:
                                    ShowIdentify();
                                    break;
                                case SplashEventType.Imbue:
                                    ShowImbue();
                                    break;
                                case SplashEventType.Uncurse:
                                    ShowUncurse();
                                    break;
                                case SplashEventType.Dialog:
                                    ShowDialog();
                                    break;
                            }
                        }
                        break;
                }
            }, true);
        }

        private void ShowMessageBox(string message)
        {
            MessageBox.Show(message);
        }
        private void ShowSplash()
        {
            var thread = _unityContainer.Resolve<SplashThread<SplashWindow>>();
            thread.Start();
        }

        private void ShowScenarioGeneration()
        {
            var thread = _unityContainer.Resolve<SplashThread<CreatingScenarioWindow>>();
            thread.Start();
        }

        private void ShowCommandPreferences()
        {
            var window = new CommandPreferencesWindow();
            window.ShowDialog();

            _eventAggregator.GetEvent<CommandPreferencesChangedEvent>().Publish(new CommandPreferencesChangedEvent());
        }

        private void ShowHelp()
        {
            var window = new HelpWindow();
            window.ShowDialog();
        }

        private void ShowObjective()
        {
            var window = new ObjectiveWindow();
            window.DataContext = _unityContainer.Resolve<LevelData>();
            window.ShowDialog();
        }

        private void ShowSave()
        {
            var thread = _unityContainer.Resolve<SplashThread<SaveWindow>>();
            thread.Start();
        }

        private void ShowOpen()
        {
            var thread = _unityContainer.Resolve<SplashThread<OpenScenarioWindow>>();
            thread.Start();
        }
        private void ShowIdentify()
        {
            var window = _unityContainer.Resolve<IdentifyWindow>();
            window.DataContext = _unityContainer.Resolve<LevelData>();
            window.ShowDialog();
            window.Close();
        }
        private void ShowEnchant()
        {
            var window = _unityContainer.Resolve<EnchantWindow>();
            window.DataContext = _unityContainer.Resolve<LevelData>();
            window.ShowDialog();
            window.Close();
        }
        private void ShowUncurse()
        {
            var window = _unityContainer.Resolve<UncurseWindow>();
            window.DataContext = _unityContainer.Resolve<LevelData>();
            window.ShowDialog();
            window.Close();
        }
        private void ShowImbue()
        {
            var window = _unityContainer.Resolve<ImbueWindow>();
            window.DataContext = _unityContainer.Resolve<LevelData>();
            window.ShowDialog();
            window.Close();
        }
        private void ShowDialog()
        {
            var window = _unityContainer.Resolve<DialogWindow>();
            window.DataContext = _unityContainer.Resolve<LevelData>();
            window.ShowDialog();
            window.Close();
        }
    }
}

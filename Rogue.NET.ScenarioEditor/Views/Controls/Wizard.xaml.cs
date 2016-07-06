using Microsoft.Practices.Unity;
using Rogue.NET.ScenarioEditor.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Rogue.NET.ScenarioEditor.Views.Controls
{
    public interface IWizardPage
    {
        Type NextPage { get; }

        void Inject(IWizardViewModel containerViewModel, object model);
    }
    public class WizardNextPageEventArgs : EventArgs
    {
        public IWizardPage NextPage { get; set; }
    }
    public partial class Wizard : UserControl
    {
        readonly IUnityContainer _unityContainer;

        DoubleAnimation _slideAnimation;

        Stack<IWizardPage> _pages;

        public Wizard(
            IUnityContainer unityContainer,
            IWizardViewModel viewModel)
        {
            InitializeComponent();

            _unityContainer = unityContainer;
            _pages = new Stack<IWizardPage>();

            this.DataContext = viewModel;

            if (viewModel.FirstPageType == null)
                throw new InvalidOperationException("The type of the first page for this Wizard control is not specified");

            viewModel.WizardPageChangeEvent += ((obj, e) =>
            {
                var page = this.ContentGrid.Children[0] as IWizardPage;
                switch (e.Result)
                {
                    case MessageBoxResult.Yes:
                        _pages.Push(page as IWizardPage);
                        ChangePage(page.NextPage, viewModel.Payload, false);
                        break;
                    case MessageBoxResult.No:
                        if (_pages.Count > 0)
                            ChangePage(_pages.Pop().GetType(), viewModel.Payload, true);
                        break;
                    case MessageBoxResult.Cancel:
                        ChangePage(viewModel.FirstPageType, viewModel.Payload, true);
                        break;
                }
            });

            ChangePage(viewModel.FirstPageType, viewModel.Payload, false);
        }

        private void ChangePage(Type nextPageType, object payload, bool backwards)
        {
            // resolve page using container
            var page = _unityContainer.Resolve(nextPageType);
            var ctrl = page as UserControl;
            var xform = new TranslateTransform(0, 0);
            var sign = backwards ? 1 : -1;
            var currentCtrl = this.ContentGrid.Children.Count > 0 ? this.ContentGrid.Children[0] as UserControl : null;

            // inject the model manually
            (page as IWizardPage).Inject(this.DataContext as IWizardViewModel, payload);

            // calculate button visibility
            if (_pages.Count == 0)
                this.BackButton.Visibility = Visibility.Collapsed;

            else
                this.BackButton.Visibility = Visibility.Visible;

            if ((page as IWizardPage).NextPage == page.GetType())
                this.NextButton.Visibility = Visibility.Collapsed;

            else
                this.NextButton.Visibility = Visibility.Visible;

            _slideAnimation = new DoubleAnimation(sign * this.ContentGrid.RenderSize.Width, new Duration(new TimeSpan(0, 0, 0, 0, 300)));

            if (currentCtrl != null)
                currentCtrl.RenderTransform = xform;

            // load the view
            _slideAnimation.Completed += (obj, e) =>
            {
                if (currentCtrl != null)
                    currentCtrl.RenderTransform = null;

                this.ContentGrid.Children.Clear();
                this.ContentGrid.Children.Add(ctrl);
                this.WizardStepsListBox.SelectedIndex = _pages.Count;
            };

            xform.BeginAnimation(TranslateTransform.XProperty, _slideAnimation);
        }

        /// <summary>
        /// Activates wizard to show first page with same bound model
        /// </summary>
        public void Reset()
        {
            _pages.Clear();
            var viewModel = this.DataContext as IWizardViewModel;
            ChangePage(viewModel.FirstPageType, viewModel.Payload, true);
        }
    }
}
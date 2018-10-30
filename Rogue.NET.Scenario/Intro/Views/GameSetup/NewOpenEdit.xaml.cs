using Prism.Events;
using Rogue.NET.Common.Events;
using Rogue.NET.Common.Events.Scenario;
using Rogue.NET.Scenario.Events;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Effects;

namespace Rogue.NET.Scenario.Intro.Views.GameSetup
{
    public partial class NewOpenEdit : UserControl
    {
        readonly IEventAggregator _eventAggregator;

        public NewOpenEdit()
        {
            InitializeComponent();
        }
        [ImportingConstructor]
        public NewOpenEdit(IEventAggregator eventAggregator)
        {
            InitializeComponent();

            _eventAggregator = eventAggregator;

            /* Mouse Enter */
            this.NewStack.MouseEnter += (obj, e) =>
            {
                UnblurElement(this.NewStack);
                BlurElement(this.OpenStack);
                BlurElement(this.EditStack);
                BlurElement(this.ExitStack);
            };
            this.OpenStack.MouseEnter += (obj, e) =>
            {
                BlurElement(this.NewStack);
                UnblurElement(this.OpenStack);
                BlurElement(this.EditStack);
                BlurElement(this.ExitStack);
            };
            this.EditStack.MouseEnter += (obj, e) =>
            {
                BlurElement(this.NewStack);
                BlurElement(this.OpenStack);
                UnblurElement(this.EditStack);
                BlurElement(this.ExitStack);
            };
            this.ExitStack.MouseEnter += (obj, e) =>
            {
                BlurElement(this.NewStack);
                BlurElement(this.OpenStack);
                BlurElement(this.EditStack);
                UnblurElement(this.ExitStack);
            };

            /* Mouse Leave */
            this.NewStack.MouseLeave += (obj, e) =>
            {
                UnblurElement(this.NewStack);
                UnblurElement(this.OpenStack);
                UnblurElement(this.EditStack);
                UnblurElement(this.ExitStack);
            };
            this.OpenStack.MouseLeave += (obj, e) =>
            {
                UnblurElement(this.NewStack);
                UnblurElement(this.OpenStack);
                UnblurElement(this.EditStack);
                UnblurElement(this.ExitStack);
            };
            this.EditStack.MouseLeave += (obj, e) =>
            {
                UnblurElement(this.NewStack);
                UnblurElement(this.OpenStack);
                UnblurElement(this.EditStack);
                UnblurElement(this.ExitStack);
            };
            this.ExitStack.MouseLeave += (obj, e) =>
            {
                UnblurElement(this.NewStack);
                UnblurElement(this.OpenStack);
                UnblurElement(this.EditStack);
                UnblurElement(this.ExitStack);
            };

            /* Click Events */
            this.NewStack.MouseDown += (obj, e) =>
            {
                _eventAggregator.GetEvent<GameSetupDisplayFinished>().Publish(new GameSetupDisplayFinishedEventArgs()
                {
                    NextDisplayType = typeof(ChooseScenario)
                });
            };
            this.OpenStack.MouseDown += (obj, e) =>
            {
                _eventAggregator.GetEvent<GameSetupDisplayFinished>().Publish(new GameSetupDisplayFinishedEventArgs()
                {
                    NextDisplayType = typeof(ChooseSavedGame)
                });
            };
            this.EditStack.MouseDown += (obj, e) =>
            {
                _eventAggregator.GetEvent<EditScenarioEvent>().Publish();
            };
            this.ExitStack.MouseDown += (obj, e) =>
            {
                _eventAggregator.GetEvent<ExitEvent>().Publish();
            };
        }

        private void BlurElement(FrameworkElement element)
        {
            element.Effect = new BlurEffect() { Radius=10 };
        }
        private void UnblurElement(FrameworkElement element)
        {
            element.Effect = null;
        }
    }
}

using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.Events.Asset;
using Rogue.NET.ScenarioEditor.Utility;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.Views.Controls;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.SharedControl
{
    // NOT EXPORTED - USE MARKUP EXTENSION TO INSTANTIATE
    public partial class MetaDataControl : UserControl
    {
        public static readonly DependencyProperty HasObjectiveSettingProperty =
            DependencyProperty.Register("HasObjectiveSetting", typeof(bool), typeof(MetaDataControl), new PropertyMetadata(true));

        public static readonly DependencyProperty HasUniqueSettingProperty =
            DependencyProperty.Register("HasUniqueSetting", typeof(bool), typeof(MetaDataControl), new PropertyMetadata(true));

        public static readonly DependencyProperty HasCursedSettingProperty =
            DependencyProperty.Register("HasCursedSetting", typeof(bool), typeof(MetaDataControl), new PropertyMetadata(true));

        public bool HasObjectiveSetting
        {
            get { return (bool)GetValue(HasObjectiveSettingProperty); }
            set { SetValue(HasObjectiveSettingProperty, value); }
        }

        public bool HasUniqueSetting
        {
            get { return (bool)GetValue(HasUniqueSettingProperty); }
            set { SetValue(HasUniqueSettingProperty, value); }
        }

        public bool HasCursedSetting
        {
            get { return (bool)GetValue(HasCursedSettingProperty); }
            set { SetValue(HasCursedSettingProperty, value); }
        }

        public MetaDataControl(IRogueEventAggregator eventAggregator)
        {
            InitializeComponent();

            this.RenameButton.Click += (sender, e) =>
            {
                var viewModel = this.DataContext as TemplateViewModel;

                if (viewModel != null)
                {
                    eventAggregator.GetEvent<RenameScenarioAssetEvent>()
                                   .Publish(viewModel);
                }
            };
        }

        private void ObjectiveCB_Checked(object sender, RoutedEventArgs e)
        {
            var viewModel = this.DataContext as DungeonObjectTemplateViewModel;

            if (viewModel != null)
            {
                // SET UNIQUE FOR ANY OBJECTIVE ITEM
                viewModel.IsUnique = true;
            }
        }
    }
}

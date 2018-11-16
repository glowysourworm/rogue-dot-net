using System.Windows;
using System.Windows.Controls;
using Rogue.NET.Common.Events.Scenario;
using Rogue.NET.Common.EventArgs;
using Prism.Events;
using Rogue.NET.Core.Model.Scenario.Content.Skill;
using Rogue.NET.Core.Model.Enums;
using System.ComponentModel.Composition;
using Rogue.NET.Scenario.Content.ViewModel.Content;

namespace Rogue.NET.Scenario.Views
{
    [Export]
    public partial class SkillGrid : UserControl
    {
        readonly IEventAggregator _eventAggregator;

        [ImportingConstructor]
        public SkillGrid(IEventAggregator eventAggregator, PlayerViewModel playerViewModel)
        {
            _eventAggregator = eventAggregator;

            this.DataContext = playerViewModel;

            InitializeComponent();
        }

        private void UpButton_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < this.TheGrid.Items.Count; i++)
            {
                var s = this.TheGrid.Items[i] as SkillSet;
                if (s.Emphasis < 3 && s.RogueName == ((Button)sender).Content.ToString())
                    s.Emphasis++;
            }
        }

        private void DownButton_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < this.TheGrid.Items.Count; i++)
            {
                var s = this.TheGrid.Items[i] as SkillSet;
                if (s.Emphasis > 0 && s.RogueName == ((Button)sender).Content.ToString())
                    s.Emphasis--;
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < this.TheGrid.Items.Count; i++)
            {
                var skillSet = this.TheGrid.Items[i] as SkillSet;
                var radioButton = sender as RadioButton;

                if (radioButton.Tag.ToString() == skillSet.RogueName)
                {
                    _eventAggregator.GetEvent<UserCommandEvent>().Publish(
                        new LevelCommandEventArgs(
                                radioButton.IsChecked.Value ? 
                                    LevelAction.ActivateSkill : 
                                    LevelAction.DeactivateSkill,
                            Compass.Null,
                            skillSet.Id));
                    break;
                }
            }
        }
    }
}

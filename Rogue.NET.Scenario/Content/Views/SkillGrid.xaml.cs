using Rogue.NET.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Dotway.WPF.Effects;
using Rogue.NET.Model.Scenario;
using Rogue.NET.Model;
using System.Windows.Media.Effects;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Prism.Events;
using Rogue.NET.Common.Events.Scenario;
using Microsoft.Practices.Prism.PubSubEvents;
using Rogue.NET.Common.EventArgs;

namespace Rogue.NET.Scenario.Views
{
    public partial class SkillGrid : UserControl
    {
        IEventAggregator _eventAggregator;

        public SkillGrid()
        {
            InitializeComponent();
        }

        public void InitializeEvents(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
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
                    _eventAggregator.GetEvent<UserCommandEvent>().Publish(new UserCommandEvent()
                    {
                        LevelCommand = new LevelCommandEventArgs(
                            radioButton.IsChecked.Value ? LevelAction.ActivateSkill  : LevelAction.DeactivateSkill, 
                            Compass.Null, 
                            skillSet.Id)
                    });
                    break;
                }
            }
        }
    }
}

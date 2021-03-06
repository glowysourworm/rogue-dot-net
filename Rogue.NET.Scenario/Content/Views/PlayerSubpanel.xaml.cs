﻿using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Processing.Command.View;
using Rogue.NET.Scenario.Content.ViewModel.Content;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Rogue.NET.Scenario.Content.Views
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export]
    public partial class PlayerSubpanel : UserControl
    {
        readonly List<FrameworkElement> _ctrlList;
        readonly List<RadioButton> _radioList;

        [ImportingConstructor]
        public PlayerSubpanel(PlayerViewModel playerViewModel, IRogueEventAggregator eventAggregator)
        {
            this.DataContext = playerViewModel;

            InitializeComponent();

            _ctrlList = new List<FrameworkElement>(new FrameworkElement[]{
                this.EquipmentCtrl,
                this.ConsumablesCtrl,
                this.SkillCtrl,
                this.StatsCtrl,
                this.AlterationsCtrl
            });

            _radioList = new List<RadioButton>(new RadioButton[]{
                this.EquipmentRB,
                this.CosumablesRB,
                this.SkillsRB,
                this.StatsRB,
                this.AlterationsRB
            });

            // Show Specific Control Events
            eventAggregator.GetEvent<ViewCommand>().Subscribe((eventData) =>
            {
                switch (eventData.ViewAction)
                {
                    case ViewActionType.ShowPlayerSubpanelEquipment:
                        ShowControl(this.EquipmentCtrl);
                        break;
                    case ViewActionType.ShowPlayerSubpanelConsumables:
                        ShowControl(this.ConsumablesCtrl);
                        break;
                    case ViewActionType.ShowPlayerSubpanelSkills:
                        ShowControl(this.SkillCtrl);
                        break;
                    case ViewActionType.ShowPlayerSubpanelStats:
                        ShowControl(this.StatsCtrl);
                        break;
                    case ViewActionType.ShowPlayerSubpanelAlterations:
                        ShowControl(this.AlterationsCtrl);
                        break;
                    default:
                        break;
                }

                return Task.Delay(1);
            });
                

            _radioList[0].IsChecked = true;
            this.TitleTB.Text = _radioList[0].Tag.ToString();
        }

        /// <summary>
        /// Performs sliding animaion on visible control then cycles to next control
        /// </summary>
        private void CycleControls(FrameworkElement visibleCtrl, FrameworkElement nextCtrl, bool right)
        {
            bool sign = _ctrlList.IndexOf(nextCtrl) > _ctrlList.IndexOf(visibleCtrl);
            if (visibleCtrl != null)
            {
                var animation = new DoubleAnimation(!right ? this.RenderSize.Width : -1 * this.RenderSize.Width, new Duration(new TimeSpan(0, 0, 0, 0, 150)));
                var transform = new TranslateTransform(0, 0);

                animation.Completed += (obj, ev) =>
                {
                    visibleCtrl.RenderTransform = null;
                    visibleCtrl.Visibility = System.Windows.Visibility.Hidden;

                    nextCtrl.Visibility = System.Windows.Visibility.Visible;
                };

                visibleCtrl.RenderTransform = transform;
                transform.ApplyAnimationClock(TranslateTransform.XProperty, animation.CreateClock());
            }
            else
                nextCtrl.Visibility = System.Windows.Visibility.Visible;

            _radioList[_ctrlList.IndexOf(nextCtrl)].IsChecked = true;

            this.TitleTB.Text = _radioList[_ctrlList.IndexOf(nextCtrl)].Tag.ToString();
        }


        private void ShowControl(FrameworkElement control)
        {
            // Current Control
            var visibleCtrl = _ctrlList.FirstOrDefault(x => x.Visibility == Visibility.Visible);

            if (visibleCtrl == control)
                return;

            // Indicies
            var index = _ctrlList.IndexOf(visibleCtrl);
            var desiredIndex = _ctrlList.IndexOf(control);

            CycleControls(visibleCtrl, control, desiredIndex > index);
        }

        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            var visibleCtrl = _ctrlList.FirstOrDefault(z => z.Visibility == System.Windows.Visibility.Visible);
            var index = _ctrlList.IndexOf(visibleCtrl);
            if (index - 1 < 0)
                CycleControls(visibleCtrl, _ctrlList.Last(), false);

            else
                CycleControls(visibleCtrl, _ctrlList[index - 1], false);
        }

        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            var visibleCtrl = _ctrlList.FirstOrDefault(z => z.Visibility == System.Windows.Visibility.Visible);
            var index = _ctrlList.IndexOf(visibleCtrl);
            if (index + 1 >= _ctrlList.Count)
                CycleControls(visibleCtrl, _ctrlList.First(), true);

            else
                CycleControls(visibleCtrl, _ctrlList[index + 1], true);
        }

        private void CycleTo(FrameworkElement control)
        {
            var visibleCtrl = _ctrlList.FirstOrDefault(z => z.Visibility == System.Windows.Visibility.Visible);
            var visibleCtrlIndex = _ctrlList.IndexOf(visibleCtrl);
            var nextCtrlIndex = _ctrlList.IndexOf(control);

            CycleControls(visibleCtrl, control, (nextCtrlIndex > visibleCtrlIndex));
        }

        private void EquipmentRB_Click(object sender, RoutedEventArgs e)
        {
            CycleTo(this.EquipmentCtrl);
        }

        private void CosumablesRB_Click(object sender, RoutedEventArgs e)
        {
            CycleTo(this.ConsumablesCtrl);
        }

        private void SkillsRB_Click(object sender, RoutedEventArgs e)
        {
            CycleTo(this.SkillCtrl);
        }

        private void StatsRB_Click(object sender, RoutedEventArgs e)
        {
            CycleTo(this.StatsCtrl);
        }

        private void AlterationsRB_Click(object sender, RoutedEventArgs e)
        {
            CycleTo(this.AlterationsCtrl);
        }
    }
}

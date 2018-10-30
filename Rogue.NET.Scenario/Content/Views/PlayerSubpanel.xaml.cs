using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Rogue.NET.Scenario.Views
{
    public partial class PlayerSubpanel : UserControl
    {
        readonly List<FrameworkElement> _ctrlList;
        readonly List<RadioButton> _radioList;

        public PlayerSubpanel()
        {
            InitializeComponent();

            _ctrlList = new List<FrameworkElement>(new FrameworkElement[]{
                this.EquipmentCtrl,
                this.ConsumablesCtrl,
                this.InventoryCtrl,
                this.SkillCtrl,
                this.StatsCtrl
            });

            _radioList = new List<RadioButton>(new RadioButton[]{
                this.EquipmentRB,
                this.CosumablesRB,
                this.InventoryRB,
                this.SkillsRB,
                this.StatsRB
            });

            _radioList[0].IsChecked = true;
            this.TitleTB.Text = _radioList[0].Tag.ToString();
        }

        public void InitializeEvents(IEventAggregator eventAggregator)
        {
            this.SkillCtrl.InitializeEvents(eventAggregator);
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
    }
}

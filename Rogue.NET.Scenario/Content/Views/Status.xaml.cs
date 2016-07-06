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
using System.Collections.Specialized;
using Rogue.NET.Common;

namespace Rogue.NET.Scenario.Views
{
    public partial class StatusCtrl : UserControl
    {
        public StatusCtrl()
        {
            InitializeComponent();

            this.HPBar.BarColor1 = Colors.Red;
            this.MPBar.BarColor1 = Colors.Blue;
            this.ExperienceBar.BarColor1 = Colors.Cyan;
            this.HaulBar.BarColor1 = Colors.Goldenrod;
            this.HungerBar.BarColor1 = Colors.DarkGreen;

            //this.ConsumablesControl.LevelActionRequest += new EventHandler<LevelCommandArgs>(OnChildLevelActionRequest);
            //this.ConsumablesControl.LevelMessageRequest += new EventHandler<LevelMessageEventArgs>(OnChildLevelMessageRequest);

            //_skillGrid.LevelActionRequest += new EventHandler<LevelCommandArgs>(OnLevelActionRequest);

            this.DataContextChanged += StatusCtrl_DataContextChanged;
        }

        void StatusCtrl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
        }
        private void OnChildLevelActionRequest(object sender, LevelCommandArgs e)
        {
            //OnLevelActionRequest(sender, e);
        }
        private void OnChildLevelMessageRequest(object sender, LevelMessageEventArgs e)
        {
            //OnLevelMessageRequest(e.Message);
        }
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            //if (this.SubPanelContainer.Child == this.ConsumablesControl)
            //{
            //    this.SubPanelContainer.Child = _skillGrid;
            //    this.SubPanelControlTB.Text = "Skills";
            //}
            //else if (this.SubPanelContainer.Child == _skillGrid)
            //{
            //    this.SubPanelContainer.Child = _statsCtrl;
            //    this.SubPanelControlTB.Text = "Stats";
            //}
            //else
            //{
            //    this.SubPanelContainer.Child = this.ConsumablesControl;
            //    this.SubPanelControlTB.Text = "Consumables";
            //}
        }
        private void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            //if (this.SubPanelContainer.Child == this.ConsumablesControl)
            //{
            //    this.SubPanelContainer.Child = _statsCtrl;
            //    this.SubPanelControlTB.Text = "Stats";
            //}
            //else if (this.SubPanelContainer.Child == _statsCtrl)
            //{
            //    this.SubPanelContainer.Child = _skillGrid;
            //    this.SubPanelControlTB.Text = "Skills";
            //}
            //else
            //{
            //    this.SubPanelContainer.Child = this.ConsumablesControl;
            //    this.SubPanelControlTB.Text = "Consumables";
            //}
        }
        /*
        #region IRogue2GameDisplay
        public event EventHandler<ModeChangeEventArgs> ModeChangeRequest;
        public event EventHandler<LevelCommandArgs> LevelActionRequest;
        public event EventHandler<LevelMessageEventArgs> LevelMessageRequest;
        protected void OnModeChangeRequest(GameCtrl.DisplayMode mode)
        {
            if (ModeChangeRequest != null)
                ModeChangeRequest(this, new ModeChangeEventArgs(mode));
        }
        protected void OnLevelActionRequest(object sender, LevelCommandArgs e)
        {
            if (LevelActionRequest != null)
                LevelActionRequest(sender, e);
        }
        protected void OnLevelMessageRequest(string msg)
        {
            if (LevelMessageRequest != null)
                LevelMessageRequest(this, new LevelMessageEventArgs(msg));
        }

        public string DisplayName
        {
            get { return "Player Status"; }
        }
        public ImageSource DisplayImageSource
        {
            get { throw new Exception(); }
        }
        public GameCtrl.DisplayMode Mode
        {
            get { return GameCtrl.DisplayMode.Null; }
        }
        #endregion
        */
    }
}

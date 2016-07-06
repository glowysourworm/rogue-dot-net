﻿using System;
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

namespace Rogue.NET.Scenario.Views
{
    /// <summary>
    /// Interaction logic for EmphasisCtrl.xaml
    /// </summary>
    public partial class Emphasis : UserControl
    {
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(int), typeof(Emphasis), new PropertyMetadata(new PropertyChangedCallback(OnEmphasisChanged)));

        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public double RedOrbHeight
        {
            get { return this.RedOrb.Width; }
            set
            {
                this.RedOrb.Width = value;
                this.PurpleOrb.Width = (value * 2) / 3;
                this.BlueOrb.Width = value / 3;
            }
        }
        public double OrbMargin
        {
            get { return this.RedOrb.Margin.Left; }
            set
            {
                this.RedOrb.Margin = new Thickness(value);
                this.BlueOrb.Margin = new Thickness(value);
                this.PurpleOrb.Margin = new Thickness(value);
            }
        }
        public Emphasis()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(EmphasisCtrl_Loaded);
        }

        private void EmphasisCtrl_Loaded(object sender, RoutedEventArgs e)
        {
            SetEmphasis();
        }
        private static void OnEmphasisChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            Emphasis ctrl = obj as Emphasis;
            ctrl.SetEmphasis();
        }

        public void SetEmphasis()
        {
            int val = this.Value;
            switch (val)
            {
                case 0:
                    this.BlueOrb.Opacity = 0.1;
                    this.PurpleOrb.Opacity = 0.1;
                    this.RedOrb.Opacity = 0.1;
                    break;
                case 1:
                    this.BlueOrb.Opacity = 1;
                    this.PurpleOrb.Opacity = 0.1;
                    this.RedOrb.Opacity = 0.1;
                    break;
                case 2:
                    this.BlueOrb.Opacity = 1;
                    this.PurpleOrb.Opacity = 1;
                    this.RedOrb.Opacity = 0.1;
                    break;
                case 3:
                    this.BlueOrb.Opacity = 1;
                    this.RedOrb.Opacity = 1;
                    this.PurpleOrb.Opacity = 1;
                    break;
            }
        }
    }
}

﻿using Prism.Events;
using Rogue.NET.ScenarioEditor.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Rogue.NET.ScenarioEditor.Views.Assets.SpellControl
{
    [Export]
    public partial class SpellEffect : UserControl
    {
        [ImportingConstructor]
        public SpellEffect(IEventAggregator eventAggregator)
        {
            InitializeComponent();

            eventAggregator.GetEvent<ScenarioLoadedEvent>().Subscribe((configuration) =>
            {
                this.RemediedSpellNameCB.ItemsSource = configuration.MagicSpells;
            });
        }
    }
}

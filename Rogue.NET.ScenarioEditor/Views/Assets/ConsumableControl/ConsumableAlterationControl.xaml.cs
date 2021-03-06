﻿using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.Common.Extension.Prism.EventAggregator;

using System.ComponentModel.Composition;
using System.Windows.Controls;
using System;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Consumable;
using Rogue.NET.ScenarioEditor.Events.Asset.Alteration;

namespace Rogue.NET.ScenarioEditor.Views.Assets.ConsumableControl
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export]
    public partial class ConsumableAlterationControl : UserControl
    {
        [ImportingConstructor]
        public ConsumableAlterationControl(IRogueEventAggregator eventAggregator)
        {
            InitializeComponent();

            eventAggregator.GetEvent<AlterationEffectChangedEvent>()
                           .Subscribe(e =>
                           {
                               var viewModel = this.DataContext as ConsumableAlterationTemplateViewModel;
                               if (viewModel == null)
                                   return;
                               
                               // Type cast the effect interface
                               if (e.Effect is IConsumableAlterationEffectTemplateViewModel &&
                                   e.Alteration == viewModel)
                                   viewModel.Effect = (e.Effect as IConsumableAlterationEffectTemplateViewModel);
                           });
        }
    }
}

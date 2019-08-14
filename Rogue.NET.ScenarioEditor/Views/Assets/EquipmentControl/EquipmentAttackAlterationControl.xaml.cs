﻿using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Equipment;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.EquipmentControl
{
    [Export]
    public partial class EquipmentAttackAlterationControl : UserControl
    {
        [ImportingConstructor]
        public EquipmentAttackAlterationControl(IRogueEventAggregator eventAggregator)
        {
            InitializeComponent();

            this.EffectChooser.AlterationEffectChosen += (sender, e) =>
            {
                var viewModel = this.DataContext as EquipmentAttackAlterationTemplateViewModel;
                if (viewModel == null)
                    return;

                // TODO:REGIONMANAGER
                //// Load new view
                //eventAggregator.GetEvent<AlterationEffectLoadRequestEvent>()
                //               .Publish(new AlterationEffectLoadRequestEventArgs()
                //               {
                //                   AlterationEffectViewType = e.AlterationEffectViewType,
                //                   AlterationEffectType = e.AlterationEffectType
                //               });
            };

            // Subscribe to response event for loading the alteration effect
            eventAggregator.GetEvent<AlterationEffectLoadResponseEvent>()
                           .Subscribe(e =>
                           {
                               var viewModel = this.DataContext as EquipmentAttackAlterationTemplateViewModel;
                               if (viewModel == null)
                                   return;

                               // Set new effect (to trigger binding)
                               viewModel.Effect = e.AlterationEffect as IEquipmentAttackAlterationEffectTemplateViewModel;
                           });
        }
    }
}

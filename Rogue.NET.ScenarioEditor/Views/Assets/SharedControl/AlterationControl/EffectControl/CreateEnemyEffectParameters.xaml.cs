﻿using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.Service.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Extension;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.SharedControl.AlterationControl.EffectControl
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export]
    public partial class CreateEnemyEffectParameters : UserControl
    {
        [ImportingConstructor]
        public CreateEnemyEffectParameters(
                IRogueEventAggregator eventAggregator,
                IScenarioCollectionProvider scenarioCollectionProvider)
        {
            InitializeComponent();
            Initialize(scenarioCollectionProvider);

            eventAggregator.GetEvent<ScenarioUpdateEvent>()
                           .Subscribe(provider =>
                           {
                               Initialize(provider);
                           });
        }

        private void Initialize(IScenarioCollectionProvider provider)
        {
            this.EnemyCB.ItemsSource = provider.Enemies.CreateDefaultView();
        }
    }
}

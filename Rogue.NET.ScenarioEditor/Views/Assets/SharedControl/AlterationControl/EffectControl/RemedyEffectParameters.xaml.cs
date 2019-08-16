﻿using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.Service.Interface;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.SharedControl.AlterationControl.EffectControl
{
    [Export]
    public partial class RemedyEffectParameters : UserControl
    {
        [ImportingConstructor]
        public RemedyEffectParameters(
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
            this.RemediedStateCB.ItemsSource = provider.AlteredCharacterStates;
        }
    }
}

using Prism.Commands;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.Events.Browser;
using Rogue.NET.ScenarioEditor.ViewModel.Browser.Interface;
using System.ComponentModel.Composition;
using System.Windows.Input;
using Rogue.NET.ScenarioEditor.Service.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Layout;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration;

namespace Rogue.NET.ScenarioEditor.ViewModel.Browser
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IScenarioAssetBrowserViewModel))]
    public class ScenarioAssetBrowserViewModel : IScenarioAssetBrowserViewModel
    {
        public IScenarioAssetGroupViewModel PlayerGroup { get; set; }
        public IScenarioAssetGroupViewModel LayoutGroup { get; set; }
        public IScenarioAssetGroupViewModel EnemyGroup { get; set; }
        public IScenarioAssetGroupViewModel FriendlyGroup { get; set; }
        public IScenarioAssetGroupViewModel EquipmentGroup { get; set; }
        public IScenarioAssetGroupViewModel ConsumableGroup { get; set; }
        public IScenarioAssetGroupViewModel DoodadGroup { get; set; }
        public IScenarioAssetGroupViewModel SkillSetGroup { get; set; }

        public ICommand CollapseAssetTreeCommand { get; set; }

        [ImportingConstructor]
        public ScenarioAssetBrowserViewModel(
            IRogueEventAggregator eventAggregator)
        {
            this.CollapseAssetTreeCommand = new DelegateCommand(() =>
            {
                eventAggregator.GetEvent<CollapseAssetTreeEvent>().Publish();
            });

            this.PlayerGroup = new ScenarioAssetGroupViewModel(typeof(PlayerTemplateViewModel), eventAggregator);
            this.LayoutGroup = new ScenarioAssetGroupViewModel(typeof(LayoutTemplateViewModel), eventAggregator);
            this.EnemyGroup = new ScenarioAssetGroupViewModel(typeof(EnemyTemplateViewModel), eventAggregator);
            this.FriendlyGroup = new ScenarioAssetGroupViewModel(typeof(FriendlyTemplateViewModel), eventAggregator);
            this.EquipmentGroup = new ScenarioAssetGroupViewModel(typeof(EquipmentTemplateViewModel), eventAggregator);
            this.ConsumableGroup = new ScenarioAssetGroupViewModel(typeof(ConsumableTemplateViewModel), eventAggregator);
            this.DoodadGroup = new ScenarioAssetGroupViewModel(typeof(DoodadTemplateViewModel), eventAggregator);
            this.SkillSetGroup = new ScenarioAssetGroupViewModel(typeof(SkillSetTemplateViewModel), eventAggregator);

            // TreeViewItem Selection:  WPF TreeViewItem has an issue handling selection. I gave up working with the resources to try and figure it out.
            eventAggregator.GetEvent<LoadAssetEvent>()
                           .Subscribe(selectedAsset =>
                           {
                               // De-Select all assets
                               foreach (var asset in this.PlayerGroup.Assets)
                                   asset.IsSelectedAsset = false;

                               foreach (var asset in this.LayoutGroup.Assets)
                                   asset.IsSelectedAsset = false;

                               foreach (var asset in this.EnemyGroup.Assets)
                                   asset.IsSelectedAsset = false;

                               foreach (var asset in this.FriendlyGroup.Assets)
                                   asset.IsSelectedAsset = false;

                               foreach (var asset in this.EquipmentGroup.Assets)
                                   asset.IsSelectedAsset = false;

                               foreach (var asset in this.ConsumableGroup.Assets)
                                   asset.IsSelectedAsset = false;

                               foreach (var asset in this.DoodadGroup.Assets)
                                   asset.IsSelectedAsset = false;

                               foreach (var asset in this.SkillSetGroup.Assets)
                                   asset.IsSelectedAsset = false;

                               // Explicitly setting here.
                               selectedAsset.IsSelectedAsset = true;
                           });
        }
    }
}

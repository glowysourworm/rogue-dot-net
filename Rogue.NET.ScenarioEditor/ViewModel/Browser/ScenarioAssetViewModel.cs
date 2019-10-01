using Rogue.NET.Common.Extension.Event;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.Events.Browser;
using Rogue.NET.ScenarioEditor.ViewModel.Browser.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using System.Windows.Input;

namespace Rogue.NET.ScenarioEditor.ViewModel.Browser
{
    public class ScenarioAssetViewModel : ScenarioAssetReadonlyViewModel, IScenarioAssetViewModel
    {
        public ScenarioAssetViewModel(IRogueEventAggregator eventAggregator, DungeonObjectTemplateViewModel templateViewModel) 
            : base(eventAggregator, templateViewModel)
        {
            Initialize(eventAggregator);
        }
        public ScenarioAssetViewModel(IRogueEventAggregator eventAggregator, TemplateViewModel templateViewModel)
            : base(eventAggregator, templateViewModel)
        {
            Initialize(eventAggregator);
        }
        private void Initialize(IRogueEventAggregator eventAggregator)
        {
            this.RemoveAssetCommand = new SimpleCommand(() =>
            {
                eventAggregator.GetEvent<RemoveAssetEvent>()
                               .Publish(this);
            });
            this.RenameAssetCommand = new SimpleCommand(() =>
            {
                eventAggregator.GetEvent<RenameAssetEvent>()
                               .Publish(this);
            });
            this.CopyAssetCommand = new SimpleCommand(() =>
            {
                eventAggregator.GetEvent<CopyAssetEvent>()
                               .Publish(this);
            });
        }
        public ICommand RemoveAssetCommand { get; set; }
        public ICommand CopyAssetCommand { get; set; }
        public ICommand RenameAssetCommand { get; set; }
    }
}

using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Extension.Event;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core.Utility;
using Rogue.NET.ScenarioEditor.Events.Browser;
using Rogue.NET.ScenarioEditor.ViewModel.Attribute;
using Rogue.NET.ScenarioEditor.ViewModel.Browser.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
using System;
using System.Windows.Input;

namespace Rogue.NET.ScenarioEditor.ViewModel.Browser
{
    public class ScenarioAssetReadonlyViewModel : NotifyViewModel, IScenarioAssetReadonlyViewModel
    {
        string _name = "";
        string _type = "";
        string _subType = "";
        SymbolDetailsTemplateViewModel _symbolDetailsViewModel;
        Type _assetType;
        public string Type
        {
            get { return _type; }
            private set { this.RaiseAndSetIfChanged(ref _type, value); }
        }
        public string SubType
        {
            get { return _subType; }
            private set { this.RaiseAndSetIfChanged(ref _subType, value); }
        }
        public string Name
        {
            get { return _name; }
            set { this.RaiseAndSetIfChanged(ref _name, value); }
        }
        public Type AssetType
        {
            get { return _assetType; }
            private set { this.RaiseAndSetIfChanged(ref _assetType, value); }
        }
        public SymbolDetailsTemplateViewModel SymbolDetails
        {
            get { return _symbolDetailsViewModel; }
            set { this.RaiseAndSetIfChanged(ref _symbolDetailsViewModel, value); }
        }
        public ICommand LoadAssetCommand { get; set; }
        public ScenarioAssetReadonlyViewModel(IRogueEventAggregator eventAggregator, DungeonObjectTemplateViewModel templateViewModel)
        {
            this.Name = templateViewModel.Name;
            this.SubType = templateViewModel is EquipmentTemplateViewModel ?
                                TextUtility.CamelCaseToTitleCase((templateViewModel as EquipmentTemplateViewModel).Type.ToString()) :
                                templateViewModel is ConsumableTemplateViewModel ?
                                TextUtility.CamelCaseToTitleCase((templateViewModel as ConsumableTemplateViewModel).Type.ToString()) :
                                "";
            this.SymbolDetails = templateViewModel.SymbolDetails;
            this.Type = templateViewModel.GetAttribute<UITypeAttribute>().DisplayName;
            this.AssetType = templateViewModel.GetType();

            Initialize(eventAggregator);
        }
        public ScenarioAssetReadonlyViewModel(IRogueEventAggregator eventAggregator, TemplateViewModel templateViewModel)
        {
            this.Name = templateViewModel.Name;
            this.SubType = "";
            this.Type = templateViewModel.GetAttribute<UITypeAttribute>().DisplayName;
            this.AssetType = templateViewModel.GetType();

            Initialize(eventAggregator);
        }
        private void Initialize(IRogueEventAggregator eventAggregator)
        {
            this.LoadAssetCommand = new SimpleCommand(() =>
            {
                eventAggregator.GetEvent<LoadAssetEvent>()
                               .Publish(this);
            });
        }
    }
}

using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Core.Processing.Command.Backend;
using Rogue.NET.Core.Processing.Command.Backend.CommandData;
using Rogue.NET.Core.Service.Interface;
using Rogue.NET.Scenario.Content.ViewModel.ItemGrid.Enum;
using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Rogue.NET.Scenario.Content.ViewModel.ItemGrid.PrimaryMode
{
    /// <summary>
    /// View Model component for the equipment item grid. updates from the backend are
    /// subscribed to for updating individual items; and two constructors are provided for
    /// injection / manual use.
    /// </summary>
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export]
    public class EquipmentPrimaryItemGridViewModel : EquipmentItemGridViewModelBase
    {
        EquipmentPrimaryMode _equipmentPrimaryMode;

        public EquipmentPrimaryMode PrimaryMode
        {
            get { return _equipmentPrimaryMode; }
            set { this.RaiseAndSetIfChanged(ref _equipmentPrimaryMode, value); Update(); }
        }

        [ImportingConstructor]
        public EquipmentPrimaryItemGridViewModel(
            IRogueEventAggregator eventAggregator,
            IModelService modelService) : base(eventAggregator, modelService)
        {
        }

        public override string Header
        {
            get
            {
                switch (this.PrimaryMode)
                {
                    case EquipmentPrimaryMode.Equip:
                        return "Equip";
                    case EquipmentPrimaryMode.Drop:
                        return "Drop";
                    default:
                        throw new Exception("Unhandled EquipmentPrimaryMode");
                }
            }
        }
        public override Brush HeaderBrush
        {
            get
            {
                switch (this.PrimaryMode)
                {
                    case EquipmentPrimaryMode.Equip:
                        return Brushes.White;
                    case EquipmentPrimaryMode.Drop:
                        return Brushes.Red;
                    default:
                        throw new Exception("Unhandled EquipmentPrimaryMode");
                }
            }
        }
        protected override Task ProcessSingleItemNonDialog(IRogueEventAggregator eventAggregator, string itemId)
        {
            switch (this.PrimaryMode)
            {
                case EquipmentPrimaryMode.Equip:
                    return eventAggregator.GetEvent<LevelCommand>()
                                          .Publish(new LevelCommandData(LevelCommandType.Equip, Compass.Null, itemId));
                case EquipmentPrimaryMode.Drop:
                    return eventAggregator.GetEvent<LevelCommand>()
                                          .Publish(new LevelCommandData(LevelCommandType.Drop, Compass.Null, itemId));
                default:
                    throw new Exception("Unhandled EquipmentPrimaryMode");
            }
        }
        protected override bool GetIsEnabled(IModelService modelService, Equipment item)
        {
            switch (this.PrimaryMode)
            {
                case EquipmentPrimaryMode.Equip:
                case EquipmentPrimaryMode.Drop:
                    return true;
                default:
                    throw new Exception("Unhandled EquipmentPrimaryMode");
            }
        }
        protected override void Update()
        {
            base.Update();

            OnPropertyChanged(() => this.Header);
            OnPropertyChanged(() => this.HeaderBrush);
        }
    }
}

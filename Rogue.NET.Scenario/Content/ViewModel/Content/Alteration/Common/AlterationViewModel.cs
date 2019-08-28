using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Effect;
using System;

namespace Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Common
{
    /// <summary>
    /// View-Model that is dual purpose for 1) Showing the Skill Tree and 2) Showing the Alterations
    /// List. The Constructor supports both (with many nullable parameters). 
    /// </summary>
    public class AlterationViewModel : RogueBaseViewModel
    {
        #region Backing Fields
        AlterationEffectViewModel _effect;
        AlterationCostViewModel _cost;
        AlterationCostType _alterationCostType;
        string _blockType;
        #endregion

        #region (public) Properties
        public AlterationEffectViewModel Effect
        {
            get { return _effect; }
            set { this.RaiseAndSetIfChanged(ref _effect, value); }
        }
        public AlterationCostViewModel Cost
        {
            get { return _cost; }
            set { this.RaiseAndSetIfChanged(ref _cost, value); }
        }
        public AlterationCostType CostType
        {
            get { return _alterationCostType; }
            set { this.RaiseAndSetIfChanged(ref _alterationCostType, value); }
        }
        public string BlockType
        {
            get { return _blockType; }
            set { this.RaiseAndSetIfChanged(ref _blockType, value); }
        }
        #endregion

        /// <summary>
        /// Constructor for the AlterationContainer properties
        /// </summary>
        public AlterationViewModel(
                IAlterationEffect effect, 
                AlterationCost cost,
                bool supportsBlocking,
                AlterationCostType costType,
                AlterationBlockType blockType) : base()
        {
            
            this.Cost = new AlterationCostViewModel(cost);
            this.CostType = costType;
            this.BlockType = supportsBlocking ? blockType.ToString() : "N/A";
            this.Effect = AlterationEffectViewModel.Create(effect);
        }

        /// <summary>
        /// Constructor for the AlterationContainer properties (using the templates)
        /// </summary>
        public AlterationViewModel(
                IAlterationEffectTemplate effect,
                AlterationCostTemplate cost,
                bool supportsBlocking,
                AlterationCostType costType,
                AlterationBlockType blockType) : base()
        {
            this.Cost = new AlterationCostViewModel(cost);
            this.CostType = costType;
            this.BlockType = supportsBlocking ? blockType.ToString() : "N/A";
            this.Effect = AlterationEffectViewModel.Create(effect);
        }
    }
}

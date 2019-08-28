using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;

namespace Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Common
{
    public class AlterationListViewModel : RogueBaseViewModel
    {
        AlterationCostViewModel _cost;
        AlterationEffectViewModel _effect;

        public AlterationCostViewModel Cost
        {
            get { return _cost; }
            set { this.RaiseAndSetIfChanged(ref _cost, value); }
        }
        public AlterationEffectViewModel Effect
        {
            get { return _effect; }
            set { this.RaiseAndSetIfChanged(ref _effect, value); }
        }

        /// <summary>
        /// Constructor to instantiate just the alteration cost
        /// </summary>
        public AlterationListViewModel(AlterationCost cost)
        {
            this.Cost = new AlterationCostViewModel(cost);
            this.Effect = null;
        }

        /// <summary>
        /// Constructor to instantiate just the alteration effect
        /// </summary>
        public AlterationListViewModel(IAlterationEffect effect)
        {
            this.Cost = null;
            this.Effect = AlterationEffectViewModel.Create(effect);
        }

        public AlterationListViewModel(AlterationCost cost, IAlterationEffect effect)
        {
            this.Cost = new AlterationCostViewModel(cost);
            this.Effect = AlterationEffectViewModel.Create(effect);
        }
    }
}

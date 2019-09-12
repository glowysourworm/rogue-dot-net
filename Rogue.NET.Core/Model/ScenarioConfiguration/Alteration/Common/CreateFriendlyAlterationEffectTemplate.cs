using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common
{
    [Serializable]
    public class CreateFriendlyAlterationEffectTemplate : Template, IConsumableAlterationEffectTemplate,
                                                                    IDoodadAlterationEffectTemplate,
                                                                    IEnemyAlterationEffectTemplate,
                                                                    ISkillAlterationEffectTemplate
    {
        AlterationRandomPlacementType _randomPlacementType;
        FriendlyTemplate _friendly;
        int _range;

        public AlterationRandomPlacementType RandomPlacementType
        {
            get { return _randomPlacementType; }
            set
            {
                if (_randomPlacementType != value)
                {
                    _randomPlacementType = value;
                    OnPropertyChanged("RandomPlacementType");
                }
            }
        }
        public FriendlyTemplate Friendly
        {
            get { return _friendly; }
            set
            {
                if (_friendly != value)
                {
                    _friendly = value;
                    OnPropertyChanged("Friendly");
                }
            }
        }
        public int Range
        {
            get { return _range; }
            set
            {
                if (_range != value)
                {
                    _range = value;
                    OnPropertyChanged("Range");
                }
            }
        }

        public CreateFriendlyAlterationEffectTemplate() { }
    }
}

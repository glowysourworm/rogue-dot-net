using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common
{
    [Serializable]
    public class CreateEnemyAlterationEffectTemplate : Template, IConsumableAlterationEffectTemplate,
                                                                 IDoodadAlterationEffectTemplate,
                                                                 IEnemyAlterationEffectTemplate,
                                                                 ISkillAlterationEffectTemplate
    {
        AlterationRandomPlacementType _randomPlacementType;
        EnemyTemplate _enemy;
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
        public EnemyTemplate Enemy
        {
            get { return _enemy; }
            set
            {
                if (_enemy != value)
                {
                    _enemy = value;
                    OnPropertyChanged("Enemy");
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

        public CreateEnemyAlterationEffectTemplate() { }
    }
}

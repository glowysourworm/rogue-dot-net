using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Interface;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common
{
    [Serializable]
    public class CreateMonsterAlterationEffectTemplate 
        : Template, IConsumableAlterationEffectTemplate,
                    IDoodadAlterationEffectTemplate,
                    IEnemyAlterationEffectTemplate,
                    ISkillAlterationEffectTemplate
    {
        AlterationRandomPlacementType _randomPlacementType;
        string _createMonsterEnemy;
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
        public string CreateMonsterEnemy
        {
            get { return _createMonsterEnemy; }
            set
            {
                if (_createMonsterEnemy != value)
                {
                    _createMonsterEnemy = value;
                    OnPropertyChanged("CreateMonsterEnemy");
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

        public CreateMonsterAlterationEffectTemplate() { }
    }
}

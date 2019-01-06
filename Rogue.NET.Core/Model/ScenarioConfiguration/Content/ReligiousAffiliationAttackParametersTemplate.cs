using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Content
{
    [Serializable]
    public class ReligiousAffiliationAttackParametersTemplate : Template
    {
        ReligionTemplate _enemyReligion;
        double _attackMultiplier;
        double _blockMultiplier;
        double _defenseMultiplier;


        public ReligionTemplate EnemyReligion
        {
            get { return _enemyReligion; }
            set
            {
                if (_enemyReligion != value)
                {
                    _enemyReligion = value;
                    OnPropertyChanged("EnemyReligion");
                }
            }
        }
        public double AttackMultiplier
        {
            get { return _attackMultiplier; }
            set
            {
                if (_attackMultiplier != value)
                {
                    _attackMultiplier = value;
                    OnPropertyChanged("AttackMultiplier");
                }
            }
        }
        public double BlockMultiplier
        {
            get { return _blockMultiplier; }
            set
            {
                if (_blockMultiplier != value)
                {
                    _blockMultiplier = value;
                    OnPropertyChanged("BlockMultiplier");
                }
            }
        }
        public double DefenseMultiplier
        {
            get { return _defenseMultiplier; }
            set
            {
                if (_defenseMultiplier != value)
                {
                    _defenseMultiplier = value;
                    OnPropertyChanged("DefenseMultiplier");
                }
            }
        }

        public ReligiousAffiliationAttackParametersTemplate()
        {
            this.EnemyReligion = new ReligionTemplate();
        }
    }
}

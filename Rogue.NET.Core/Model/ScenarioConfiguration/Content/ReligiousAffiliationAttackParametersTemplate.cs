using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Content
{
    [Serializable]
    public class ReligiousAffiliationAttackParametersTemplate : Template
    {
        string _enemyReligionName;
        double _attackMultiplier;
        double _blockMultiplier;
        double _defenseMultiplier;


        public string EnemyReligionName
        {
            get { return _enemyReligionName; }
            set
            {
                if (_enemyReligionName != value)
                {
                    _enemyReligionName = value;
                    OnPropertyChanged("EnemyReligionName");
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
            this.EnemyReligionName = "";
        }
    }
}

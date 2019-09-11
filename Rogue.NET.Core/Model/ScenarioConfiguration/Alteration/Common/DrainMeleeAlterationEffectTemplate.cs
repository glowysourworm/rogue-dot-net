using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Interface;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common
{
    /// <summary>
    /// Alteration Effect that causes Hp or Mp to be drained from one character to another during 
    /// Melee Combat ONLY
    /// </summary>
    [Serializable]
    public class DrainMeleeAlterationEffectTemplate : Template, IEquipmentAttackAlterationEffectTemplate
    {
        Range<double> _hp;
        Range<double> _mp;

        public Range<double> Hp
        {
            get { return _hp; }
            set
            {
                if (_hp != value)
                {
                    _hp = value;
                    OnPropertyChanged("Hp");
                }
            }
        }
        public Range<double> Mp
        {
            get { return _mp; }
            set
            {
                if (_mp != value)
                {
                    _mp = value;
                    OnPropertyChanged("Mp");
                }
            }
        }

        public DrainMeleeAlterationEffectTemplate()
        {
            this.Hp = new Range<double>(0, 0, 0, 100);
            this.Mp = new Range<double>(0, 0, 0, 100);
        }
    }
}

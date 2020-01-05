using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Interface;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common
{
    /// <summary>
    /// Alteration Effect that causes Hp or Stamina to be drained from one character to another during 
    /// Melee Combat ONLY
    /// </summary>
    [Serializable]
    public class DrainMeleeAlterationEffectTemplate : Template, IEquipmentAttackAlterationEffectTemplate
    {
        Range<double> _health;
        Range<double> _stamina;

        public Range<double> Health
        {
            get { return _health; }
            set
            {
                if (_health != value)
                {
                    _health = value;
                    OnPropertyChanged("Health");
                }
            }
        }
        public Range<double> Stamina
        {
            get { return _stamina; }
            set
            {
                if (_stamina != value)
                {
                    _stamina = value;
                    OnPropertyChanged("Stamina");
                }
            }
        }

        public DrainMeleeAlterationEffectTemplate()
        {
            this.Health = new Range<double>(0, 0);
            this.Stamina = new Range<double>(0, 0);
        }
    }
}

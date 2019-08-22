using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.Scenario.Dynamic.Alteration
{
    public class CharacterAlterationTypeValidator
    {
        // Types that ARE supported by the corresponding interface but ARE NOT collected
        // by the CharacterAlteration
        //
        // Example:  ICharacterAlterationEffect -> _characterAlterationList will show the SUPPORTED implementation types
        //                                         that are NOT collected.
        readonly IList<Type> _alterationEffectList;

        public CharacterAlterationTypeValidator()
        {
            _alterationEffectList = new List<Type>();

            Initialize();
        }

        private void Initialize()
        {
            // ICharacterAlterationEffect
            _alterationEffectList.Add(typeof(AttackAttributeMeleeAlterationEffect));
            _alterationEffectList.Add(typeof(ChangeLevelAlterationEffect));
            _alterationEffectList.Add(typeof(CreateMonsterAlterationEffect));
            _alterationEffectList.Add(typeof(EquipmentDamageAlterationEffect));
            _alterationEffectList.Add(typeof(EquipmentEnhanceAlterationEffect));
            _alterationEffectList.Add(typeof(OtherAlterationEffect));
            _alterationEffectList.Add(typeof(PermanentAlterationEffect));
            _alterationEffectList.Add(typeof(RevealAlterationEffect));
            _alterationEffectList.Add(typeof(RunAwayAlterationEffect));
            _alterationEffectList.Add(typeof(StealAlterationEffect));
            _alterationEffectList.Add(typeof(TeleportAlterationEffect));
        }

        /// <summary>
        /// Throws an exception if the alteration effect type is improper for using in the collector
        /// </summary>
        public void Validate(IAlterationEffect effect)
        {
            if (_alterationEffectList.Contains(effect.GetType()))
                ThrowException(effect.GetType());
        }

        private void ThrowException(Type type)
        {
            throw new Exception(type.ToString() + " should not be processed by the CharacterAlteration");
        }
    }
}

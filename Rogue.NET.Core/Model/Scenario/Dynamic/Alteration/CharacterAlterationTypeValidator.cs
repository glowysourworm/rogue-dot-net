using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Model.Scenario.Dynamic.Alteration
{
    public class CharacterAlterationTypeValidator
    {
        // Types that ARE supported by the corresponding interface but ARE NOT collected
        // by the CharacterAlteration
        //
        // Example:  ICharacterAlterationEffect -> _characterAlterationList will show the SUPPORTED implementation types
        //                                         that are NOT collected.
        readonly IList<Type> _consumableAlterationList;
        readonly IList<Type> _consumableProjectileAlterationList;
        readonly IList<Type> _doodadAlterationList;
        readonly IList<Type> _enemyAlterationList;
        readonly IList<Type> _skillAlterationList;

        public CharacterAlterationTypeValidator()
        {
            _consumableAlterationList = new List<Type>();
            _consumableProjectileAlterationList = new List<Type>();
            _doodadAlterationList = new List<Type>();
            _enemyAlterationList = new List<Type>();
            _skillAlterationList = new List<Type>();

            Initialize();
        }

        private void Initialize()
        {
            // ICharacterAlterationEffect
            _consumableAlterationList.Add(typeof(AttackAttributeMeleeAlterationEffect));
            _consumableAlterationList.Add(typeof(AttackAttributeTemporaryAlterationEffect));
            _consumableAlterationList.Add(typeof(ChangeLevelAlterationEffect));
            _consumableAlterationList.Add(typeof(CreateMonsterAlterationEffect));
            _consumableAlterationList.Add(typeof(EquipmentModifyAlterationEffect));
            _consumableAlterationList.Add(typeof(OtherAlterationEffect));
            _consumableAlterationList.Add(typeof(PermanentAlterationEffect));
            _consumableAlterationList.Add(typeof(RevealAlterationEffect));

            // ICharacterProjectileAlterationEffect
            _consumableProjectileAlterationList.Add(typeof(AttackAttributeMeleeAlterationEffect));
            _consumableProjectileAlterationList.Add(typeof(PermanentAlterationEffect));

            // IDoodadAlterationEffect
            _doodadAlterationList.Add(typeof(AttackAttributeMeleeAlterationEffect));
            _doodadAlterationList.Add(typeof(ChangeLevelAlterationEffect));
            _doodadAlterationList.Add(typeof(CreateMonsterAlterationEffect));
            _doodadAlterationList.Add(typeof(EquipmentModifyAlterationEffect));
            _doodadAlterationList.Add(typeof(PermanentAlterationEffect));
            _doodadAlterationList.Add(typeof(RevealAlterationEffect));
            _doodadAlterationList.Add(typeof(TeleportAlterationEffect));

            // IEnemyAlterationEffect
            _enemyAlterationList.Add(typeof(AttackAttributeMeleeAlterationEffect));
            _enemyAlterationList.Add(typeof(CreateMonsterAlterationEffect));
            _enemyAlterationList.Add(typeof(EquipmentModifyAlterationEffect));
            _enemyAlterationList.Add(typeof(OtherAlterationEffect));
            _enemyAlterationList.Add(typeof(PermanentAlterationEffect));
            _enemyAlterationList.Add(typeof(TeleportAlterationEffect));

            // ISkillAlterationEffect
            _skillAlterationList.Add(typeof(AttackAttributeMeleeAlterationEffect));
            _skillAlterationList.Add(typeof(ChangeLevelAlterationEffect));
            _skillAlterationList.Add(typeof(CreateMonsterAlterationEffect));
            _skillAlterationList.Add(typeof(EquipmentModifyAlterationEffect));
            _skillAlterationList.Add(typeof(OtherAlterationEffect));
            _skillAlterationList.Add(typeof(PermanentAlterationEffect));
            _skillAlterationList.Add(typeof(RevealAlterationEffect));
            _skillAlterationList.Add(typeof(TeleportAlterationEffect));
        }

        #region (public) Validate Methods
        /// <summary>
        /// Throws appropriate exception if type is not supposed to be processed by CharacterAlteration.
        /// </summary>
        public void Validate(IConsumableAlterationEffect effect)
        {
            var implementationType = effect.GetType();

            if (!_consumableAlterationList.Contains(implementationType))
                ThrowException(implementationType);

            else
                ThrowUnknownTypeException(implementationType);
        }

        /// <summary>
        /// Throws appropriate exception if type is not supposed to be processed by CharacterAlteration.
        /// </summary>
        public void Validate(IConsumableProjectileAlterationEffect effect)
        {
            var implementationType = effect.GetType();

            if (!_consumableProjectileAlterationList.Contains(implementationType))
                ThrowException(implementationType);

            else
                ThrowUnknownTypeException(implementationType);
        }

        /// <summary>
        /// Throws appropriate exception if type is not supposed to be processed by CharacterAlteration.
        /// </summary>
        public void Validate(IDoodadAlterationEffect effect)
        {
            var implementationType = effect.GetType();

            if (!_doodadAlterationList.Contains(implementationType))
                ThrowException(implementationType);

            else
                ThrowUnknownTypeException(implementationType);
        }

        /// <summary>
        /// Throws appropriate exception if type is not supposed to be processed by CharacterAlteration.
        /// </summary>
        public void Validate(IEnemyAlterationEffect effect)
        {
            var implementationType = effect.GetType();

            if (!_enemyAlterationList.Contains(implementationType))
                ThrowException(implementationType);

            else
                ThrowUnknownTypeException(implementationType);
        }

        /// <summary>
        /// Throws appropriate exception if type is not supposed to be processed by CharacterAlteration.
        /// </summary>
        public void Validate(IEquipmentCurseAlterationEffect effect)
        {
            return;
        }

        /// <summary>
        /// Throws appropriate exception if type is not supposed to be processed by CharacterAlteration.
        /// </summary>
        public void Validate(IEquipmentEquipAlterationEffect effect)
        {
            return;
        }

        /// <summary>
        /// Throws appropriate exception if type is not supposed to be processed by CharacterAlteration.
        /// </summary>
        public void Validate(ISkillAlterationEffect effect)
        {
            var implementationType = effect.GetType();

            if (!_skillAlterationList.Contains(implementationType))
                ThrowException(implementationType);

            else
                ThrowUnknownTypeException(implementationType);
        }
        #endregion

        private void ThrowException(Type type)
        {
            throw new Exception(type.ToString() + " should not be processed by the CharacterAlteration");
        }

        private void ThrowUnknownTypeException(Type type)
        {
            throw new Exception(type.ToString() + " is an unknown implementation type");
        }
    }
}

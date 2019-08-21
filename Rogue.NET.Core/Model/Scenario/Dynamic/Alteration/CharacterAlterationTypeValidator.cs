﻿using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
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
            _consumableAlterationList.Add(typeof(EquipmentDamageAlterationEffect));
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
            _doodadAlterationList.Add(typeof(EquipmentDamageAlterationEffect));
            _doodadAlterationList.Add(typeof(PermanentAlterationEffect));
            _doodadAlterationList.Add(typeof(RevealAlterationEffect));
            _doodadAlterationList.Add(typeof(TeleportAlterationEffect));

            // IEnemyAlterationEffect
            _enemyAlterationList.Add(typeof(AttackAttributeMeleeAlterationEffect));
            _enemyAlterationList.Add(typeof(CreateMonsterAlterationEffect));
            _enemyAlterationList.Add(typeof(EquipmentDamageAlterationEffect));
            _enemyAlterationList.Add(typeof(PermanentAlterationEffect));
            _enemyAlterationList.Add(typeof(RunAwayAlterationEffect));
            _enemyAlterationList.Add(typeof(StealAlterationEffect));
            _enemyAlterationList.Add(typeof(TeleportAlterationEffect));

            // ISkillAlterationEffect
            _skillAlterationList.Add(typeof(AttackAttributeMeleeAlterationEffect));
            _skillAlterationList.Add(typeof(ChangeLevelAlterationEffect));
            _skillAlterationList.Add(typeof(CreateMonsterAlterationEffect));
            _skillAlterationList.Add(typeof(EquipmentDamageAlterationEffect));
            _skillAlterationList.Add(typeof(OtherAlterationEffect));
            _skillAlterationList.Add(typeof(PermanentAlterationEffect));
            _skillAlterationList.Add(typeof(RevealAlterationEffect));
            _skillAlterationList.Add(typeof(TeleportAlterationEffect));
        }

        public void Validate(IAlterationEffect effect)
        {
            if (effect is IConsumableAlterationEffect)
                Validate(effect as IConsumableAlterationEffect);

            else if (effect is IConsumableProjectileAlterationEffect)
                Validate(effect as IConsumableProjectileAlterationEffect);

            else if (effect is IDoodadAlterationEffect)
                Validate(effect as IDoodadAlterationEffect);

            else if (effect is IEquipmentAttackAlterationEffect)
                Validate(effect as IEquipmentAttackAlterationEffect);

            else if (effect is IEquipmentCurseAlterationEffect)
                Validate(effect as IEquipmentCurseAlterationEffect);

            else if (effect is IEquipmentEquipAlterationEffect)
                Validate(effect as IEquipmentEquipAlterationEffect);

            else if (effect is IEnemyAlterationEffect)
                Validate(effect as IEnemyAlterationEffect);

            else if (effect is ISkillAlterationEffect)
                Validate(effect as ISkillAlterationEffect);

            else
                throw new Exception("Unknwon IAlterationEffect Type");
        }

        #region (protected) Validate Methods
        /// <summary>
        /// Throws appropriate exception if type is not supposed to be processed by CharacterAlteration.
        /// </summary>
        protected void Validate(IConsumableAlterationEffect effect)
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
        protected void Validate(IConsumableProjectileAlterationEffect effect)
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
        protected void Validate(IDoodadAlterationEffect effect)
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
        protected void Validate(IEnemyAlterationEffect effect)
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
        protected void Validate(IEquipmentCurseAlterationEffect effect)
        {
            return;
        }

        /// <summary>
        /// Throws appropriate exception if type is not supposed to be processed by CharacterAlteration.
        /// </summary>
        protected void Validate(IEquipmentEquipAlterationEffect effect)
        {
            return;
        }

        /// <summary>
        /// Throws appropriate exception if type is not supposed to be processed by CharacterAlteration.
        /// </summary>
        protected void Validate(ISkillAlterationEffect effect)
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

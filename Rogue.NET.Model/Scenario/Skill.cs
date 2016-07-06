using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;
using System.Xml.Serialization;
using Rogue.NET.Common;
using Rogue.NET.Model.Scenario;
using Rogue.NET.Model;
using Rogue.NET.Common.Collections;
using System.Runtime.Serialization;
using System.Reflection;

namespace Rogue.NET.Scenario.Model
{
    [Serializable]
    public class Spell : ScenarioObject
    {
        public List<AnimationTemplate> Animations { get; set; }
        public AlterationCostTemplate Cost { get; set; }
        new public AlterationEffectTemplate Effect { get; set; }
        public AlterationEffectTemplate AuraEffect { get; set; }
        public AlterationType Type { get; set; }
        public AlterationBlockType BlockType { get; set; }
        public AlterationMagicEffectType OtherEffectType { get; set; }
        public AlterationAttackAttributeType AttackAttributeType { get; set; }

        public double EffectRange { get; set; }
        public bool Stackable { get; set; }

        /// <summary>
        /// Had to use name because of circular reference
        /// </summary>
        public string CreateMonsterEnemyName { get; set; }
        public string DisplayName { get; set; }

        public bool IsAura()
        {
            return this.Type == AlterationType.PassiveSource || this.Type == AlterationType.PassiveAura;
        }

        public Spell() { }
        public Spell(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            var properties = typeof(Spell).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(z => z.GetSetMethod() != null);
            foreach (var property in properties)
            {
                var val = info.GetValue(property.Name, property.PropertyType);
                property.SetValue(this, val);
            }
        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            var properties = typeof(Spell).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(z => z.GetSetMethod() != null);
            foreach (var property in properties)
                info.AddValue(property.Name, property.GetValue(this));
        }     
    }
}

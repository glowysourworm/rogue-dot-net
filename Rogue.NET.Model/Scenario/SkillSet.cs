using Rogue.NET.Common;
using Rogue.NET.Common.Collections;
using Rogue.NET.Scenario.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace Rogue.NET.Model.Scenario
{
    [Serializable]
    public class SkillSet : ScenarioObject
    {
        int _level;
        int _levelLearned;
        int _emphasis;
        bool _isActive;
        bool _isTurnedOn;
        bool _isLearned;
        double _progress;

        public SerializableObservableCollection<Spell> Skills { get; set; }
        public int Level
        {
            get { return _level; }
            set
            {
                _level = value;
                OnPropertyChanged("Level");
                OnPropertyChanged("DisplayLevel");
                OnPropertyChanged("Progress");
            }
        }
        public int DisplayLevel
        {
            get { return _level + 1; }
        }
        public int LevelLearned
        {
            get { return _levelLearned; }
            set
            {
                _levelLearned = value;
                OnPropertyChanged("LevelLearned");
            }
        }
        public int Emphasis
        {
            get { return _emphasis; }
            set
            {
                _emphasis = value;
                OnPropertyChanged("Emphasis");
            }
        }
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                OnPropertyChanged("IsActive");
            }
        }
        public bool IsTurnedOn
        {
            get { return _isTurnedOn; }
            set
            {
                _isTurnedOn = value;
                OnPropertyChanged("IsTurnedOn");
            }
        }
        public bool IsLearned
        {
            get { return _isLearned; }
            set
            {
                _isLearned = value;
                OnPropertyChanged("IsLearned");
            }
        }
        public double SkillProgress
        {
            get { return _progress; }
            set
            {
                _progress = value;
                OnPropertyChanged("SkillProgress");
            }
        }

        public Spell CurrentSkill
        {
            get
            {
                if (this.Level < this.Skills.Count)
                    return this.Skills[this.Level];

                return null;
            }
                
        }

        public SkillSet(string name, ImageResources icon) : base(name, icon)
        {
            this.IsActive = false;
            this.IsLearned = false;
            this.Emphasis = 0;
            this.Skills = new SerializableObservableCollection<Spell>();
        }
        public SkillSet(string name, ImageResources icon, double scale)
            : base(name, icon, scale)
        {
            this.IsActive = false;
            this.IsLearned = false;
            this.Emphasis = 0;
            this.Skills = new SerializableObservableCollection<Spell>();
        }
        public SkillSet(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            var properties = typeof(SkillSet).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(z => z.GetSetMethod() != null);
            foreach (var property in properties)
            {
                var val = info.GetValue(property.Name, property.PropertyType);
                property.SetValue(this, val);
            }
        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            var properties = typeof(SkillSet).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(z => z.GetSetMethod() != null);
            foreach (var property in properties)
                info.AddValue(property.Name, property.GetValue(this));
        }     
    }
}

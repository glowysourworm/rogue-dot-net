using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.PubSubEvents;
using Rogue.NET.Model;
using Rogue.NET.ScenarioEditor.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.ScenarioEditor
{
    public class ScenarioConfigurationChangeTracker
    {
        readonly IEventAggregator _eventAggregator;

        public ScenarioConfigurationChangeTracker(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }
        public void RegisterConfig(ScenarioConfiguration config)
        {
            foreach (Template o in config.AnimationTemplates)
                TrackObject(o);

            foreach (Template o in config.AttackAttributes)
                TrackObject(o);

            foreach (Template o in config.BrushTemplates)
                TrackObject(o);

            foreach (Template o in config.CharacterClasses)
                TrackObject(o);

            foreach (Template o in config.ConsumableTemplates)
                TrackObject(o);

            foreach (Template o in config.DoodadTemplates)
                TrackObject(o);

            foreach (Template o in config.EnemyTemplates)
                TrackObject(o);

            foreach (Template o in config.EquipmentTemplates)
                TrackObject(o);

            foreach (Template o in config.MagicSpells)
                TrackObject(o);

            foreach (Template o in config.PenTemplates)
                TrackObject(o);

            foreach (Template o in config.SkillTemplates)
                TrackObject(o);

            TrackObject(config.PlayerTemplate);
            TrackObject(config.DungeonTemplate);
        }
        public void TrackObject(Template obj)
        {
            TrackChanges(obj);

            _eventAggregator.GetEvent<ScenarioEditorMessageEvent>().Publish(new ScenarioEditorMessageEvent()
            {
                Message = string.Format("Scenario Object added " + obj.Name + " ({0})", obj.Guid)
            });
        }
        private void TrackChanges(INotifyPropertyChanged obj)
        {
            // recurse through notifiable properties
            var notifiableProperties = obj.GetType().GetProperties().Where(p => p.PropertyType is INotifyPropertyChanged);
            foreach (var property in notifiableProperties)
                TrackChanges((INotifyPropertyChanged)property.GetValue(obj));

            // iterate collections
            var collections = obj.GetType().GetFields().Where(p => p.FieldType.GetInterface("IList") != null);
            foreach (var field in collections)
            {
                var genericType = field.FieldType.GetGenericArguments()[0];
                if (genericType.GetInterface("INotifyPropertyChanged") != null)
                {
                    foreach (var collectionObj in (IEnumerable)field.GetValue(obj))
                        TrackChanges((INotifyPropertyChanged)collectionObj);
                }
            }
            var propertyCollections = obj.GetType().GetProperties().Where(p => p.PropertyType.GetInterface("IList") != null);
            foreach (var property in propertyCollections)
            {
                var genericType = property.PropertyType.GetGenericArguments()[0];
                if (genericType.GetInterface("INotifyPropertyChanged") != null)
                {
                    foreach (var collectionObj in (IEnumerable)property.GetValue(obj))
                        TrackChanges((INotifyPropertyChanged)collectionObj);
                }
            }

            obj.PropertyChanged += OnTrackedObjectPropertyChanged;
        }

        private void OnTrackedObjectPropertyChanged(object o, PropertyChangedEventArgs e)
        {
            var template = o as Template;
            if (template != null)
            {
                var msg = template.Name + " ({0}) " + e.PropertyName + " Changed";
                _eventAggregator.GetEvent<ScenarioEditorMessageEvent>().Publish(new ScenarioEditorMessageEvent()
                {
                    Message = string.Format(msg, template.Guid)
                });
            }
        }
    }
}

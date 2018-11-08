using System.Linq;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Collections.Generic;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using System.Reflection;
using System;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;

namespace Rogue.NET.ScenarioEditor.Utility.Undo
{
    public class UndoAccumulator<T> where T : INotifyPropertyChanged
    {
        // Target for undo / redo
        readonly T _target;

        // Undo stack / Redo stack
        Stack<UndoChange> _undoAccumulator;
        Stack<UndoChange> _redoAccumulator;

        // PropertyInfo dictionary for fast retrieval and caching of reflection objects
        Dictionary<Type, Dictionary<string, PropertyInfo>> _propertyDict;

        // Pending changes for INotifyPropertyChanging event hooks
        Dictionary<string, UndoChange> _pendingChangeDict;

        // Require blocker to prevent events from being consumed during an undo / redo
        bool _performingUndo = false;

        // Require another blocker to prevent registering new changes
        bool _blocked = false;

        public event EventHandler<string> UndoChangedEvent;

        public UndoAccumulator(T target)
        {
            _target = target;

            _undoAccumulator = new Stack<UndoChange>();
            _redoAccumulator = new Stack<UndoChange>();

            _propertyDict = new Dictionary<Type, Dictionary<string, PropertyInfo>>();
            _pendingChangeDict = new Dictionary<string, UndoChange>();

            Initialize();
        }

        private void Initialize()
        {
            Recurse(_target);
        }

        #region (public) Members
        public bool CanUndo()
        {
            return _undoAccumulator.Count > 0;
        }
        public bool CanRedo()
        {
            return _redoAccumulator.Count > 0;
        }
        public void Clear()
        {
            _undoAccumulator.Clear();
            _redoAccumulator.Clear();
        }
        public void Undo()
        {
            var undoChange = _undoAccumulator.Pop();

            if (undoChange.Type == UndoChangeType.Property)
                UndoProperty(undoChange);
            else
                UndoCollection(undoChange);

            _redoAccumulator.Push(undoChange);
        }
        public void Redo()
        {
            var redoChange = _redoAccumulator.Pop();

            if (redoChange.Type == UndoChangeType.Property)
                RedoProperty(redoChange);
            else
                RedoCollection(redoChange);

            _undoAccumulator.Push(redoChange);
        }
        public void Unhook()
        {
            Recurse(_target, true);
        }
        public void Block()
        {
            _blocked = true;
        }
        public void UnBlock()
        {
            _blocked = false;
        }
        #endregion

        #region (private) Tree Traversal Methods
        private void Recurse(INotifyPropertyChanged node, bool unbind = false)
        {
            // Update dictionary of property info entries
            PropertyInfo[] properties;
            var type = node.GetType();
            if (!_propertyDict.ContainsKey(type))
            {
                properties = type.GetProperties();
                _propertyDict[type] = properties.ToDictionary(x => x.Name, x => x);
            }
            else
                properties = _propertyDict[type].Values.ToArray();

            // Loop through properties and bind / recurse through the tree
            foreach (var property in properties)
            {
                var propertyValue = property.GetValue(node);

                // Observable Collections
                if (propertyValue is INotifyCollectionChanged)
                {
                    if (unbind)
                        UnBind(propertyValue as INotifyCollectionChanged);
                    else
                        Bind(propertyValue as INotifyCollectionChanged);

                    Loop(propertyValue as IList, unbind);
                }

                // Complex object properties
                else if (propertyValue is INotifyPropertyChanged)
                {
                    if (unbind)
                        UnBind(propertyValue as INotifyPropertyChanged);
                    else
                        Bind(propertyValue as INotifyPropertyChanged);

                    Recurse(propertyValue as INotifyPropertyChanged, unbind);
                }
            }
        }

        private void Loop(IList collection, bool unbind = false)
        {
            if (collection.Count <= 0)
                return;

            foreach (var item in collection)
            {
                if (item is TemplateViewModel)
                {
                    if (unbind)
                        UnBind(item as TemplateViewModel);
                    else
                        Bind(item as TemplateViewModel);

                    Recurse(item as TemplateViewModel, unbind);
                }
            }
        }
        #endregion

        #region (private) Binding Methods
        private void Bind(INotifyPropertyChanged node)
        {
            if (!(node is INotifyPropertyChanging))
                throw new Exception("Undo property must also implement INotifyPropertyChanging");

            node.PropertyChanged += OnPropertyChanged;
            (node as INotifyPropertyChanging).PropertyChanging += OnPropertyChanging;
        }
        private void Bind(INotifyCollectionChanged collectionNode)
        {
            collectionNode.CollectionChanged += OnCollectionChanged;
        }
        private void UnBind(INotifyPropertyChanged node)
        {
            if (!(node is INotifyPropertyChanging))
                throw new Exception("Undo property must also implement INotifyPropertyChanging");

            node.PropertyChanged -= OnPropertyChanged;
            (node as INotifyPropertyChanging).PropertyChanging += OnPropertyChanging;
        }
        private void UnBind(INotifyCollectionChanged collectionNode)
        {
            collectionNode.CollectionChanged -= OnCollectionChanged;
        }
        #endregion

        #region (private) Undo / Redo Transform Methods
        private void UndoProperty(UndoChange undoChange)
        {
            _performingUndo = true;

            var type = undoChange.Node.GetType();
            var property = _propertyDict[type][undoChange.PropertyName];

            property.SetValue(undoChange.Node, undoChange.OldValue);

            _performingUndo = false;

            UndoChangedEvent(this, property.Name + " of type " + type.Name + " was changed to " + undoChange.OldValue?.ToString() ?? " (Null)");
        }
        private void RedoProperty(UndoChange redoChange)
        {
            _performingUndo = true;

            var type = redoChange.Node.GetType();
            var property = _propertyDict[type][redoChange.PropertyName];

            property.SetValue(redoChange.Node, redoChange.NewValue);

            _performingUndo = false;

            UndoChangedEvent(this, property.Name + " of type " + type.Name + " was changed to " + redoChange.NewValue?.ToString() ?? " (Null)");
        }
        private void UndoCollection(UndoChange undoChange)
        {
            _performingUndo = true;

            var collection = undoChange.CollectionNode as IList;

            switch (undoChange.CollectionChangeAction)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in undoChange.NewItems)
                        collection.Remove(item);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in undoChange.OldItems)
                        if (!collection.Contains(item))
                            collection.Add(item);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    throw new Exception("Replace not handled for Undo collections");
                case NotifyCollectionChangedAction.Move:
                    throw new Exception("Move not handled for Undo collections");
                case NotifyCollectionChangedAction.Reset:
                    collection.Clear();
                    foreach (var item in undoChange.OldItems)
                        collection.Add(item);
                    break;
                default:
                    break;
            }

            _performingUndo = false;

            UndoChangedEvent(this, "Element " + (undoChange.CollectionChangeAction == NotifyCollectionChangedAction.Remove ? " added to " : " removed from ") + undoChange.CollectionNode.GetType().Name);
        }
        private void RedoCollection(UndoChange redoChange)
        {
            _performingUndo = true;

            var collection = redoChange.CollectionNode as IList;

            switch (redoChange.CollectionChangeAction)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in redoChange.NewItems)
                        collection.Add(item);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in redoChange.OldItems)
                        if (collection.Contains(item))
                            collection.Remove(item);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    throw new Exception("Replace not handled for Undo collections");
                case NotifyCollectionChangedAction.Move:
                    throw new Exception("Move not handled for Undo collections");
                case NotifyCollectionChangedAction.Reset:
                    collection.Clear();
                    break;
                default:
                    break;
            }

            _performingUndo = false;

            UndoChangedEvent(this, "Element " + (redoChange.CollectionChangeAction == NotifyCollectionChangedAction.Add ? " added to " : " removed from ") + redoChange.CollectionNode.GetType().Name);
        }
        #endregion

        #region (private) Handlers
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_performingUndo || _blocked)
                return;

            var type = sender.GetType();
            var property = _propertyDict[type][e.PropertyName];
            var propertyValue = property.GetValue(sender);

            var undoEventArgs = e as UndoPropertyChangedEventArgs;
            var undoChange = _pendingChangeDict[undoEventArgs.PropertyChangingEventId];

            // Set new value
            undoChange.NewValue = propertyValue;

            // Push onto change stack
            _undoAccumulator.Push(undoChange);

            // Clear the redo stack (can't redo after a change is performed)
            _redoAccumulator.Clear();

            // Remove from pending Dictionary
            _pendingChangeDict.Remove(undoEventArgs.PropertyChangingEventId);

            // Notify listeners
            UndoChangedEvent(this, property.Name + " of type " + type.Name + " was changed to " + propertyValue?.ToString() ?? " (Null)");
        }

        // Sent before change
        private void OnPropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            if (_performingUndo || _blocked)
                return;

            var type = sender.GetType();
            var property = _propertyDict[type][e.PropertyName];
            var propertyValue = property.GetValue(sender);

            var undoEventArgs = e as UndoPropertyChangingEventArgs;

            // Add to pending changes
            _pendingChangeDict.Add(undoEventArgs.Id, new UndoChange()
            {
                Type = UndoChangeType.Property,
                Node = sender as INotifyPropertyChanged,
                OldValue = propertyValue,
                PropertyName = e.PropertyName
            });
        }
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_performingUndo || _blocked)
                return;

            var collection = sender as IList;

            // Hook / Unhook changed items
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    Loop(e.NewItems, false);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    Loop(e.OldItems, true);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    throw new Exception("Replace not handled for Undo collections");
                case NotifyCollectionChangedAction.Move:
                    throw new Exception("Move not handled for Undo collections");
                case NotifyCollectionChangedAction.Reset:
                    Loop(collection, true);
                    break;
                default:
                    break;
            }

            // Add change to the undo stack
            _undoAccumulator.Push(new UndoChange()
            {
                Type = UndoChangeType.Collection,
                CollectionNode = sender as INotifyCollectionChanged,
                CollectionChangeAction = e.Action,
                NewItems = e.NewItems,
                NewStartingIndex = e.NewStartingIndex,
                OldItems = e.OldItems,
                OldStartingIndex = e.OldStartingIndex
            });

            // Clear the redo stack (can't redo after change is made)
            _redoAccumulator.Clear();

            // Notify listeners
            UndoChangedEvent(this, "Element " + (e.Action == NotifyCollectionChangedAction.Add ? " added to " : " removed from ") + sender.GetType().Name);
        }
        #endregion
    }
}

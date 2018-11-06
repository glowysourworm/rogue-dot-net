using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Rogue.NET.ScenarioEditor.Utility.Undo
{
    public class UndoChange
    {
        public UndoChangeType Type { get; set; }

        public INotifyPropertyChanged Node { get; set; }
        public string PropertyName { get; set; }
        public object OldValue { get; set; }
        public object NewValue { get; set; }

        public NotifyCollectionChangedAction CollectionChangeAction { get; set; }
        public INotifyCollectionChanged CollectionNode { get; set; }
        public IList NewItems { get; set; }
        public IList OldItems { get; set; }
        public int NewStartingIndex { get; set; }
        public int OldStartingIndex { get; set; }
    }
}

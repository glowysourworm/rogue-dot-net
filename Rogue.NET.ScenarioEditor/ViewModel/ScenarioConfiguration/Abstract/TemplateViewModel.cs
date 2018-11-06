using Rogue.NET.ScenarioEditor.Utility.Undo;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract
{
    public class TemplateViewModel : INotifyPropertyChanged, INotifyPropertyChanging
    {
        public TemplateViewModel()
        {
            this.Name = "New Template";
            this.Guid = System.Guid.NewGuid().ToString();
        }

        private string _name;
        private string _guid;

        public string Name
        {
            get { return _name; }
            set { this.RaiseAndSetIfChanged(ref _name, value); }
        }
        public string Guid
        {
            get { return _guid; }
            set { this.RaiseAndSetIfChanged(ref _guid, value); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;

        protected virtual void RaiseAndSetIfChanged<T>(ref T field, T value, [CallerMemberName] string memberName = "")
        {
            var changed = false;
            if (field == null)
                changed = value != null;
            else
                changed = !field.Equals(value);

            if (changed)
            {
                // Use the Id to relate the two events
                var eventArgs = new UndoPropertyChangingEventArgs(memberName);
                if (PropertyChanging != null)
                    PropertyChanging(this, eventArgs);

                field = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new UndoPropertyChangedEventArgs(eventArgs.Id, memberName));
            }
        }
    }
}

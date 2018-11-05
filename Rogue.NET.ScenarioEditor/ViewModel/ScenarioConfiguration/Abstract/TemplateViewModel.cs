using ReactiveUI;
using System;
using System.ComponentModel;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract
{
    public abstract class TemplateViewModel : ReactiveObject
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
    }
}

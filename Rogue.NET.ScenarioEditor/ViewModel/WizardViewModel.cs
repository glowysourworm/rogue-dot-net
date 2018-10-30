using Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Rogue.NET.ScenarioEditor.ViewModel
{
    public interface IWizardViewModel
    {
        Type FirstPageType { get; set; }

        string Title { get; set; }

        string PayloadTitle { get; set; }

        object Payload { get; set; }

        object SecondaryPayload { get; set; }

        IEnumerable<string> WizardSteps { get; set; }

        bool NextButtonVisible { get; set; }
        bool BackButtonVisible { get; set; }
        bool CancelButtonVisible { get; set; }

        bool NextButtonEnabled { get; set; }
        bool BackButtonEnabled { get; set; }
        bool CancelButtonEnabled { get; set; }

        ICommand NextCommand { get; }
        ICommand BackCommand { get; }
        ICommand CancelCommand { get; }

        event EventHandler<WizardPageChangeEventArgs> WizardPageChangeEvent;
    }
    public class WizardPageChangeEventArgs : EventArgs
    {
        public MessageBoxResult Result { get; set; }
    }
    public class WizardViewModel : IWizardViewModel, INotifyPropertyChanged
    {
        public Type FirstPageType { get; set; }

        public string Title { get; set; }

        string _payloadTitle = "";
        public string PayloadTitle 
        {
            get { return _payloadTitle; }
            set
            {
                _payloadTitle = value;
                OnPropertyChanged("PayloadTitle");
            }
        }

        public IEnumerable<string> WizardSteps { get; set; }

        public object Payload { get; set; }

        public object SecondaryPayload { get; set; }

        public bool NextButtonVisible { get; set; }
        public bool BackButtonVisible { get; set; }
        public bool CancelButtonVisible { get; set; }

        public bool NextButtonEnabled { get; set; }
        public bool BackButtonEnabled { get; set; }
        public bool CancelButtonEnabled { get; set; }


        public ICommand NextCommand
        {
            get { return new DelegateCommand(() => { WizardPageChangeEvent(this, new WizardPageChangeEventArgs() { Result = MessageBoxResult.Yes }); }); }
        }

        public ICommand BackCommand
        {
            get { return new DelegateCommand(() => { WizardPageChangeEvent(this, new WizardPageChangeEventArgs() { Result = MessageBoxResult.No }); }); }
        }

        public ICommand CancelCommand
        {
            get { return new DelegateCommand(() => { WizardPageChangeEvent(this, new WizardPageChangeEventArgs() { Result = MessageBoxResult.Cancel }); }); }
        }

        public event EventHandler<WizardPageChangeEventArgs> WizardPageChangeEvent;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}

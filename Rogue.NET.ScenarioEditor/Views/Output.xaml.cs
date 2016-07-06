using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.Unity;
using Rogue.NET.ScenarioEditor.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Rogue.NET.ScenarioEditor.Views
{
    public partial class Output : UserControl
    {
        public class MessageItem
        {
            public string Message { get; set; }
        }

        public Output()
        {
            InitializeComponent();
        }
        [InjectionConstructor]
        public Output(IEventAggregator eventAggregator)
        {
            InitializeComponent();

            var collection = new ObservableCollection<MessageItem>();
            eventAggregator.GetEvent<ScenarioEditorMessageEvent>().Subscribe((e) =>
            {
                collection.Insert(0, new MessageItem()
                {
                    Message = e.Message
                });
            }, true);

            this.MessageListBox.ItemsSource = collection;
        }
    }
}

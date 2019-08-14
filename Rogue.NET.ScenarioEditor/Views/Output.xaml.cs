using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.Events;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views
{
    [Export]
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
        [ImportingConstructor]
        public Output(IRogueEventAggregator eventAggregator)
        {
            InitializeComponent();

            var collection = new ObservableCollection<MessageItem>();
            eventAggregator.GetEvent<ScenarioEditorMessageEvent>().Subscribe((e) =>
            {
                collection.Insert(0, new MessageItem()
                {
                    Message = e.Message
                });
            });

            this.MessageListBox.ItemsSource = collection;
        }
    }
}

using Rogue.NET.Core.Service.Interface;
using Rogue.NET.Scenario.Content.ViewModel.Content;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using Rogue.NET.Scenario.Content.Views.Dialog.Interface;
using Rogue.NET.Common.Extension.Event;
using System.Collections.Generic;

namespace Rogue.NET.Scenario.Content.Views.Dialog
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export]
    public partial class NoteDialogView : UserControl, IDialogView
    {
        public event SimpleEventHandler<IDialogView> DialogViewFinishedEvent;

        [ImportingConstructor]
        public NoteDialogView()
        {
            InitializeComponent();

            this.OkButton.Click += (sender, e) =>
            {
                if (this.DialogViewFinishedEvent != null)
                    this.DialogViewFinishedEvent(this);
            };
        }

        public IEnumerable<string> GetSelectedItemIds()
        {
            return new List<string>();
        }
    }
}

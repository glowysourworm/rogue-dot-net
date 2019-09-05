﻿using Rogue.NET.Common.Extension.Event;
using Rogue.NET.Scenario.Content.Views.Dialog.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Controls;

namespace Rogue.NET.Scenario.Content.Views.Dialog
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export]
    public partial class CommandsDialogView : UserControl, IDialogView
    {
        public event SimpleEventHandler<IDialogView> DialogViewFinishedEvent;

        [ImportingConstructor]
        public CommandsDialogView()
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

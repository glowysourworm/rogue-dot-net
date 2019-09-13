using Prism.Mef.Modularity;
using Prism.Modularity;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Common.Extension.Prism.RegionManager.Interface;
using Rogue.NET.Core.Processing.Command.Backend;
using Rogue.NET.Core.Processing.Command.Backend.CommandData;
using Rogue.NET.Core.Processing.Event.Dialog;
using Rogue.NET.Scenario.Constant;
using Rogue.NET.Scenario.Content.Views.Dialog.Interface;
using System.ComponentModel.Composition;
using System.Windows;
using System;
using Rogue.NET.Core.Processing.Event.Backend;

namespace Rogue.NET
{
    [ModuleExport("RogueModule", typeof(RogueModule))]
    public class RogueModule : IModule
    {
        readonly IRogueEventAggregator _eventAggregator;
        readonly IRogueRegionManager _regionManager;

        [ImportingConstructor]
        public RogueModule(IRogueEventAggregator eventAggregator, IRogueRegionManager regionManager)
        {
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            // Dialog Event:  Going to show a "Synchronous" "Dialog" "Window"
            //
            // Brief History:  This has been an issue to deal with because of all of the
            //                 involved events wired up to the Shell for showing a window.
            //
            //                 Window.Show resulted in un-desired behavior for forcing 
            //                 the proper event sequence.
            //
            //                 Window.ShowDialog became a mess because the dialog result
            //                 had to be set while the view was being "consumed"..
            //
            //                 I'd like to use the region manager to try and create a
            //                 synchronous dialog "window" (region) so that it can be done smoothly
            //                 without dealing with Window.ShowDialog
            //
            //                 Another way is to create multiple region managers and have a 
            //                 separate Shell window. I'm not sure WPF will allow two windows
            //                 with separate dispatchers.
            //                 
            _eventAggregator.GetEvent<DialogEvent>()
                            .Subscribe(dialogUpdate =>
                            {
                                // Get regions involved with the dialog sequence
                                var mainRegion = _regionManager.GetRegion(RegionName.MainRegion);
                                var dialogRegion = _regionManager.GetRegion(RegionName.DialogRegion);

                                // Set opacity to dim the background
                                mainRegion.Opacity = 0.5;

                                // Show the dialog region
                                dialogRegion.Visibility = Visibility.Visible;

                                // Get the IDialogContainer
                                var dialogContainer = dialogRegion.Content as IDialogContainer;

                                // Initialize the dialog container with the new update - which
                                // loads the proper content view into the container
                                dialogContainer.Initialize(dialogUpdate);

                                // Hook the completed method to finish the dialog sequence
                                dialogContainer.DialogFinishedEvent += OnDialogFinished;
                            });
        }

        // NOTE*** A couple things might consider a better design: async / await method isn't propagated with        
        //         a Task (does this work properly in the call stack to wait on the method?)
        //  
        //         Another is the UserCommandEventArgs. This depends on the view-model passed into the dialog
        //         view and other parameters that are specific to what's happening. So, it's assumed to be
        //         "ready to go" so that the event aggregator can just fire a user command event back to the
        //         back end (ONLY IF IT'S NON-NULL)
        //
        //         For null UserCommandEventArgs - there's no user command to fire. So, wasn't sure how to
        //         handle this case using the same dialog "cycle".
        //
        private async void OnDialogFinished(IDialogContainer dialogContainer, object eventData)
        {
            // Get regions involved with the dialog sequence
            var mainRegion = _regionManager.GetRegion(RegionName.MainRegion);
            var dialogRegion = _regionManager.GetRegion(RegionName.DialogRegion);

            // Set opacity to dim the background
            mainRegion.Opacity = 1;

            // Show the dialog region
            dialogRegion.Visibility = Visibility.Collapsed;

            // Unhook event to complete the sequence
            dialogContainer.DialogFinishedEvent -= OnDialogFinished;

            // Fire event to signal the end of the dialog sequence
            _eventAggregator.GetEvent<DialogEventFinished>()
                            .Publish();

            // Fire event to backend ONLY IF event args is non-null (there's an event to be processed)
            if (eventData != null)
            {
                if (eventData is PlayerCommandData)
                    await _eventAggregator.GetEvent<PlayerCommand>().Publish(eventData as PlayerCommandData);

                else if (eventData is PlayerMultiItemCommandData)
                    await _eventAggregator.GetEvent<PlayerMultiItemCommand>().Publish(eventData as PlayerMultiItemCommandData);

                else
                    throw new Exception("Unhandled Event Data Type ScenarioModule.OnDialogFinished");
            }
        }
    }
}

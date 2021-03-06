﻿using Rogue.NET.Common.Extension.Event;
using Rogue.NET.Scenario.Processing.Controller.Enum;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Rogue.NET.Scenario.Processing.Controller.Interface
{
    /// <summary>
    /// Component that keeps track of the game mode / routing user commands / issuing returns from 
    /// the frontend with stored state data (like Targeting mode)
    /// </summary>
    public interface ICommandRouter
    {
        event SimpleEventHandler RequestMaximizedWindowEvent;

        /// <summary>
        /// Enters game mode - allows user to post commands to the backend
        /// </summary>
        void Start();

        /// <summary>
        /// Exits game mode - blocks user input to the backend / frontend
        /// </summary>
        void Stop();

        /// <summary>
        /// Gets / sets the current command mode
        /// </summary>
        GameCommandMode CommandMode { get; set; }

        /// <summary>
        /// Issues command from the keyboard to the appropriate place
        /// </summary>
        Task IssueCommand(Key key, bool shift, bool ctrl, bool alt);
    }
}

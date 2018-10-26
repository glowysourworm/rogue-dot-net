using Rogue.NET.Core.Model.Enums;
using System;

namespace Rogue.NET.Core.Logic.Event
{
    public class LevelChangeEventArgs : EventArgs
    {
        public int LevelNumber { get; set; }
        public PlayerStartLocation StartLocation { get; set; }
    }
}

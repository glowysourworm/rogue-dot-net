using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Reflection;

namespace Rogue.NET.Common
{
    public class LevelCommandArgs : EventArgs
    {
        public LevelAction Action = LevelAction.Null;
        public Compass Direction = Compass.Null;

        /// <summary>
        /// Id of associated item or skill
        /// </summary>
        public string ItemId = "";

        public LevelCommandArgs(LevelAction action, Compass direction, string id)
        {
            Action = action;
            Direction = direction;
            ItemId = id;
        }
    }
    public class LevelMessageEventArgs : EventArgs
    {
        public string Message { get; private set; }
        public LevelMessageEventArgs(string msg)
        {
            this.Message = msg;
        }
    }
    public class ImportantMessageEventArgs : EventArgs
    {
        public string[] MessageLines { get; private set; }
        public ImportantMessageEventArgs(string[] messageLines)
        {
            this.MessageLines = messageLines;
        }
    }
    public class PlayerAdvancementEventArgs : EventArgs
    {
        public string Header { get; private set; }
        public string[] MessageLines { get; private set; }
        public PlayerAdvancementEventArgs(string header, string[] lines)
        {
            this.Header = header;
            this.MessageLines = lines;
        }
    }
    public class ObjectiveMessageEventArgs : EventArgs
    {
        public string Header { get; private set; }
        public string[] MessageLines { get; private set; }
        public ObjectiveMessageEventArgs(string header, string[] lines)
        {
            this.Header = header;
            this.MessageLines = lines;
        }
    }
    public class LoadLevelEventArgs : EventArgs
    {
        public int LevelNumber { get; set; }
        public PlayerStartLocation StartLocation { get; set; }
        public LoadLevelEventArgs(int number, PlayerStartLocation startLoc)
        {
            this.LevelNumber = number;
            this.StartLocation = startLoc;
        }
    }
    public class ColorEventArgs : EventArgs
    {
        public Color NewColor { get; set; }
        public ColorEventArgs(Color c)
        {
            this.NewColor = c;
        }
    }
    public class EnemyTargetedArgs
    {
        public Point[] Locations { get; set; }
        public EnemyTargetedArgs(Point[] pts)
        {
            this.Locations = pts;
        }
    }
    public class LevelTemporaryEventArgs : EventArgs
    {
        public int EventTime { get; set; }
        public LevelTemporaryEventType Type { get; set; }
        public LevelTemporaryEventArgs(int time, LevelTemporaryEventType type)
        {
            this.EventTime = time;
            this.Type = type;
        }
    }
    public class DebugGoToLevelEventArgs : EventArgs
    {
        public int LevelNumber { get; set; }
        public DebugGoToLevelEventArgs(int number)
        {
            this.LevelNumber = number;
        }
    }
}

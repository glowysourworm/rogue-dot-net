using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Rogue.NET.Scenario.Content.ViewModel.LevelCanvas.Inteface
{
    public interface ILevelCanvasElement
    {
        /// <summary>
        /// Canvas Location (Canvas.Left, Canvas.Top) of the ILevelCanvasElement
        /// </summary>
        Point Location { get; set; }
    }
}

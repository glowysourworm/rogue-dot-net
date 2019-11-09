using Rogue.NET.Core.Media;
using Rogue.NET.Core.Media.Animation.Interface;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Rogue.NET.Scenario.Content.ViewModel.LevelCanvas.Inteface
{
    /// <summary>
    /// Component responsible for data binding for the LevelCanvas
    /// </summary>
    public interface ILevelCanvasViewModel : INotifyPropertyChanged
    {
        DrawingBrush VisibleLayer { get; set; }
        DrawingBrush ExploredLayer { get; set; }
        DrawingBrush RevealedLayer { get; set; }
        DrawingBrush TerrainLayer { get; set; }
        DrawingBrush ExploredOpacityMask { get; set; }
        DrawingBrush RevealedOpacityMask { get; set; }
        DrawingBrush VisibleOpacityMask { get; set; }

        /// <summary>
        /// Layer of Visuals for the Light Radii
        /// </summary>
        ObservableCollection<LevelCanvasShape> LightRadii { get; set; }

        /// <summary>
        /// Layer of Visuals for the Auras
        /// </summary>
        ObservableCollection<LevelCanvasShape> Auras { get; set; }

        /// <summary>
        /// Layer of Visuals for the Characters
        /// </summary>
        ObservableCollection<LevelCanvasImage> Characters { get; set; }

        /// <summary>
        /// Layer of Visuals for the Items
        /// </summary>
        ObservableCollection<LevelCanvasImage> Items { get; set; }

        /// <summary>
        /// Layer of Visuals for the Doodads
        /// </summary>
        ObservableCollection<LevelCanvasImage> Doodads { get; set; }

        /// <summary>
        /// Layer of Visuals for the Animations
        /// </summary>
        ObservableCollection<FrameworkElement> Animations { get; set; }

        /// <summary>
        /// Visual for the player
        /// </summary>
        LevelCanvasImage Player { get; set; }

        int LevelWidth { get; set; }
        int LevelHeight { get; set; }

        void UpdateLayout(RegionBoundary boundary);
        void UpdateContent(IEnumerable<ScenarioObject> contents, Player player);
        void UpdateLayoutVisibility(IEnumerable<GridLocation> exploredLocations,
                                    IEnumerable<GridLocation> visibleLocations,
                                    IEnumerable<GridLocation> revealedLocations);

        Task PlayAnimationSeries(IAnimationPlayer animationPlayer);
        void PlayTargetAnimation(IAnimationPlayer targetAnimationPlayer);
        void StopTargetAnimation();
    }
}

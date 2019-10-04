using Rogue.NET.Core.Media;
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
        /// <summary>
        /// Wall layer of the layout
        /// </summary>
        Path WallLayout { get; }

        /// <summary>
        /// Door layer of the layout
        /// </summary>
        Path DoorLayout { get; }

        /// <summary>
        /// Revealed layer of the layout
        /// </summary>
        Path RevealedLayout { get; }

        /// <summary>
        /// Opacity DrawingBrush for explored portion of the layout
        /// </summary>
        DrawingBrush ExploredOpacityMask { get; }

        /// <summary>
        /// Opacity DrawingBrush for revealed portion of the layout
        /// </summary>
        DrawingBrush RevealedOpacityMask { get; }

        /// <summary>
        /// Opacity DrawingBrush for visible portion of the layout
        /// </summary>
        DrawingBrush VisibleOpacityMask { get; }

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
        int LevelContainerWidth { get; }
        int LevelContainerHeight { get; }

        void UpdateLayout(CellRectangle levelBounds, Color wallColor, Color doorColor);
        void UpdateContent(IEnumerable<ScenarioObject> contents, Player player);
        void UpdateLayoutVisibility(IEnumerable<GridLocation> exploredLocations,
                                    IEnumerable<GridLocation> visibleLocations,
                                    IEnumerable<GridLocation> revealedLocations);

        Task PlayAnimationSeries(IEnumerable<IEnumerable<AnimationQueue>> animations);
        void PlayTargetAnimation(AnimationQueue animation);
        void StopTargetAnimation();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Rogue.NET.Scenario.Control
{
    /// <summary>
    /// Panel that arranges elements on an ellipse geometry. For animations, it attaches clocks when child
    /// elements are added.
    /// </summary>
    public class EllipsePanel : Canvas
    {
        const int PADDING = 60;
        const int ITEM_HEIGHT = 60;
        const int ITEM_WIDTH = 40;

        Dictionary<FrameworkElement, EllipsePanelAnimation> _animationDict;

        delegate void VoidDelegate();

        public EllipsePanel()
        {
            _animationDict = new Dictionary<FrameworkElement, EllipsePanelAnimation>();
        }

        // Attach Animation Clocks
        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);

            if (visualRemoved != null && _animationDict.ContainsKey(visualRemoved as FrameworkElement))
                _animationDict.Remove(visualRemoved as FrameworkElement);

            if (visualAdded != null && !_animationDict.ContainsKey(visualAdded as FrameworkElement))
                _animationDict.Add(visualAdded as FrameworkElement, new EllipsePanelAnimation());

            Application.Current.Dispatcher.BeginInvoke(new VoidDelegate(Reset));
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            Application.Current.Dispatcher.BeginInvoke(new VoidDelegate(Reset));
        }

        private void Reset()
        {
            // Re-Calculate Ellipse Geometry
            var geometry =
                new EllipseGeometry(
                    new Point((this.RenderSize.Width - ITEM_WIDTH) / 2.0D, (this.RenderSize.Height - ITEM_HEIGHT) / 2.0D),
                              (this.RenderSize.Width / 2.0D) - PADDING, (this.RenderSize.Height / 2.0D) - PADDING)
                              .GetFlattenedPathGeometry();

            // Define Geometry -> Set relative offset -> Seek to initial point
            var counter = 0D;
            foreach (var keyValuePair in _animationDict)
            {
                keyValuePair.Value.DefineGeometry(geometry, keyValuePair.Key);
                keyValuePair.Value.SetRelativeOffset(counter++ / _animationDict.Count);
                keyValuePair.Value.Seek(0.1);
            }
        }
    }
}

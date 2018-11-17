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
    public class EllipsePanel : Panel
    {
        EllipseGeometry _ellipseGeometry;

        Dictionary<FrameworkElement, EllipsePanelAnimation> _animationDict;

        public EllipsePanel()
        {
            _ellipseGeometry = new EllipseGeometry();
            _animationDict = new Dictionary<FrameworkElement, EllipsePanelAnimation>();
        }

        // Attach Animation Clocks
        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);

            if (_animationDict.ContainsKey(visualRemoved as FrameworkElement))
                _animationDict.Remove(visualRemoved as FrameworkElement);

            if (!_animationDict.ContainsKey(visualAdded as FrameworkElement))
                _animationDict.Add(visualAdded as FrameworkElement, new EllipsePanelAnimation(_ellipseGeometry, visualAdded as FrameworkElement));

            ResetAnimations();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            // Re-calculate ellipse geometry
            _ellipseGeometry.RadiusX = sizeInfo.NewSize.Width;
            _ellipseGeometry.RadiusY = sizeInfo.NewSize.Height;

            ResetAnimations();
        }

        private void ResetAnimations()
        {
            // Define Geometry -> Set relative offset -> Seek to initial point
            foreach (var keyValuePair in _animationDict)
            {
                keyValuePair.Value.DefineGeometry(_ellipseGeometry, keyValuePair.Key);
                keyValuePair.Value.SetRelativeOffset(1.0D / _animationDict.Count);
                keyValuePair.Value.Seek(0);
            }
        }
    }
}

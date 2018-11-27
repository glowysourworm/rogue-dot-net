using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(
                "SelectedItem", 
                typeof(DependencyObject), 
                typeof(EllipsePanel), 
                new PropertyMetadata(new PropertyChangedCallback(OnSelectedItemChanged)));

        IList<EllipsePanelAnimation> _animations;

        PathGeometry _ellipseGeometry;

        delegate void VoidDelegate();

        public DependencyObject SelectedItem
        {
            get { return (DependencyObject)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public EllipsePanel()
        {
            _animations = new List<EllipsePanelAnimation>();
            _ellipseGeometry = new PathGeometry();
        }

        // Attach Animation Clocks
        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);

            // Remove
            var animationRemoved = _animations.FirstOrDefault(x => x.Element == visualRemoved);
            if (animationRemoved != null)
                _animations.Remove(animationRemoved);

            // Add
            if (visualAdded != null)
                _animations.Add(new EllipsePanelAnimation(visualAdded as FrameworkElement));

            Application.Current.Dispatcher.BeginInvoke(new VoidDelegate(Reset));
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            var width = (this.RenderSize.Width / 2.0D) - PADDING;
            var height = (this.RenderSize.Height / 2.0D) - PADDING;
            var center = new Point((this.RenderSize.Width - ITEM_WIDTH) / 2.0D, (this.RenderSize.Height - ITEM_HEIGHT) / 2.0D);

            // Re-Calculate Ellipse Geometry
            //
            // Flip by 90 degrees and squish back down to give a starting point at the center position
            _ellipseGeometry =
                new EllipseGeometry(center, height, width, new RotateTransform(90, center.X, center.Y))
                        .GetFlattenedPathGeometry();

            Application.Current.Dispatcher.BeginInvoke(new VoidDelegate(Reset));
        }

        // When item is selected - seek to selection
        protected static void OnSelectedItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var element = e.NewValue as FrameworkElement;
            var panel = sender as EllipsePanel;
            if (element != null || panel == null)
            {
                panel.AnimateTo(element);
            }
        }

        private void Reset()
        {
            // Reset the offset for each element
            var counter = 0D;
            foreach (var animation in _animations)
                animation.Offset = (counter++ / _animations.Count);

            AnimateTo(null);
        }

        private void AnimateTo(FrameworkElement element)
        {
            EllipsePanelAnimation selectedAnimation = null;

            // Center this element
            if (element != null)
            {
                // Had to cast this here because of container.. TODO: fix the container problem
                selectedAnimation = _animations.FirstOrDefault(x => (x.Element as ListBoxItem).Content == element);
            }

            // Either send elements back to starting place or center the selected element
            var delta = selectedAnimation == null ? 0 :
                        selectedAnimation.Offset > 0.5 ? 1 - selectedAnimation.Offset : // Past the furthest back location
                                                         0 - selectedAnimation.Offset;  // Before the furthest back location

            // Animations each element from current (relative) offset -> next position (either current "Bump" or 
            // to "Select" the targeted element) -> Plays animation
            foreach (var animation in _animations)
            {
                // Center the element by moving it to relative position 0;
                animation.Animate(_ellipseGeometry, animation.Offset + delta);
            }

            // "Reset" animation offsets
            if (_animations.Any())
            {
                var maxOffset = _animations.Max(x => x.Offset);
                var minOffset = _animations.Min(x => x.Offset);

                // "Rewind" the animations
                if (maxOffset > 1)
                {
                    foreach (var animation in _animations)
                        animation.Offset = animation.Offset % 1;
                }
                // "Unwind" the animtions
                else if (minOffset < 0)
                {
                    foreach (var animation in _animations)
                        animation.Offset = (animation.Offset - (int)animation.Offset) + 1;
                }
            }
        }
    }
}

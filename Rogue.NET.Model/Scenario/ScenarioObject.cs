using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rogue.NET.Common;
using System.ComponentModel;
using System.Windows.Controls;
using Rogue.NET.Common.Views;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Runtime.Serialization;

namespace Rogue.NET.Model.Scenario
{
    [Serializable]
    public abstract class ScenarioObject : NamedObject
    {
        #region SymbolInfo
        public SymbolDetails SymbolInfo { get; set; }
        #endregion

        public CellPoint Location
        {
            get { return _cellPoint; }
            set
            {
                if (value != null)
                {
                    var newMargin = new Thickness(
                        value.Column * ScenarioConfiguration.CELLWIDTH,
                        value.Row * ScenarioConfiguration.CELLHEIGHT,
                        0,
                        0);

                    if (this is Enemy && (newMargin.Left != 0 && Margin.Top != 0))
                    {
                        var marginAnimation = new ThicknessAnimation(this.Margin, newMargin, new Duration(new TimeSpan(0, 0, 0, 0, 100)));
                        this.ApplyAnimationClock(MarginProperty, marginAnimation.CreateClock());
                    }
                    else
                        this.Margin = newMargin;
                }

                _cellPoint = value;
                OnPropertyChanged("Location");
            }
        }

        public Point GetVisualLocation()
        {
            return new Point(
                (this.Location.Column * ScenarioConfiguration.CELLWIDTH),
                (this.Location.Row * ScenarioConfiguration.CELLHEIGHT));
        }

        bool _isExplored = false;
        bool _isHidden = false;
        bool _isRevealed = false;
        bool _isPhysicallyVisible = false;
        CellPoint _cellPoint;

        public bool IsExplored
        {
            get { return _isExplored; }
            set
            {
                _isExplored = value;
                SetVisibility();
                InvalidateVisual();
            }
        }
        public bool IsHidden
        {
            get { return _isHidden; }
            set
            {
                _isHidden = value;
                SetVisibility();
                InvalidateVisual();
            }
        }
        public bool IsRevealed
        {
            get { return _isRevealed; }
            set
            {
                _isRevealed = value;
                SetVisibility();
                InvalidateVisual();
            }
        }
        public bool IsPhysicallyVisible
        {
            get { return _isPhysicallyVisible; }
            set
            {
                _isPhysicallyVisible = value;
                SetVisibility();
                InvalidateVisual();
            }
        }

        public ScenarioObject()
        {
            this.RogueName = "Unnamed";
            this.SymbolInfo = new SymbolDetails(1, 10, ImageResources.AmuletBlack);
            this.Location = new CellPoint();

            this.Height = ScenarioConfiguration.CELLHEIGHT;
            this.Width = ScenarioConfiguration.CELLWIDTH;
            this.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            this.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            Canvas.SetZIndex(this, 10);

            SetVisibility();
            InvalidateVisual();
        }
        public ScenarioObject(string name)
        {
            this.RogueName = name;
            this.SymbolInfo = new SymbolDetails(1, 10, ImageResources.AmuletBlack);
            this.Location = new CellPoint();

            this.Height = ScenarioConfiguration.CELLHEIGHT;
            this.Width = ScenarioConfiguration.CELLWIDTH;
            this.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            this.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            Canvas.SetZIndex(this, 10);

            SetVisibility();
            InvalidateVisual();
        }
        public ScenarioObject(string name, ImageResources icon)
        {
            this.RogueName = name;
            this.SymbolInfo = new SymbolDetails(1, 10, icon);
            this.Location = new CellPoint();

            this.Height = ScenarioConfiguration.CELLHEIGHT;
            this.Width = ScenarioConfiguration.CELLWIDTH;
            this.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            this.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            Canvas.SetZIndex(this, 10);

            SetVisibility();
            InvalidateVisual();
        }
        public ScenarioObject(string name, ImageResources icon, double scale)
        {
            this.RogueName = name;
            this.SymbolInfo = new SymbolDetails(scale, 10, icon);
            this.Location = new CellPoint();

            this.Height = ScenarioConfiguration.CELLHEIGHT * scale;
            this.Width = ScenarioConfiguration.CELLWIDTH * scale;
            this.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            this.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            Canvas.SetZIndex(this, 10);

            SetVisibility();
            InvalidateVisual();
        }
        public ScenarioObject(string name, string symbol, string color)
        {
            this.RogueName = name;
            this.SymbolInfo = new SymbolDetails(1, 10, symbol, color);
            this.Location = new CellPoint();

            this.Height = ScenarioConfiguration.CELLHEIGHT;
            this.Width = ScenarioConfiguration.CELLWIDTH;
            this.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            this.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            Canvas.SetZIndex(this, 10);

            SetVisibility();
            InvalidateVisual();
        }
        public ScenarioObject(string name, string symbol, string color, double scale)
        {
            this.RogueName = name;
            this.SymbolInfo = new SymbolDetails(scale, 10, symbol, color);
            this.Location = new CellPoint();

            this.Height = ScenarioConfiguration.CELLHEIGHT * scale;
            this.Width = ScenarioConfiguration.CELLWIDTH * scale;
            this.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            this.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            Canvas.SetZIndex(this, 10);

            SetVisibility();
            InvalidateVisual();
        }
        public ScenarioObject(string name, SmileyMoods mood, string body, string line, string aura)
        {
            this.RogueName = name;
            this.SymbolInfo = new SymbolDetails(1, 10, mood, body, line, aura);
            this.Location = new CellPoint();

            this.Height = ScenarioConfiguration.CELLHEIGHT;
            this.Width = ScenarioConfiguration.CELLWIDTH;
            this.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            this.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            Canvas.SetZIndex(this, 10);

            SetVisibility();
            InvalidateVisual();
        }

        public ScenarioObject(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.SymbolInfo = (SymbolDetails)info.GetValue("SymbolInfo", typeof(SymbolDetails));
            this.Location = (CellPoint)info.GetValue("Location", typeof(CellPoint));

            this.IsExplored = (bool)info.GetValue("IsExplored", typeof(bool));
            this.IsHidden = (bool)info.GetValue("IsHidden", typeof(bool));
            this.IsRevealed = (bool)info.GetValue("IsRevealed", typeof(bool));
            this.IsPhysicallyVisible = (bool)info.GetValue("IsPhysicallyVisible", typeof(bool));

            this.Height = ScenarioConfiguration.CELLHEIGHT * this.SymbolInfo.Scale;
            this.Width = ScenarioConfiguration.CELLWIDTH * this.SymbolInfo.Scale;

            this.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            this.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            Canvas.SetZIndex(this, 10);

            SetVisibility();
            InvalidateVisual();
        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("SymbolInfo", this.SymbolInfo);
            info.AddValue("Location", this.Location);
            info.AddValue("IsExplored", this.IsExplored);
            info.AddValue("IsHidden", this.IsHidden);
            info.AddValue("IsRevealed", this.IsRevealed);
            info.AddValue("IsPhysicallyVisible", this.IsPhysicallyVisible);
        }

        public void SetVisibility()
        {
            this.Visibility = (_isRevealed || _isPhysicallyVisible) && !_isHidden ? Visibility.Visible : Visibility.Hidden;
        }

        #region Layout
        protected override Size MeasureOverride(Size availableSize)
        {
            // TODO: remove this
            if (this.SymbolInfo != null)
                this.SymbolInfo.Measure(availableSize);

            return base.MeasureOverride(availableSize);
        }
        protected override Size ArrangeOverride(Size finalSize)
        {
            // TODO: remove this
            if (this.SymbolInfo != null)
                this.SymbolInfo.Arrange(new Rect(finalSize));

            return base.ArrangeOverride(finalSize);
        }
        protected override Visual GetVisualChild(int index)
        {
            return this.SymbolInfo;
        }
        protected override int VisualChildrenCount
        {
            get
            {
                return 1;
            }
        }
        #endregion

        public override string ToString()
        {
            return this.RogueName;
        }
    }

    public class ScenarioObjectNameComparer : IEqualityComparer<ScenarioObject>
    {
        public bool Equals(ScenarioObject x, ScenarioObject y)
        {
            if (x == null || y == null)
                return x == y;

            return x.RogueName.Equals(y.RogueName);
        }

        public int GetHashCode(ScenarioObject obj)
        {
            return obj.RogueName.GetHashCode();
        }
    }
    public class ScenarioObjectIdComparer : IEqualityComparer<ScenarioObject>
    {
        public bool Equals(ScenarioObject x, ScenarioObject y)
        {
            if (x == null || y == null)
                return x == y;

            return x.Id.Equals(y.Id);
        }

        public int GetHashCode(ScenarioObject obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}

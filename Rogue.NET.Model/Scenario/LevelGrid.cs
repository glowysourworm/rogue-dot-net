using System;
using System.Linq;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Collections.Generic;
using Rogue.NET.Common;
using System.Runtime.Serialization;


namespace Rogue.NET.Model.Scenario
{
    [Serializable]
    public class CellPoint : INotifyPropertyChanged
    {
        public static CellPoint Empty = new CellPoint(-1, -1);

        int _row;
        int _column;

        public int Row 
        {
            get { return _row; }
            set
            {
                _row = value;
                OnPropertyChanged("Row");
            }
        }
        public int Column
        {
            get { return _column; }
            set
            {
                _column = value;
                OnPropertyChanged("Column");
            }
        }

        public CellPoint() { }
        public CellPoint(CellPoint copy)
        {
            this.Row = copy.Row;
            this.Column = copy.Column;
        }
        public CellPoint(int row, int col)
        {
            Row = row;
            Column = col;
        }
        public static bool operator==(CellPoint p1, CellPoint p2)
        {
            return p1.Equals(p2);
        }
        public static bool operator !=(CellPoint p1, CellPoint p2)
        {
            return !p1.Equals(p2);
        }
        public override string ToString()
        {
            return "Column=" + Column.ToString() + " Row=" + Row.ToString();
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj.GetType() == typeof(CellPoint))
                return ((CellPoint)obj).Row == this.Row && ((CellPoint)obj).Column == this.Column;

            return false;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public Point ToPoint()
        {
            return new Point(this.Column, this.Row);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
    [Serializable]
    public class CellRectangle
    {
        public CellPoint Location;
        public int CellHeight = -1;
        public int CellWidth = -1;
        public CellRectangle()
        {
            Location = new CellPoint(-1, -1);
        }
        public CellRectangle(CellPoint p, int cellwidth, int cellheight)
        {
            Location = p;
            CellHeight = cellheight;
            CellWidth = cellwidth;
        }
        public int Left { get { return this.Location.Column; } }
        public int Right { get { return this.Location.Column + this.CellWidth; } }
        public int Top { get { return this.Location.Row; } }
        public int Bottom { get { return this.Location.Row + this.CellHeight; } }

        public override string ToString()
        {
            return "X=" + Location.Column + "Y=" + Location.Row + "Width=" + CellWidth + "Height=" + CellHeight;
        }
        public static CellRectangle Join(CellRectangle r1, CellRectangle r2)
        {
            CellPoint tl = new CellPoint(Math.Min(r1.Location.Row, r2.Location.Row), Math.Min(r1.Location.Column, r2.Location.Column));
            CellPoint br = new CellPoint(Math.Max(r1.Location.Row + r1.CellHeight, r2.Location.Row + r2.CellHeight), Math.Max(r1.Location.Column + r1.CellWidth, r2.Location.Column + r2.CellWidth));
            return new CellRectangle(tl, br.Column - tl.Column, br.Row - tl.Row);
        }
        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(CellRectangle))
            {
                return (this.Location.Equals(((CellRectangle)obj).Location)
                    && this.CellHeight == ((CellRectangle)obj).CellHeight && this.CellWidth == ((CellRectangle)obj).CellWidth);
            }
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        /// <summary>
        /// Includes Boundary
        /// </summary>
        public bool Contains(CellPoint cp)
        {
            if (cp.Column < Location.Column)
                return false;

            if (cp.Column > Location.Column + CellWidth)
                return false;

            if (cp.Row < Location.Row)
                return false;

            if (cp.Row > Location.Row + CellHeight)
                return false;

            return true;
        }
        public bool Contains(CellRectangle cr)
        {
            if (!Contains(cr.Location))
                return false;

            if (cr.Location.Column + cr.CellWidth >= this.Location.Column + this.CellWidth)
                return false;

            if (cr.Location.Row + cr.CellHeight >= this.Location.Row + this.CellHeight)
                return false;

            return true;
        }
    }
    [Serializable]
    public class Cell : Shape, ISerializable
    {
        static Pen _doorPen = null;
        static Brush _fillBrush = null;
        static Cell()
        {
            _doorPen = new Pen(Brushes.Fuchsia, 4);
            _doorPen.EndLineCap = PenLineCap.Round;
            _doorPen.StartLineCap = PenLineCap.Round;

            _fillBrush = new SolidColorBrush(Color.FromArgb(155, 200, 5, 200));
        }

        PathGeometry _definingGeometry;

        CellPoint _location;
        string _id = System.Guid.NewGuid().ToString();
        bool _isExplored;
        bool _isRevealed;
        bool _isPhysicallyVisible;
        Compass _doors = Compass.Null;
        Compass _walls = Compass.Null;

        public string Id { get { return _id; } }

        int _northCounter = 0;
        int _southCounter = 0;
        int _eastCounter = 0;
        int _westCounter = 0;

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
        public bool IsRevealed 
        {
            get { return _isRevealed; }
            set
            {
                _isRevealed = value;
                this.Stroke = value ? Brushes.White : Brushes.Blue;
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
        public bool IsDoor
        {
            get { return this.Doors != Compass.Null; }
        }
        public Compass Walls
        {
            get { return _walls; }
            set
            {
                _walls = value;
                _definingGeometry = null;
                InvalidateVisual();
            }
        }
        public Compass Doors
        {
            get { return _doors; }
            set
            {
                _doors = value;
                _definingGeometry = null;
                InvalidateVisual();
            }
        }
        public CellPoint Location 
        {
            get { return _location; }
            set
            {
                if (value != null)
                {
                    Canvas.SetLeft(this, value.Column * ScenarioConfiguration.CELLWIDTH);
                    Canvas.SetTop(this, value.Row * ScenarioConfiguration.CELLHEIGHT);
                }
                _location = value;
            }
        }

        public void ToggleDoor(Compass direction)
        {
            switch (direction)
            {
                case Compass.N:
                    if (this.NorthDoorSearchCounter <= 0)
                        this.Doors &= ~Compass.N;
                    break;
                case Compass.S:
                    if (this.SouthDoorSearchCounter <= 0)
                        this.Doors &= ~Compass.S;
                    break;
                case Compass.E:
                    if (this.EastDoorSearchCounter <= 0)
                        this.Doors &= ~Compass.E;
                    break;
                case Compass.W:
                    if (this.WestDoorSearchCounter <= 0)
                        this.Doors &= ~Compass.W;
                    break;
            }
        }
        public int NorthDoorSearchCounter
        {
            get { return _northCounter; }
            set
            {
                _northCounter = value;
                InvalidateVisual();
            }
        }
        public int SouthDoorSearchCounter
        {
            get { return _southCounter; }
            set
            {
                _southCounter = value;
                InvalidateVisual();
            }
        }
        public int EastDoorSearchCounter
        {
            get { return _eastCounter; }
            set
            {
                _eastCounter = value;
                InvalidateVisual();
            }
        }
        public int WestDoorSearchCounter
        {
            get { return _westCounter; }
            set
            {
                _westCounter = value;
                InvalidateVisual();
            }
        }

        public Cell()
        {
            this.Walls = Compass.Null;
            this.Location = new CellPoint(-1, -1);
            this.IsPhysicallyVisible = false;
            this.IsExplored = false;
            this.IsRevealed = false;

            this.Stroke = Brushes.Blue;
            this.StrokeThickness = 2;
            this.Height = ScenarioConfiguration.CELLHEIGHT;
            this.Width = ScenarioConfiguration.CELLWIDTH;

            this.Fill = _fillBrush;

            this.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            this.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            Canvas.SetTop(this, 0);
            Canvas.SetLeft(this, 0);
            Canvas.SetZIndex(this, 0);
        }
        public Cell(CellPoint p, Compass walls)
        {
            this.Location = new CellPoint(p);
            this.Walls = walls;
            this.IsPhysicallyVisible = false;
            this.IsExplored = false;

            this.Stroke = Brushes.Blue;
            this.StrokeThickness = 2;
            this.Height = ScenarioConfiguration.CELLHEIGHT;
            this.Width = ScenarioConfiguration.CELLWIDTH;

            this.Fill = _fillBrush;

            this.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            this.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            Canvas.SetTop(this, 0);
            Canvas.SetLeft(this, 0);
            Canvas.SetZIndex(this, 0);
        }
        public Cell(int col, int row, Compass walls)
        {
            this.Location = new CellPoint(row, col);
            this.Walls = walls;
            this.IsPhysicallyVisible = false;
            this.IsExplored = false;

            this.Stroke = Brushes.Blue;
            this.StrokeThickness = 2;
            this.Height = ScenarioConfiguration.CELLHEIGHT;
            this.Width = ScenarioConfiguration.CELLWIDTH;

            this.Fill = _fillBrush;

            this.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            this.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            Canvas.SetTop(this, 0);
            Canvas.SetLeft(this, 0);
            Canvas.SetZIndex(this, 0);
        }
        public Cell(SerializationInfo info, StreamingContext context)
        {
            this.IsExplored = info.GetBoolean("IsExplored");
            this.IsRevealed = info.GetBoolean("IsRevealed");
            this.IsPhysicallyVisible = info.GetBoolean("IsPhysicallyVisible");

            this.Walls = (Compass)info.GetValue("Walls", typeof(Compass));
            this.Doors = (Compass)info.GetValue("Doors", typeof(Compass));
            this.Location = (CellPoint)info.GetValue("Location", typeof(CellPoint));

            this.NorthDoorSearchCounter = info.GetInt32("NorthDoorSearchCounter");
            this.SouthDoorSearchCounter = info.GetInt32("SouthDoorSearchCounter");
            this.EastDoorSearchCounter = info.GetInt32("EastDoorSearchCounter");
            this.WestDoorSearchCounter = info.GetInt32("WestDoorSearchCounter");
        }

        public override string ToString()
        {
            return this.Location == null ? "" : this.Location.ToString();
        }

        private void SetVisibility()
        {
            this.Visibility = (_isPhysicallyVisible || _isRevealed || _isExplored) ? Visibility.Visible : Visibility.Hidden;
            //this.Visibility = System.Windows.Visibility.Visible;
        }

        protected override Geometry DefiningGeometry
        {
            get
            {
                if (_definingGeometry == null)
                {
                    _definingGeometry = new PathGeometry();
                    _definingGeometry.Figures.Add(DataHelper.CreateWallsPathFigure(new Point(this.Location.Column, this.Location.Row),
                        this.Doors, this.Walls));
                }
                return _definingGeometry;
            }
        }
        public override Geometry RenderedGeometry
        {
            get
            {
                return this.DefiningGeometry;
            }
        }
        protected override Size MeasureOverride(Size constraint)
        {
            base.MeasureOverride(constraint);
            return new Size(ScenarioConfiguration.CELLWIDTH, ScenarioConfiguration.CELLHEIGHT);
        }
        protected override Size ArrangeOverride(Size finalSize)
        {
            return base.ArrangeOverride(finalSize);
        }
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            if (this.Doors != Compass.Null)
            {
                var doors = this.Doors;
                if (_northCounter > 0 && (doors & Compass.N) != 0)
                    doors &= ~Compass.N;

                if (_southCounter > 0 && (doors & Compass.S) != 0)
                    doors &= ~Compass.S;

                if (_eastCounter > 0 && (doors & Compass.E) != 0)
                    doors &= ~Compass.E;

                if (_westCounter > 0 && (doors & Compass.W) != 0)
                    doors &= ~Compass.W;

                PathGeometry pg = new PathGeometry();
                if (this.IsPhysicallyVisible)
                    pg.Figures.Add(DataHelper.CreateDoorsPathFigure(
                        new Point(this.Location.Column, 
                            this.Location.Row), doors));
                drawingContext.DrawGeometry(null, _doorPen, pg);
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("IsExplored", this.IsExplored);
            info.AddValue("IsRevealed", this.IsRevealed);
            info.AddValue("IsPhysicallyVisible", this.IsPhysicallyVisible);
            info.AddValue("Walls", this.Walls);
            info.AddValue("Doors", this.Doors);
            info.AddValue("Location", this.Location);

            info.AddValue("NorthDoorSearchCounter", this.NorthDoorSearchCounter);
            info.AddValue("SouthDoorSearchCounter", this.SouthDoorSearchCounter);
            info.AddValue("EastDoorSearchCounter", this.EastDoorSearchCounter);
            info.AddValue("WestDoorSearchCounter", this.WestDoorSearchCounter);
        }
    }
    [Serializable]
    public class LevelGrid : FrameworkElement, ISerializable
    {
        private List<Cell> _cells = new List<Cell>();
        private Cell[,] _grid;
        private CellRectangle[,] _rooms;

        public LevelGrid()
        {
            this.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            this.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            Canvas.SetTop(this, 0);
            Canvas.SetLeft(this, 0);
            Canvas.SetZIndex(this, 0);
        }
        public LevelGrid(SerializationInfo info, StreamingContext context)
        {
            var width = info.GetInt32("Width");
            var height = info.GetInt32("Height");
            var count = info.GetInt32("Length");
            var cells = new List<Cell>();
            for (int i=0;i<count;i++)
                cells.Add((Cell)info.GetValue("Cell" + i.ToString(), typeof(Cell)));

            _grid = new Cell[width, height];
            foreach (var cell in cells)
                AddCell(cell);

            this.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            this.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            Canvas.SetTop(this, 0);
            Canvas.SetLeft(this, 0);
            Canvas.SetZIndex(this, 0);
        }
        public LevelGrid(int width, int height, int roomWidth, int roomHeight)
        {
            _grid = new Cell[width, height];
            _rooms = new CellRectangle[roomWidth, roomHeight];

            this.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            this.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            Canvas.SetTop(this, 0);
            Canvas.SetLeft(this, 0);
            Canvas.SetZIndex(this, 0);
        }
        
        //Cell Accessors
        public Cell this[int x, int y]
        {
            get { return GetCell(x, y); }
            set { AddCell(value); }
        }
        public Cell GetCell(int x, int y)
        {
            if (x < 0 || x >= this.GetBounds().Right)
                return null;

            if (y < 0 || y >= this.GetBounds().Bottom)
                return null;

            return _grid[x, y];
        }
        public Cell GetCell(CellPoint cp)
        {
            return GetCell(cp.Column, cp.Row);
        }
        public Cell GetRandomCell(Random r)
        {
            int width = _grid.GetLength(0);
            int height = _grid.GetLength(1);
            Cell c = null;
            int ctr = 0;
            while (c == null && ctr < 20)
            {
                c = _grid[r.Next(0, width), r.Next(0, height)];
                ctr++;
            }
            if (c == null)
            {
                Cell[] cells = GetCellsAsArray();
                c = cells[r.Next(cells.Length)];
            }
            return c;
        }
        public CellRectangle GetRandomRoom(ref Random r)
        {
            int width = _rooms.GetLength(0);
            int height = _rooms.GetLength(1);
            CellRectangle cr = _rooms[r.Next(0, width), r.Next(0, height)];
            return cr;
        }
        public CellRectangle GetRoom(int col, int row)
        {
            return _rooms[col, row];
        }
        public int GetRoomGridWidth()
        {
            return _rooms.GetLength(0);
        }
        public int GetRoomGridHeight()
        {
            return _rooms.GetLength(1);
        }

        public void AddCell(Cell c)
        {
            if (c.Location.Column < 0 || c.Location.Column >= _grid.GetLength(0))
                return;

            if (c.Location.Row < 0 || c.Location.Row >= _grid.GetLength(1))
                return;

            if (GetCell(c.Location) == null)
            {
                _grid[c.Location.Column, c.Location.Row] = c;

                if (c.IsDoor)
                    _cells.Add(c);
                else
                    _cells.Insert(0, c);
            }
            AddVisualChild(c);
        }
        public void SetRoom(CellRectangle r, int col, int row)
        {
            _rooms[col, row] = r;
        }

        public CellRectangle GetBounds()
        {
            return new CellRectangle(new CellPoint(0, 0), _grid.GetLength(0), _grid.GetLength(1));
        }
        public Cell[] GetVisibleCells()
        {
            List<Cell> list = new List<Cell>();
            for (int i = 0; i < _cells.Count; i++)
            {
                Cell c = _cells[i] as Cell;
                if (c != null)
                {
                    if (c.IsPhysicallyVisible)
                        list.Add(c);
                }
            }
            return list.ToArray();
        }
        public Cell[] GetDoors()
        {
            List<Cell> list = new List<Cell>();
            for (int i = 0; i < _cells.Count ; i++)
            {
                Cell c = _cells[i] as Cell;
                if (c != null)
                {
                    if (c.IsDoor)
                        list.Add(c);
                }
            }
            return list.ToArray();
        }
        public Cell[] GetCellsAsArray()
        {
            List<Cell> list = new List<Cell>();
            for (int i = 0; i < _grid.GetLength(0); i++)
            {
                for (int j = 0; j < _grid.GetLength(1); j++)
                {
                    if (_grid[i,j] != null)
                        list.Add(_grid[i, j]);
                }
            }
            return list.ToArray();
        }
        public CellRectangle[] GetRoomsAsArray()
        {
            List<CellRectangle> list = new List<CellRectangle>();
            for (int i = 0; i < _rooms.GetLength(0); i++)
            {
                for (int j = 0; j < _rooms.GetLength(1); j++)
                    list.Add(_rooms[i, j]);
            }
            return list.ToArray();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var size = new Size(_grid.GetLength(0) * ScenarioConfiguration.CELLWIDTH, _grid.GetLength(1) * ScenarioConfiguration.CELLHEIGHT);
            foreach (var cell in _cells)
                cell.Measure(size);

            return size;
        }
        protected override Size ArrangeOverride(Size finalSize)
        {
            var rect = new Rect(finalSize);
            foreach (var cell in _cells)
                cell.Arrange(rect);

            return base.ArrangeOverride(finalSize);
        }
        protected override int VisualChildrenCount
        {
            get
            {
                return _cells.Count;
            }
        }
        protected override Visual GetVisualChild(int index)
        {
            return _cells[index];
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Width", _grid.GetLength(0));
            info.AddValue("Height", _grid.GetLength(1));
            info.AddValue("Length", _cells.Count);
            for (int i=0;i<_cells.Count;i++)
                info.AddValue("Cell" + i.ToString(), _cells[i]);
        }
    }
}

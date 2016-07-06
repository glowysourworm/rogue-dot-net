using Rogue.NET.Common;
using Rogue.NET.Common.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Rogue.NET.Model.Scenario
{
    [Serializable]
    public class Level : FrameworkElement, INotifyPropertyChanged, ISerializable
    {
        bool _isPlayerOnShop = false;

        public LevelGrid Grid { get; set; }
        public LayoutType Type { get; set; }

        public int Number { get; set; }
        public bool IsPlayerOnShop
        {
            get { return _isPlayerOnShop; }
            set
            {
                _isPlayerOnShop = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("IsPlayerOnShop"));
            }
        }
        public bool HasStairsUp { get; private set; }
        public bool HasStairsDown { get; private set; }
        public bool HasSavePoint { get; private set; }

        DoodadNormal _stairsUp = null;
        DoodadNormal _stairsDown = null;
        DoodadNormal _savePoint = null;
        SerializableObservableCollection<Enemy> _enemies { get; set; }
        SerializableObservableCollection<Equipment> _equipment { get; set; }
        SerializableObservableCollection<Consumable> _consumableItems { get; set; }
        SerializableObservableCollection<DoodadMagic> _doodads { get; set; }
        SerializableObservableCollection<DoodadNormal> _doodadsNormal { get; set; }

        Path _playerAura;
        GradientStop _auraStop;
        string _auraStopColorString;

        //Statistics
        public int StepsTaken { get; set; }
        public int MonsterScore { get; set; }   //Monster Killed * Experience for that monster
        public int ItemScore { get; set; }      //Item collected * Shop Value for that item
        public SerializableDictionary<string, int> MonstersKilled { get; set; }
        public SerializableDictionary<string, int> ItemsFound { get; set; }

        public Level() { } 
        public Level(SerializationInfo info, StreamingContext context)
        {
            _playerAura = new Path();

            this.Grid = (LevelGrid)info.GetValue("Grid", typeof(LevelGrid));
            this.Type = (LayoutType)info.GetValue("Type", typeof(LayoutType));

            this.Number = (int)info.GetValue("Number", typeof(int));

            this.HasStairsUp = (bool)info.GetValue("HasStairsUp", typeof(bool));
            this.HasStairsDown = (bool)info.GetValue("HasStairsDown", typeof(bool));
            this.HasSavePoint = (bool)info.GetValue("HasSavePoint", typeof(bool));

            _stairsUp = (DoodadNormal)info.GetValue("StairsUp", typeof(DoodadNormal));
            if (!this.HasStairsUp)
                _stairsUp = null;
            else
                AddVisualChild(_stairsUp);

            _stairsDown = (DoodadNormal)info.GetValue("StairsDown", typeof(DoodadNormal));
            if (!this.HasStairsDown)
                _stairsDown = null;
            else
                AddVisualChild(_stairsDown);

            _savePoint = (DoodadNormal)info.GetValue("SavePoint", typeof(DoodadNormal));
            if (!this.HasSavePoint)
                _savePoint = null;
            else
                AddVisualChild(_savePoint);

            _enemies = (SerializableObservableCollection<Enemy>)info.GetValue("Enemies", typeof(SerializableObservableCollection<Enemy>));
            _equipment = (SerializableObservableCollection<Equipment>)info.GetValue("Equipment", typeof(SerializableObservableCollection<Equipment>));
            _consumableItems = (SerializableObservableCollection<Consumable>)info.GetValue("Consumables", typeof(SerializableObservableCollection<Consumable>));
            _doodads = (SerializableObservableCollection<DoodadMagic>)info.GetValue("Doodads", typeof(SerializableObservableCollection<DoodadMagic>));
            _doodadsNormal = (SerializableObservableCollection<DoodadNormal>)info.GetValue("DoodadsNormal", typeof(SerializableObservableCollection<DoodadNormal>));

            foreach (var enemy in _enemies)
                AddVisualChild(enemy);

            foreach (var equipment in _equipment)
                AddVisualChild(equipment);

            foreach (var consumable in _consumableItems)
                AddVisualChild(consumable);

            foreach (var doodad in _doodads)
                AddVisualChild(doodad);

            foreach (var doodadNormal in _doodadsNormal)
                AddVisualChild(doodadNormal);

            AddVisualChild(this.Grid);
            AddVisualChild(_playerAura);

            this.StepsTaken = (int)info.GetValue("StepsTaken", typeof(int));
            this.MonsterScore = (int)info.GetValue("MonsterScore", typeof(int));
            this.ItemScore = (int)info.GetValue("ItemScore", typeof(int));
            this.MonstersKilled = (SerializableDictionary<string, int>)info.GetValue("MonstersKilled", typeof(SerializableDictionary<string, int>));
            this.ItemsFound = (SerializableDictionary<string, int>)info.GetValue("ItemsFound", typeof(SerializableDictionary<string, int>));
        }
        public Level(LevelGrid grid, LayoutType layoutType, int number)
        {
            _playerAura = new Path();

            this.Type = layoutType;
            this.Grid = grid;
            this.Number = number;
            this.MonstersKilled = new SerializableDictionary<string, int>();
            this.ItemsFound = new SerializableDictionary<string, int>();

            _stairsDown = new DoodadNormal();
            _stairsUp = new DoodadNormal();
            _savePoint = new DoodadNormal();

            this.HasSavePoint = false;
            this.HasStairsDown = false;
            this.HasStairsUp = false;

            _enemies = new SerializableObservableCollection<Enemy>();
            _doodads = new SerializableObservableCollection<DoodadMagic>();
            _equipment = new SerializableObservableCollection<Equipment>();
            _doodadsNormal = new SerializableObservableCollection<DoodadNormal>();
            _consumableItems = new SerializableObservableCollection<Consumable>();

            this.Margin = new Thickness(0);
            this.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            this.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            Canvas.SetTop(this, 0);
            Canvas.SetLeft(this, 0);

            Canvas.SetZIndex(_playerAura, 11);

            AddVisualChild(grid);
            AddVisualChild(_playerAura);
        }

        public void InitializeAura(Player p)
        {
            //Player aura
            var b = new RadialGradientBrush();
            _auraStop = new GradientStop((Color)ColorConverter.ConvertFromString(p.SymbolInfo.SmileyAuraColor), 0.1);
            _auraStopColorString =  p.SymbolInfo.SmileyAuraColor;
            b.GradientStops.Add(_auraStop);
            b.GradientStops.Add(new GradientStop(Colors.Transparent, 0.3));
            b.RadiusX = 1;
            b.RadiusY = 1;

            var visualPath = new Path();
            visualPath.Fill = Brushes.Black;

            var vb = new VisualBrush(visualPath);
            vb.Stretch = Stretch.None;
            vb.TileMode = TileMode.None;
            vb.AutoLayoutContent = true;
            vb.ViewportUnits = BrushMappingMode.Absolute;
            vb.ViewboxUnits = BrushMappingMode.Absolute;

            _playerAura.Fill = b;
            _playerAura.Opacity = 0.4;
            _playerAura.OpacityMask = vb;
            _playerAura.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            _playerAura.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            
            UpdatePlayerAura(p);
        }

        public void UpdatePlayerAura(Player p)
        {
            if (_auraStopColorString != p.SymbolInfo.SmileyAuraColor)
            {
                _auraStopColorString = p.SymbolInfo.SmileyAuraColor;
                _auraStop.Color = (Color)ColorConverter.ConvertFromString(_auraStopColorString);
            }
            CreatePlayerAuraFigure(p, this.Grid.GetVisibleCells(), p.GetVisualLocation());
        }

        /// <summary>
        /// Calculates aura figure based on view Point "center"
        /// </summary>
        private void CreatePlayerAuraFigure(Player p, IEnumerable<Cell> visibleCells, Point center)
        {
            //Set the opacity mask data
            var vb = _playerAura.OpacityMask as VisualBrush;
            var visualPath = vb.Visual as Path;

            var opacityMaskGroup = new GeometryGroup();
            foreach (var view in visibleCells)
                opacityMaskGroup.Children.Add(view.RenderedGeometry);

            //Size of Drawing region
            var ellipseSize = new Size(p.AuraRadius * ScenarioConfiguration.CELLHEIGHT, p.AuraRadius * ScenarioConfiguration.CELLHEIGHT);

            //Create ellipse halo not biased to the opacity mask
            var eg = new EllipseGeometry(center, ellipseSize.Width, ellipseSize.Height);

            //Gradient origin
            var pt = new Point((((ScenarioConfiguration.CELLWIDTH / 2) + center.X - eg.Bounds.Left)) / eg.Bounds.Width
            , ((ScenarioConfiguration.CELLHEIGHT / 2) + (center.Y - eg.Bounds.Top)) / eg.Bounds.Height);

            //Brush is initialized already
            var b = _playerAura.Fill as RadialGradientBrush;
            b.Center = new Point(pt.X, pt.Y);
            b.GradientOrigin = new Point(pt.X, pt.Y);

            //Ellipse geometry is data for aura path
            _playerAura.Data = eg;

            //DIDN'T SEEM TO NEED TO CALCULATE THIS
            //visualPath.Data = opacityMaskGroup.GetOutlinedPathGeometry(1, ToleranceType.Absolute);

            //Working Geometry
            if (visualPath.Data != opacityMaskGroup)
                visualPath.Data = opacityMaskGroup;

            vb.Viewport = eg.Bounds;
            vb.Viewbox = eg.Bounds;
        }

        #region Add
        public void AddStairsUp(DoodadNormal d)
        {
            if (this.HasStairsUp)
                return;

            this.HasStairsUp = true;
            _stairsUp = d;

            AddVisualChild(d);
        }
        public void AddStairsDown(DoodadNormal d)
        {
            if (this.HasStairsDown)
                return;

            this.HasStairsDown = true;
            _stairsDown = d;

            AddVisualChild(d);
        }
        public void AddSavePoint(DoodadNormal d)
        {
            if (this.HasSavePoint)
                return;

            this.HasSavePoint = true;
            _savePoint = d;

            AddVisualChild(d);
        }
        public void AddEnemy(Enemy e)
        {
            _enemies.Add(e);

            AddVisualChild(e);
        }
        public void AddDoodadMagic(DoodadMagic d)
        {
            _doodads.Add(d);

            AddVisualChild(d);
        }
        public void AddDoodadNormal(DoodadNormal d)
        {
            _doodadsNormal.Add(d);

            AddVisualChild(d);
        }
        public void AddEquipment(Equipment e)
        {
            _equipment.Add(e);

            AddVisualChild(e);
        }
        public void AddConsumable(Consumable c)
        {
            _consumableItems.Add(c);

            AddVisualChild(c);
        }
        #endregion

        #region Remove
        public void RemoveEnemy(Enemy e)
        {
            _enemies.Remove(e);

            RemoveVisualChild(e);
        }
        public void RemoveDoodadMagic(DoodadMagic d)
        {
            _doodads.Remove(d);

            RemoveVisualChild(d);
        }
        public void RemoveDoodadNormal(DoodadNormal d)
        {
            _doodadsNormal.Remove(d);

            RemoveVisualChild(d);
        }
        public void RemoveEquipment(Equipment e)
        {
            _equipment.Remove(e);

            RemoveVisualChild(e);
        }
        public void RemoveConsumable(Consumable c)
        {
            _consumableItems.Remove(c);

            RemoveVisualChild(c);
        }
        #endregion

        #region Get
        public IEnumerable<ScenarioObject> GetEverything()
        {
            var list = new List<ScenarioObject>();
            foreach (ScenarioObject e in _enemies)
                list.Add(e);

            foreach (ScenarioObject e in _doodads)
                list.Add(e);

            foreach (ScenarioObject e in _equipment)
                list.Add(e);

            foreach (ScenarioObject e in _doodadsNormal)
                list.Add(e);

            foreach (ScenarioObject e in _consumableItems)
                list.Add(e);

            if (this.HasSavePoint)
                list.Add(_savePoint);

            if (this.HasStairsDown)
                list.Add(_stairsDown);

            if (this.HasStairsUp)
                list.Add(_stairsUp);
            return list;
        }
        public Enemy[] GetEnemies()
        {
            return _enemies.ToArray();
        }
        public DoodadMagic[] GetMagicDoodads()
        {
            return _doodads.ToArray();
        }
        public DoodadNormal[] GetNormalDoodads()
        {
            return _doodadsNormal.ToArray();
        }
        public Equipment[] GetEquipment()
        {
            return _equipment.ToArray();
        }
        public Consumable[] GetConsumables()
        {
            return _consumableItems.ToArray();
        }
        public DoodadNormal GetStairsUp()
        {
            return _stairsUp;
        }
        public DoodadNormal GetStairsDown()
        {
            return _stairsDown;
        }
        public DoodadNormal GetSavePoint()
        {
            return _savePoint;
        }
        #endregion

        public void ProcessVisibility(Cell[] visibleCells)
        {
            foreach (Enemy e in _enemies)
            {
                e.IsPhysicallyVisible = visibleCells.Any(c => c.Location.Equals(e.Location));
                if (e.IsPhysicallyVisible)
                    e.IsRevealed = false;
            }
            foreach (Equipment e in _equipment)
            {
                e.IsPhysicallyVisible = visibleCells.Any(c => c.Location.Equals(e.Location));
                if (e.IsPhysicallyVisible)
                    e.IsRevealed = false;
            }
            foreach (Consumable e in _consumableItems)
            {
                e.IsPhysicallyVisible = visibleCells.Any(c => c.Location.Equals(e.Location));
                if (e.IsPhysicallyVisible)
                    e.IsRevealed = false;
            }
            foreach (DoodadMagic e in _doodads)
            {
                e.IsPhysicallyVisible = visibleCells.Any(c => c.Location.Equals(e.Location));
                if (e.IsPhysicallyVisible)
                {
                    e.IsRevealed = false;
                    e.IsExplored = true;
                }
            }
            foreach (DoodadNormal e in _doodadsNormal)
            {
                e.IsPhysicallyVisible = visibleCells.Any(c => c.Location.Equals(e.Location));
                if (e.IsPhysicallyVisible)
                {
                    e.IsRevealed = false;
                    e.IsExplored = true;
                }
            }
            if (this.HasSavePoint)
            {
                _savePoint.IsPhysicallyVisible = visibleCells.Any(c => c.Location.Equals(_savePoint.Location));
                if (_savePoint.IsPhysicallyVisible)
                {
                    _savePoint.IsRevealed = false;
                    _savePoint.IsExplored = true;
                }
            }
            if (this.HasStairsDown)
            {
                _stairsDown.IsPhysicallyVisible = visibleCells.Any(c => c.Location.Equals(_stairsDown.Location));
                if (_stairsDown.IsPhysicallyVisible)
                {
                    _stairsDown.IsRevealed = false;
                    _stairsDown.IsExplored = true;
                }
            }
            if (this.HasStairsUp)
            {
                _stairsUp.IsPhysicallyVisible = visibleCells.Any(c => c.Location.Equals(_stairsUp.Location));
                if (_stairsUp.IsPhysicallyVisible)
                {
                    _stairsUp.IsRevealed = false;
                    _stairsUp.IsExplored = true;
                }
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            _playerAura.Measure(availableSize);
            this.Grid.Measure(availableSize);

            foreach (var thing in GetEverything())
                thing.Measure(availableSize);

            return this.Grid.DesiredSize;
        }
        protected override Size ArrangeOverride(Size finalSize)
        {
            var rect = new Rect(finalSize);
            _playerAura.Arrange(rect);

            this.Grid.Arrange(rect);

            foreach (var thing in GetEverything())
                thing.Arrange(rect);

            return base.ArrangeOverride(finalSize);
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index == 0)
                return _playerAura;

            else if (index == 1)
                return this.Grid;

            else
                return GetEverything().ToList()[index - 2];
        }
        protected override int VisualChildrenCount
        {
            get
            {
                return 2 + GetEverything().Count();
            }
        }
        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);

            InvalidateVisual();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Grid", this.Grid);
            info.AddValue("Type", this.Type);

            info.AddValue("Number", this.Number);

            info.AddValue("HasStairsUp", this.HasStairsUp);
            info.AddValue("HasStairsDown", this.HasStairsDown);
            info.AddValue("HasSavePoint", this.HasSavePoint);
            info.AddValue("StairsUp", this.HasStairsUp ? _stairsUp : new DoodadNormal());
            info.AddValue("StairsDown", this.HasStairsUp ? _stairsDown : new DoodadNormal());
            info.AddValue("SavePoint", this.HasStairsUp ? _savePoint : new DoodadNormal());

            info.AddValue("Enemies", _enemies);
            info.AddValue("Equipment", _equipment);
            info.AddValue("Consumables", _consumableItems);
            info.AddValue("Doodads", _doodads);
            info.AddValue("DoodadsNormal", _doodadsNormal);

            info.AddValue("StepsTaken", this.StepsTaken);
            info.AddValue("MonsterScore", this.MonsterScore);
            info.AddValue("ItemScore", this.ItemScore);
            info.AddValue("MonstersKilled", this.MonstersKilled);
            info.AddValue("ItemsFound", this.ItemsFound);
        }
    }
}

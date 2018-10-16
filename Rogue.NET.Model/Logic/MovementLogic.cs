using System;
using System.Linq;
using System.Windows;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Drawing;
using System.Windows.Media;
using System.Threading.Tasks;
using Rogue.NET.Common.Collections;
using Rogue.NET.Scenario.Model;
using Rogue.NET.Common;
using Rogue.NET.Model.Scenario;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Prism.PubSubEvents;
using Rogue.NET.Model.Physics;

namespace Rogue.NET.Model.Logic
{
    public class MovementLogic : LogicBase
    {
        readonly IRayTracer _rayTracer;

        public MovementLogic(IEventAggregator eventAggregator, IUnityContainer unityContainer, IRayTracer rayTracer)
            : base(eventAggregator, unityContainer)
        {
            _rayTracer = rayTracer;
        }

        protected override void OnLevelLoaded(PlayerStartLocation location)
        {
            PlacePlayer(location);
        }

        #region Nested Classes
        public class PathCell
        {
            public int PathIndex { get; private set; }
            public Cell CurrentCell { get; private set; }
            public Cell NextCell { get; private set; }

            public PathCell(int pathIdx, Cell currentCell, Cell nextCell)
            {
                this.PathIndex = pathIdx;
                this.CurrentCell = currentCell;
                this.NextCell = nextCell;
            }
            public override bool Equals(object obj)
            {
                return this.CurrentCell == null ? base.Equals(obj) : this.CurrentCell.Equals(obj);
            }
            public override int GetHashCode()
            {
                return this.CurrentCell == null ? base.GetHashCode() : this.CurrentCell.GetHashCode();
            }
            public override string ToString()
            {
                return this.CurrentCell.ToString();
            }
        }
        #endregion

        public void PlacePlayer(PlayerStartLocation location)
        {
            switch (location)
            {
                case PlayerStartLocation.StairsDown:
                    if (this.Level.HasStairsDown)
                        this.Player.Location = this.Level.GetStairsDown().Location;
                    break;
                case PlayerStartLocation.StairsUp:
                    if (this.Level.HasStairsUp)
                        this.Player.Location = this.Level.GetStairsUp().Location;
                    break;
                case PlayerStartLocation.Random:
                    this.Player.Location = Helper.GetRandomCellPoint(this.Level,  this.Random);
                    break;
            }

            ProcessExploredCells();

            // notify listeners that player has been placed and visibility has been processed
            PublishInitializedEvent();

            this.Level.InitializeAura(this.Player);
        }

        #region Character Movement / Path Finding
        public Cell CalculateCharacterMove(IEnumerable<CharacterStateType> states, CharacterMovementType type, double disengageRadius, CellPoint location, CellPoint target)
        {
            if (states.Any(z => z == CharacterStateType.Confused))
            {
                //Return random if confused
                List<Cell> adjCells = Helper.GetAdjacentCells(location.Row, location.Column, this.Level.Grid);
                return adjCells[this.Random.Next(0, adjCells.Count)];
            }

            switch (type)
            {
                case CharacterMovementType.Random:
                    List<Cell> adjCells = Helper.GetAdjacentCells(location.Row, location.Column, this.Level.Grid);
                    return adjCells[this.Random.Next(0, adjCells.Count)];
                case CharacterMovementType.HeatSeeker:
                    return FindMinDistCell(location, target);
                case CharacterMovementType.StandOffIsh:
                    return FindMaxDistCell(location, target);
                case CharacterMovementType.PathFinder:
                    int maxCtr = 50;
                    Dictionary<string, PathCell> pathDictionary = new Dictionary<string, PathCell>(1000);
                    Cell ce = FindPath(disengageRadius, pathDictionary, location, location, target, 1, maxCtr);
                    return (ce == null) ? FindMinDistCell(location, target) : ce;
            }
            return null;
        }
        /// <summary>
        /// Assumes Doors are open - if closed, will have to open when character gets there
        /// </summary>
        private Cell FindPath(double maxSeparation, Dictionary<string, PathCell> pathDictionary, CellPoint start, CellPoint current, CellPoint end, int pathIdx, int maxCtr)
        {
            if (pathIdx > maxSeparation)
                return null;

            Cell currentCell = this.Level.Grid.GetCell(current);
            List<Cell> adjCells = Helper.GetAdjacentCells(current.Row, current.Column, this.Level.Grid);
            List<Cell> cellsFound = FindPathAdjacentCells(maxSeparation, current, end, adjCells);

            cellsFound.Sort(Comparer<Cell>.Create(new Comparison<Cell>((x, y) =>
                {
                    double dist_x = Helper.RoguianDistance(x.Location, end);
                    double dist_y = Helper.RoguianDistance(y.Location, end);

                    return dist_x.CompareTo(dist_y);
                })));

            for (int i=cellsFound.Count - 1;i>=0;i--)
            {
                Cell foundCell = cellsFound[i];
                if (!pathDictionary.ContainsKey(foundCell.Id))
                    pathDictionary.Add(foundCell.Id, new PathCell(pathIdx, currentCell, foundCell));

                else
                    cellsFound.RemoveAt(i);

                if (foundCell.Location.Equals(end))
                {
                    //Backtrack to start location
                    Cell backTrackCell = this.Level.Grid.GetCell(end);
                    while (!backTrackCell.Location.Equals(start))
                    {
                        PathCell path = pathDictionary.Values.FirstOrDefault(pathCell => pathCell.NextCell == backTrackCell);
                        if (path == null)
                            return null;

                        backTrackCell = path.CurrentCell;
                        if (backTrackCell.Location.Equals(start))
                            return path.NextCell;
                    }
                }
            }

            //have to pass on main thread to add to dictionary
            foreach (Cell foundCell in cellsFound)
            {
                Cell nextInPath = FindPath(maxSeparation, pathDictionary, start, foundCell.Location, end, pathIdx + 1, maxCtr);
                if (nextInPath != null)
                    return nextInPath;
            }
            return null;
        }

        private List<Cell> FindPathAdjacentCells(double maxSeparation, CellPoint current, CellPoint end, List<Cell> adjCells)
        {
            List<Cell> cellsFound = new List<Cell>();
            //adjCells.Sort(new CellComparer(end));
            foreach (Cell c in adjCells)
            {
                if (c.Location.Equals(current))
                    continue;

                if (Helper.EuclidianDistance(c.Location, end) > maxSeparation)
                    continue;

                //CellPoint pt = CellPoint.Empty;
                //Compass cs = Compass.Null;
                //bool throughDoor = LayoutHelper.IsCellThroughDoor(current, c.Location, this.Level, out pt, out cs);

                //Set character melee flag to true to indicate that characters will be fighting as a possible outcome
                bool isBlocked = Helper.IsPathToAdjacentCellBlocked(current, c.Location, this.Level, this.Player, true, true);

                if (isBlocked /*&& !throughDoor*/)
                    continue;

                cellsFound.Add(c);
            }
            return cellsFound;
        }

        //For use with character movement
        public Cell FindMaxDistCell(CellPoint p1, CellPoint p2)
        {
            List<Cell> adjCells = Helper.GetAdjacentCells(p1.Row, p1.Column, this.Level.Grid);
            Cell maxDistCell = null;
            CellPoint openingPosition = CellPoint.Empty;
            double max = double.MinValue;
            foreach (Cell c in adjCells)
            {
                if (Helper.IsCellOccupied(c.Location, this.Level, this.Player, true))
                    continue;

                //if (CharacterMovementEngine.IsCellThroughDoor(p1, c.Location, l, out openingPosition, out cs))
                //    continue;

                double x = Helper.RoguianDistance(c.Location, p2);
                if (x > max)
                {
                    max = x;
                    maxDistCell = c;
                }
            }
            return maxDistCell;
        }
        //For use with character movement
        public Cell FindMinDistCell(CellPoint p1, CellPoint p2)
        {
            List<Cell> adjCells = Helper.GetAdjacentCells(p1.Row, p1.Column, this.Level.Grid);
            Cell minDistCell = null;
            double min = double.MaxValue;
            foreach (Cell c in adjCells)
            {
                if (Helper.IsCellOccupied(c.Location, this.Level, this.Player, true))
                    continue;

                //if (CharacterMovementEngine.IsPathToAdjacentCellBlocked(p1, c.Location, l, true, false))
                //    continue;

                double x = Helper.RoguianDistance(c.Location, p2);
                if (x < min)
                {
                    min = x;
                    minDistCell = c;
                }
            }
            return minDistCell;
        }
        #endregion

        #region Visible Cells Calculation
        public List<Cell> GetLogicallyVisibleCells(Level l, CellPoint p, double maxCellRadius)
        {
            List<Cell> logVisCells = new List<Cell>();
            logVisCells.Add(l.Grid.GetCell(p));
            PointF origin = TransformToPhysicalLayout(p, ScenarioConfiguration.CELLHEIGHT, ScenarioConfiguration.CELLWIDTH);
            origin.X += ScenarioConfiguration.CELLWIDTH / 2;
            origin.Y += ScenarioConfiguration.CELLHEIGHT / 2;
            int angle = 0;

            while (angle < 360)
            {
                bool hitWall = false;
                double mag = 0;
                CellPoint pt = new CellPoint(p);
                while (!hitWall)
                {
                    if (Helper.EuclidianDistance(pt, p) > maxCellRadius)
                        break;

                    Cell current = l.Grid.GetCell(pt.Column, pt.Row);
                    if (angle < 90)
                    {
                        CellPoint n = new CellPoint(pt.Row - 1, pt.Column);
                        CellPoint e = new CellPoint(pt.Row, pt.Column + 1);

                        bool enterCell = false;
                        while (!enterCell)
                        {
                            mag += 1;
                            PointF rp = GetPointOnRay(origin, angle, mag, ScenarioConfiguration.CELLHEIGHT / (double)ScenarioConfiguration.CELLWIDTH);
                            CellPoint cp = TransformFromPhysicalLayout(rp, ScenarioConfiguration.CELLHEIGHT, ScenarioConfiguration.CELLWIDTH);
                            if (!l.Grid.GetBounds().Contains(cp))
                            {
                                hitWall = true;
                                break;
                            }

                            if (n == cp)
                            {
                                Cell ce = l.Grid.GetCell(n.Column, n.Row);
                                if (ce != null)
                                {
                                    pt.Row--;
                                    hitWall = ((ce.Walls & Compass.S) != 0) || ((current.Walls & Compass.N) != 0 || (ce.Doors & Compass.S) != 0 || (current.Doors & Compass.N) != 0);
                                    if (!hitWall && !logVisCells.Contains(ce))
                                        logVisCells.Add(ce);
                                }
                                else
                                    hitWall = true;
                                enterCell = true;
                            }
                            else if (e == cp)
                            {
                                Cell ce = l.Grid.GetCell(e.Column, e.Row);
                                if (ce != null)
                                {
                                    pt.Column++;
                                    hitWall = ((ce.Walls & Compass.W) != 0) || ((current.Walls & Compass.E) != 0 || (ce.Doors & Compass.W) != 0) || ((current.Doors & Compass.E) != 0);
                                    if (!hitWall && !logVisCells.Contains(ce))
                                        logVisCells.Add(ce);
                                }
                                else
                                    hitWall = true;
                                enterCell = true;
                            }
                        }
                    }
                    else if (angle < 180)
                    {
                        CellPoint n = new CellPoint(pt.Row - 1, pt.Column);
                        CellPoint w = new CellPoint(pt.Row, pt.Column - 1);

                        bool enterCell = false;
                        while (!enterCell)
                        {
                            mag += 1;
                            PointF rp = GetPointOnRay(origin, angle, mag, ScenarioConfiguration.CELLHEIGHT / (double)ScenarioConfiguration.CELLWIDTH);
                            CellPoint cp = TransformFromPhysicalLayout(rp, ScenarioConfiguration.CELLHEIGHT, ScenarioConfiguration.CELLWIDTH);
                            if (!l.Grid.GetBounds().Contains(cp))
                            {
                                hitWall = true;
                                break;
                            }
                            if (n == cp)
                            {
                                Cell ce = l.Grid.GetCell(n.Column, n.Row);
                                if (ce != null)
                                {
                                    pt.Row--;
                                    hitWall = ((ce.Walls & Compass.S) != 0) || ((current.Walls & Compass.N) != 0 || (ce.Doors & Compass.S) != 0) || ((current.Doors & Compass.N) != 0);
                                    if (!hitWall && !logVisCells.Contains(ce))
                                        logVisCells.Add(ce);
                                }
                                else
                                    hitWall = true;
                                enterCell = true;
                            }
                            else if (w == cp)
                            {
                                Cell ce = l.Grid.GetCell(w.Column, w.Row);
                                if (ce != null)
                                {
                                    pt.Column--;
                                    hitWall = ((ce.Walls & Compass.E) != 0) || ((current.Walls & Compass.W) != 0 || (ce.Doors & Compass.E) != 0) || ((current.Doors & Compass.W) != 0);
                                    if (!hitWall && !logVisCells.Contains(ce))
                                        logVisCells.Add(ce);
                                }
                                else
                                    hitWall = true;
                                enterCell = true;
                            }
                        }
                    }
                    else if (angle < 270)
                    {
                        CellPoint s = new CellPoint(pt.Row + 1, pt.Column);
                        CellPoint w = new CellPoint(pt.Row, pt.Column - 1);

                        bool enterCell = false;
                        while (!enterCell)
                        {
                            mag += 1;
                            PointF rp = GetPointOnRay(origin, angle, mag, ScenarioConfiguration.CELLHEIGHT / (double)ScenarioConfiguration.CELLWIDTH);
                            CellPoint cp = TransformFromPhysicalLayout(rp, ScenarioConfiguration.CELLHEIGHT, ScenarioConfiguration.CELLWIDTH);
                            if (!l.Grid.GetBounds().Contains(cp))
                            {
                                hitWall = true;
                                break;
                            }
                            if (s == cp)
                            {
                                Cell ce = l.Grid.GetCell(s.Column, s.Row);
                                if (ce != null)
                                {
                                    pt.Row++;
                                    hitWall = ((ce.Walls & Compass.N) != 0) || ((current.Walls & Compass.S) != 0 || (ce.Doors & Compass.N) != 0) || ((current.Doors & Compass.S) != 0);
                                    if (!hitWall && !logVisCells.Contains(ce))
                                        logVisCells.Add(ce);
                                }
                                else
                                    hitWall = true;
                                enterCell = true;
                            }
                            else if (w == cp)
                            {
                                Cell ce = l.Grid.GetCell(w.Column, w.Row);
                                if (ce != null)
                                {
                                    pt.Column--;
                                    hitWall = ((ce.Walls & Compass.E) != 0) || ((current.Walls & Compass.W) != 0 || (ce.Doors & Compass.E) != 0) || ((current.Doors & Compass.W) != 0);
                                    if (!hitWall && !logVisCells.Contains(ce))
                                        logVisCells.Add(ce);
                                }
                                else
                                    hitWall = true;
                                enterCell = true;
                            }
                        }
                    }
                    else
                    {
                        CellPoint s = new CellPoint(pt.Row + 1, pt.Column);
                        CellPoint e = new CellPoint(pt.Row, pt.Column + 1);

                        bool enterCell = false;
                        while (!enterCell)
                        {
                            mag += 1;
                            PointF rp = GetPointOnRay(origin, angle, mag, ScenarioConfiguration.CELLHEIGHT / (double)ScenarioConfiguration.CELLWIDTH);
                            CellPoint cp = TransformFromPhysicalLayout(rp, ScenarioConfiguration.CELLHEIGHT, ScenarioConfiguration.CELLWIDTH);
                            if (!l.Grid.GetBounds().Contains(cp))
                            {
                                hitWall = true;
                                break;
                            }
                            if (s == cp)
                            {
                                Cell ce = l.Grid.GetCell(s.Column, s.Row);
                                if (ce != null)
                                {
                                    pt.Row++;
                                    hitWall = ((ce.Walls & Compass.N) != 0) || ((current.Walls & Compass.S) != 0 || (ce.Doors & Compass.N) != 0) || ((current.Doors & Compass.S) != 0);
                                    if (!hitWall && !logVisCells.Contains(ce))
                                        logVisCells.Add(ce);
                                }
                                else
                                    hitWall = true;
                                enterCell = true;
                            }
                            else if (e == cp)
                            {
                                Cell ce = l.Grid.GetCell(e.Column, e.Row);
                                if (ce != null)
                                {
                                    pt.Column++;
                                    hitWall = ((ce.Walls & Compass.W) != 0) || ((current.Walls & Compass.E) != 0 || (ce.Doors & Compass.W) != 0) || ((current.Doors & Compass.E) != 0);
                                    if (!hitWall && !logVisCells.Contains(ce))
                                        logVisCells.Add(ce);
                                }
                                else
                                    hitWall = true;
                                enterCell = true;
                            }
                        }
                    }
                }
                angle += 2;
            }
            return logVisCells.Where(c=> c != null).ToList();
        }
        private CellPoint TransformFromPhysicalLayout(System.Drawing.PointF p, double cellHeight, double cellWidth)
        {
            int col = (int)(p.X / cellWidth);
            int row = (int)(p.Y / cellHeight);
            return new CellPoint(row, col);
        }
        private PointF TransformToPhysicalLayout(CellPoint p, double cellHeight, double cellWidth)
        {
            float x = (float)(cellWidth * p.Column);
            float y = (float)(cellHeight * p.Row);
            return new PointF(x, y);
        }
        /// <summary>
        /// Returns point on ray in graphics coordinates (which are flipped in y)
        /// Must map to ellipse using y to x ratio
        /// </summary>
        private PointF GetPointOnRay(System.Drawing.PointF origin, int angle, double magnitude, double yToxRatio)
        {
            System.Drawing.PointF p = new System.Drawing.PointF();
            double rad = (Math.PI / 180) * angle;
            p.X = (float)(origin.X + magnitude * Math.Cos(rad));
            p.Y = (float)(origin.Y - (yToxRatio * magnitude * Math.Sin(rad)));
            return p;
        }
        #endregion

        //Character Movement
        public ScenarioObject MoveRandom()
        {
            return Move(DataHelper.GetRandomDirection(this.Random));
        }
        public ScenarioObject Move(Compass c)
        {
            //Desired Cell
            CellPoint p = new CellPoint(this.Player.Location);
            p = Helper.AdvanceToCell(p, c);

            //Look for road blocks - move player
            if (!Helper.IsPathToAdjacentCellBlocked(this.Player.Location, p, this.Level, this.Player, true, false))
                this.Player.Location = p;

            //Increment counter
            this.Level.StepsTaken++;

            //See what the player stepped on...
            Doodad d = null;
            Consumable i = null;
            Equipment e = null;
            if (Helper.IsCharacterOnConsumable(this.Player.Location, this.Level, ref  i))
            {
                return i;
            }
            else if (Helper.IsCharacterOnEquipment(this.Player.Location, this.Level, ref e))
            {
                return e;
            }
            else if (Helper.DoesCellContainDoodad(this.Player.Location, this.Level, out d))
            {
                return d;
            }
            return null;
        }
        public void Search()
        {
            CellPoint p = this.Player.Location;
            Cell c = this.Level.Grid.GetCell(p);
            Cell n = this.Level.Grid[p.Column, p.Row - 1];
            Cell s = this.Level.Grid[p.Column, p.Row + 1];
            Cell e = this.Level.Grid[p.Column + 1, p.Row];
            Cell w = this.Level.Grid[p.Column - 1, p.Row];
            Cell ne = this.Level.Grid[p.Column + 1, p.Row - 1];
            Cell nw = this.Level.Grid[p.Column - 1, p.Row - 1];
            Cell se = this.Level.Grid[p.Column + 1, p.Row + 1];
            Cell sw = this.Level.Grid[p.Column - 1, p.Row + 1];

            if (c != null)
            {
                c.NorthDoorSearchCounter--;
                c.SouthDoorSearchCounter--;
                c.EastDoorSearchCounter--;
                c.WestDoorSearchCounter--;
            }
            if (n != null)
            {
                n.SouthDoorSearchCounter--;
                n.EastDoorSearchCounter--;
                n.WestDoorSearchCounter--;
            }
            if (s != null)
            {
                s.NorthDoorSearchCounter--;
                s.EastDoorSearchCounter--;
                s.WestDoorSearchCounter--;
            }
            if (e != null)
            {
                e.WestDoorSearchCounter--;
                e.NorthDoorSearchCounter--;
                e.SouthDoorSearchCounter--;
            }
            if (w != null)
            {
                w.EastDoorSearchCounter--;
                w.NorthDoorSearchCounter--;
                w.SouthDoorSearchCounter--;
            }

            if (ne != null)
            {
                ne.SouthDoorSearchCounter--;
                ne.WestDoorSearchCounter--;
            }
            if (nw != null)
            {
                nw.SouthDoorSearchCounter--;
                nw.EastDoorSearchCounter--;
            }
            if (se != null)
            {
                se.NorthDoorSearchCounter--;
                se.WestDoorSearchCounter--;
            }
            if (sw != null)
            {
                sw.NorthDoorSearchCounter--;
                sw.EastDoorSearchCounter--;
            }
        }
        public Cell[] ProcessExploredCells()
        {
            double lightRadius = this.Player.AuraRadius;

            //If blind - no visible cells
            IEnumerable<Cell> visibleCells = this.Player.States.Any(z => z == CharacterStateType.Blind)?
                new List<Cell>() : _rayTracer.GetLogicallyVisibleCells(this.Level.Grid, this.Player.Location, (int)lightRadius);

            Cell[] currentlyVisibleCells = this.Level.Grid.GetVisibleCells();

            foreach (Cell c in currentlyVisibleCells)
            {
                c.IsPhysicallyVisible = false;
            }

            foreach (Cell c in visibleCells)
            {
                //SHOULDN'T BE NULL!!
                if (c == null)
                    continue;

                c.IsPhysicallyVisible = true;
                c.IsExplored = true;

                //No longer have to highlight revealed cells
                c.IsRevealed = false;
            }

            //Process content visibility
            this.Level.ProcessVisibility(visibleCells.ToArray());

            //return all affected cells
            return visibleCells.Union(currentlyVisibleCells).ToArray();
        }
    }
    public class CellComparer : IComparer<Cell>
    {
        private CellPoint _comparePt = null;
        public CellComparer(CellPoint comparePoint)
        {
            _comparePt = comparePoint;
        }
        public int Compare(Cell x, Cell y)
        {
            if (x == null && y == null)
                return 0;

            else if (x == null)
                return 1;

            else if (y == null)
                return -1;

            double d1 = Helper.RoguianDistance(x.Location, _comparePt);
            double d2 = Helper.RoguianDistance(y.Location, _comparePt);
            return d1.CompareTo(d2);
        }
    }
    public class CellArrayComparer : IComparer<Cell[]>
    {
        public int Compare(Cell[] x, Cell[] y)
        {
            if (x == null && y == null)
                return 0;

            else if (x == null)
                return 1;
            else if (y == null)
                return -1;

            return x.Length.CompareTo(y.Length);
        }
    }
}

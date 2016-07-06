using Rogue.NET.Common;
using Rogue.NET.Scenario.Model;
using Rogue.NET.Model.Scenario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Rogue.NET.Model.Logic
{
    public static class Helper
    {
        //Some Basic Functions
        public static CellPoint AdvanceToCell(CellPoint pt, Compass c)
        {
            CellPoint p = new CellPoint(pt);
            switch (c)
            {
                case Compass.N: p.Row--; break;
                case Compass.S: p.Row++; break;
                case Compass.E: p.Column++; break;
                case Compass.W: p.Column--; break;
                case Compass.NE:
                    p.Column++;
                    p.Row--;
                    break;
                case Compass.NW:
                    p.Column--;
                    p.Row--;
                    break;
                case Compass.SW:
                    p.Row++;
                    p.Column--;
                    break;
                case Compass.SE:
                    p.Row++;
                    p.Column++;
                    break;
                default:
                    break;
            }
            return p;
        }
        public static bool IsInRoom(Level l, CellPoint p)
        {
            foreach (CellRectangle r in l.Grid.GetRoomsAsArray())
            {
                if (r.Contains(p))
                    return true;
            }
            return false;
        }
        public static bool IsCellOccupied(CellPoint p, Level l, Player p1, bool characterMove)
        {
            if (p.Column >= l.Grid.GetBounds().Right || p.Column < 0)
                return true;

            if (p.Row >= l.Grid.GetBounds().Bottom || p.Row < 0)
                return true;

            if (l.Grid[p.Column, p.Row] == null)
                return true;

            Doodad d;
            Item i;
            Enemy e;
            return ((DoesCellContainDoodad(p, l, out d) && !characterMove)
                || DoesCellContainEnemy(p, l, out e)
                || (DoesCellContainItem(p, l, out i) && !characterMove))
                || (p1.Location.Equals(p));
        }
        public static bool IsCellThroughDoor(CellPoint p1, CellPoint p2, Level l, out CellPoint openingPosition, out Compass openingDirection)
        {
            openingPosition = CellPoint.Empty;
            openingDirection = Compass.Null;
            Cell c1 = l.Grid.GetCell(p1);
            Cell c2 = l.Grid.GetCell(p2);
            if (c1 == null || c2 == null)
                return false;

            Compass c = GetDirectionBetweenAdjacentPoints(p1, p2);
            switch (c)
            {
                case Compass.N:
                    openingPosition = p1;
                    openingDirection = c;
                    return ((c1.Doors & Compass.N) != 0) && ((c2.Doors & Compass.S) != 0);
                case Compass.S:
                    openingPosition = p1;
                    openingDirection = c;
                    return ((c1.Doors & Compass.S) != 0) && ((c2.Doors & Compass.N) != 0);
                case Compass.E:
                    openingPosition = p1;
                    openingDirection = c;
                    return ((c1.Doors & Compass.E) != 0) && ((c2.Doors & Compass.W) != 0);
                case Compass.W:
                    openingPosition = p1;
                    openingDirection = c;
                    return ((c1.Doors & Compass.W) != 0) && ((c2.Doors & Compass.E) != 0);
                case Compass.NE:
                    {
                        Cell diag1 = l.Grid.GetCell(p1.Column, p1.Row - 1);
                        Cell diag2 = l.Grid.GetCell(p1.Column + 1, p1.Row);
                        if (diag1 == null && diag2 == null)
                            return true;

                        if (diag1 != null)
                        {
                            if ((diag1.Doors & Compass.S) != 0)
                            {
                                openingPosition = p1;
                                openingDirection = Compass.N;
                            }
                            if ((c2.Doors & Compass.W) != 0)
                            {
                                openingPosition = diag1.Location;
                                openingDirection = Compass.E;
                            }
                        }
                        if (diag2 != null)
                        {
                            if ((diag2.Doors & Compass.W) != 0)
                            {
                                openingPosition = p1;
                                openingDirection = Compass.E;
                            }
                            if ((c2.Doors & Compass.S) != 0)
                            {
                                openingPosition = diag2.Location;
                                openingDirection = Compass.N;
                            }
                        }
                        return openingPosition != CellPoint.Empty;
                    }
                case Compass.NW:
                    {
                        Cell diag1 = l.Grid.GetCell(p1.Column, p1.Row - 1);
                        Cell diag2 = l.Grid.GetCell(p1.Column - 1, p1.Row);
                        if (diag1 == null && diag2 == null)
                            return true;

                        if (diag1 != null)
                        {
                            if ((diag1.Doors & Compass.S) != 0)
                            {
                                openingPosition = p1;
                                openingDirection = Compass.N;
                            }
                            if ((c2.Doors & Compass.E) != 0)
                            {
                                openingPosition = diag1.Location;
                                openingDirection = Compass.W;
                            }
                        }
                        if (diag2 != null)
                        {
                            if ((diag2.Doors & Compass.E) != 0)
                            {
                                openingPosition = p1;
                                openingDirection = Compass.W;
                            }
                            if ((c2.Doors & Compass.S) != 0)
                            {
                                openingPosition = diag2.Location;
                                openingDirection = Compass.N;
                            }
                        }
                        return openingPosition != CellPoint.Empty;
                    }
                case Compass.SE:
                    {
                        Cell diag1 = l.Grid.GetCell(p1.Column, p1.Row + 1);
                        Cell diag2 = l.Grid.GetCell(p1.Column + 1, p1.Row);
                        if (diag1 == null && diag2 == null)
                            return true;

                        if (diag1 != null)
                        {
                            if ((diag1.Doors & Compass.N) != 0)
                            {
                                openingPosition = p1;
                                openingDirection = Compass.S;
                            }
                            if ((c2.Doors & Compass.W) != 0)
                            {
                                openingPosition = diag1.Location;
                                openingDirection = Compass.E;
                            }
                        }
                        if (diag2 != null)
                        {
                            if ((diag2.Doors & Compass.W) != 0)
                            {
                                openingPosition = p1;
                                openingDirection = Compass.E;
                            }
                            if ((c2.Doors & Compass.N) != 0)
                            {
                                openingPosition = diag2.Location;
                                openingDirection = Compass.S;
                            }
                        }
                        return openingPosition != CellPoint.Empty;
                    }
                case Compass.SW:
                    {
                        Cell diag1 = l.Grid.GetCell(p1.Column, p1.Row + 1);
                        Cell diag2 = l.Grid.GetCell(p1.Column - 1, p1.Row);
                        if (diag1 == null && diag2 == null)
                            return true;

                        if (diag1 != null)
                        {
                            if ((diag1.Doors & Compass.N) != 0)
                            {
                                openingPosition = p1;
                                openingDirection = Compass.S;
                            }
                            if ((c2.Doors & Compass.E) != 0)
                            {
                                openingPosition = diag1.Location;
                                openingDirection = Compass.W;
                            }
                        }
                        if (diag2 != null)
                        {
                            if ((diag2.Doors & Compass.E) != 0)
                            {
                                openingPosition = p1;
                                openingDirection = Compass.W;
                            }
                            if ((c2.Doors & Compass.N) != 0)
                            {
                                openingPosition = diag2.Location;
                                openingDirection = Compass.S;
                            }
                        }
                        return openingPosition != CellPoint.Empty;
                    }
            }
            return false;
        }
        public static bool IsCellThroughWall(CellPoint p1, CellPoint p2, Level l)
        {
            Cell c1 = l.Grid.GetCell(p1);
            Cell c2 = l.Grid.GetCell(p2);

            if (c1 == null || c2 == null)
                return false;

            Compass c = GetDirectionBetweenAdjacentPoints(p1, p2);
            switch (c)
            {
                case Compass.N:
                    return ((c1.Walls & Compass.N) != 0) && ((c2.Walls & Compass.S) != 0);
                case Compass.S:
                    return ((c1.Walls & Compass.S) != 0) && ((c2.Walls & Compass.N) != 0);
                case Compass.E:
                    return ((c1.Walls & Compass.E) != 0) && ((c2.Walls & Compass.W) != 0);
                case Compass.W:
                    return ((c1.Walls & Compass.W) != 0) && ((c2.Walls & Compass.E) != 0);
                case Compass.NE:
                    {
                        Cell diag1 = l.Grid.GetCell(p1.Column, p1.Row - 1);
                        Cell diag2 = l.Grid.GetCell(p1.Column + 1, p1.Row);
                        if (diag1 == null && diag2 == null)
                            return true;

                        bool b1 = diag1 == null;
                        bool b2 = diag2 == null;
                        if (diag1 != null)
                        {
                            b1 |= (diag1.Walls & Compass.S) != 0;
                            b1 |= (c2.Walls & Compass.W) != 0;
                        }
                        if (diag2 != null)
                        {
                            b2 |= (diag2.Walls & Compass.W) != 0;
                            b2 |= (c2.Walls & Compass.S) != 0;
                        }
                        return b1 && b2;
                    }
                case Compass.NW:
                    {
                        Cell diag1 = l.Grid.GetCell(p1.Column, p1.Row - 1);
                        Cell diag2 = l.Grid.GetCell(p1.Column - 1, p1.Row);
                        if (diag1 == null && diag2 == null)
                            return true;

                        bool b1 = diag1 == null;
                        bool b2 = diag2 == null;
                        if (diag1 != null)
                        {
                            b1 |= (diag1.Walls & Compass.S) != 0;
                            b1 |= (c2.Walls & Compass.E) != 0;
                        }
                        if (diag2 != null)
                        {
                            b2 |= (diag2.Walls & Compass.E) != 0;
                            b2 |= (c2.Walls & Compass.S) != 0;
                        }
                        return b1 && b2;
                    }
                case Compass.SE:
                    {
                        Cell diag1 = l.Grid.GetCell(p1.Column, p1.Row + 1);
                        Cell diag2 = l.Grid.GetCell(p1.Column + 1, p1.Row);
                        if (diag1 == null && diag2 == null)
                            return true;

                        bool b1 = diag1 == null;
                        bool b2 = diag2 == null;
                        if (diag1 != null)
                        {
                            b1 |= (diag1.Walls & Compass.N) != 0;
                            b1 |= (c2.Walls & Compass.W) != 0;
                        }
                        if (diag2 != null)
                        {
                            b2 |= (diag2.Walls & Compass.W) != 0;
                            b2 |= (c2.Walls & Compass.N) != 0;
                        }
                        return b1 && b2;
                    }
                case Compass.SW:
                    {
                        Cell diag1 = l.Grid.GetCell(p1.Column, p1.Row + 1);
                        Cell diag2 = l.Grid.GetCell(p1.Column - 1, p1.Row);
                        if (diag1 == null && diag2 == null)
                            return true;

                        bool b1 = diag1 == null;
                        bool b2 = diag2 == null;
                        if (diag1 != null)
                        {
                            b1 |= (diag1.Walls & Compass.N) != 0;
                            b1 |= (c2.Walls & Compass.E) != 0;
                        }
                        if (diag2 != null)
                        {
                            b2 |= (diag2.Walls & Compass.E) != 0;
                            b2 |= (c2.Walls & Compass.N) != 0;
                        }
                        return b1 && b2;
                    }
            }
            return false;
        }
        public static bool IsPathToAdjacentCellBlocked(CellPoint p1, CellPoint p2, Level l, Player p, bool characterMovement, bool characterMelee)
        {
            Cell c1 = l.Grid.GetCell(p1);
            Cell c2 = l.Grid.GetCell(p2);

            if (c1 == null || c2 == null)
                return true;

            Compass c = GetDirectionBetweenAdjacentPoints(p1, p2);
            switch (c)
            {
                case Compass.N:
                    {
                        bool b = ((c1.Doors & Compass.N) != 0) && ((c2.Doors & Compass.S) != 0);
                        b |= ((c1.Walls & Compass.N) != 0) && ((c2.Walls & Compass.S) != 0);
                        b |= (IsCellOccupied(c2.Location, l,p,  characterMovement) && !characterMelee);
                        return b;
                    }
                case Compass.S:
                    {
                        bool b = ((c1.Doors & Compass.S) != 0) && ((c2.Doors & Compass.N) != 0);
                        b |= ((c1.Walls & Compass.S) != 0) && ((c2.Walls & Compass.N) != 0);
                        b |= (IsCellOccupied(c2.Location, l, p, characterMovement) && !characterMelee);
                        return b;
                    }
                case Compass.E:
                    {
                        bool b = ((c1.Doors & Compass.E) != 0) && ((c2.Doors & Compass.W) != 0);
                        b |= ((c1.Walls & Compass.E) != 0) && ((c2.Walls & Compass.W) != 0);
                        b |= (IsCellOccupied(c2.Location, l, p, characterMovement) && !characterMelee);
                        return b;
                    }
                case Compass.W:
                    {
                        bool b = ((c1.Doors & Compass.W) != 0) && ((c2.Doors & Compass.E) != 0);
                        b |= ((c1.Walls & Compass.W) != 0) && ((c2.Walls & Compass.E) != 0);
                        b |= (IsCellOccupied(c2.Location, l, p, characterMovement) && !characterMelee);
                        return b;
                    }
                case Compass.NE:
                    {
                        Cell diag1 = l.Grid.GetCell(p1.Column, p1.Row - 1);
                        Cell diag2 = l.Grid.GetCell(p1.Column + 1, p1.Row);
                        if (diag1 == null && diag2 == null)
                            return true;

                        bool b1 = (diag1 == null);
                        bool b2 = (diag2 == null);
                        if (diag1 != null)
                        {
                            b1 |= (diag1.Doors & Compass.S) != 0;
                            b1 |= (c2.Doors & Compass.W) != 0;
                            b1 |= (diag1.Walls & Compass.S) != 0;
                            b1 |= (c2.Walls & Compass.W) != 0;
                            //b1 |= (!characterMelee && IsCellOccupied(diag1.Location, l, characterMovement));
                        }
                        if (diag2 != null)
                        {
                            b2 |= (diag2.Doors & Compass.W) != 0;
                            b2 |= (c2.Doors & Compass.S) != 0;
                            b2 |= (diag2.Walls & Compass.W) != 0;
                            b2 |= (c2.Walls & Compass.S) != 0;
                            //b2 |= (!characterMelee && IsCellOccupied(diag2.Location, l, characterMovement));
                        }
                        return (b1 && b2) || (IsCellOccupied(p2, l, p, characterMovement) && !characterMelee);
                    }
                case Compass.NW:
                    {
                        Cell diag1 = l.Grid.GetCell(p1.Column, p1.Row - 1);
                        Cell diag2 = l.Grid.GetCell(p1.Column - 1, p1.Row);
                        if (diag1 == null && diag2 == null)
                            return true;

                        bool b1 = (diag1 == null);
                        bool b2 = (diag2 == null);
                        if (diag1 != null)
                        {
                            b1 |= (diag1.Doors & Compass.S) != 0;
                            b1 |= (c2.Doors & Compass.E) != 0;
                            b1 |= (diag1.Walls & Compass.S) != 0;
                            b1 |= (c2.Walls & Compass.E) != 0;
                            //b1 |= (!characterMelee && IsCellOccupied(diag1.Location, l, characterMovement));
                        }
                        if (diag2 != null)
                        {
                            b2 |= (diag2.Doors & Compass.E) != 0;
                            b2 |= (c2.Doors & Compass.S) != 0;
                            b2 |= (diag2.Walls & Compass.E) != 0;
                            b2 |= (c2.Walls & Compass.S) != 0;
                            //b2 |= (!characterMelee && IsCellOccupied(diag2.Location, l, characterMovement));
                        }
                        return (b1 && b2) || (IsCellOccupied(p2, l, p, characterMovement) && !characterMelee);
                    }
                case Compass.SE:
                    {
                        Cell diag1 = l.Grid.GetCell(p1.Column, p1.Row + 1);
                        Cell diag2 = l.Grid.GetCell(p1.Column + 1, p1.Row);
                        if (diag1 == null && diag2 == null)
                            return true;

                        bool b1 = (diag1 == null);
                        bool b2 = (diag2 == null);
                        if (diag1 != null)
                        {
                            b1 |= (diag1.Doors & Compass.N) != 0;
                            b1 |= (c2.Doors & Compass.W) != 0;
                            b1 |= (diag1.Walls & Compass.N) != 0;
                            b1 |= (c2.Walls & Compass.W) != 0;
                            //b1 |= (!characterMelee && IsCellOccupied(diag1.Location, l, characterMovement));
                        }
                        if (diag2 != null)
                        {
                            b2 |= (diag2.Doors & Compass.W) != 0;
                            b2 |= (c2.Doors & Compass.N) != 0;
                            b2 |= (diag2.Walls & Compass.W) != 0;
                            b2 |= (c2.Walls & Compass.N) != 0;
                            //b2 |= (!characterMelee && IsCellOccupied(diag2.Location, l, characterMovement));
                        }
                        return (b1 && b2) || (IsCellOccupied(p2, l, p, characterMovement) && !characterMelee);
                    }
                case Compass.SW:
                    {
                        Cell diag1 = l.Grid.GetCell(p1.Column, p1.Row + 1);
                        Cell diag2 = l.Grid.GetCell(p1.Column - 1, p1.Row);
                        if (diag1 == null && diag2 == null)
                            return true;

                        bool b1 = (diag1 == null);
                        bool b2 = (diag2 == null);
                        if (diag1 != null)
                        {
                            b1 |= (diag1.Doors & Compass.N) != 0;
                            b1 |= (c2.Doors & Compass.E) != 0;
                            b1 |= (diag1.Walls & Compass.N) != 0;
                            b1 |= (c2.Walls & Compass.E) != 0;
                            //b1 |= (!characterMelee && IsCellOccupied(diag1.Location, l, characterMovement));
                        }
                        if (diag2 != null)
                        {
                            b2 |= (diag2.Doors & Compass.E) != 0;
                            b2 |= (c2.Doors & Compass.N) != 0;
                            b2 |= (diag2.Walls & Compass.E) != 0;
                            b2 |= (c2.Walls & Compass.N) != 0;
                            //b2 |= (!characterMelee && IsCellOccupied(diag2.Location, l, characterMovement));
                        }
                        return (b1 && b2) || (IsCellOccupied(p2, l, p, characterMovement) && !characterMelee);
                    }
            }
            return false;
        }
        public static bool IsCharacterOnConsumable(CellPoint p, Level l,ref Consumable c)
        {
            foreach (Consumable i in l.GetConsumables())
            {
                if (i.Location.Equals(p))
                {
                    c = i;
                    return true;
                }
            }
            return false;
        }
        public static bool IsCharacterOnEquipment(CellPoint p, Level l,ref Equipment e)
        {
            foreach (Equipment i in l.GetEquipment())
            {
                if (i.Location.Equals(p))
                {
                    e = i;
                    return true;
                }
            }
            return false;
        }
        public static bool DoesCellContainItem(CellPoint p, Level l, out Item item)
        {
            item = null;
            foreach (Item i in l.GetConsumables())
            {
                if (i.Location == p)
                {
                    item = i;
                    return true;
                }
            }
            foreach (Item i in l.GetEquipment())
            {
                if (i.Location == p)
                {
                    item = i;
                    return true;
                }
            }
            return false;
        }
        public static bool DoesCellContainDoodad(CellPoint p, Level l, out Doodad dd)
        {
            dd = null;
            foreach (Doodad d in l.GetNormalDoodads())
            {
                if (p == d.Location)
                {
                    dd = d;
                    return true;
                }
            }
            foreach (Doodad d in l.GetMagicDoodads())
            {
                if (d.Location == p)
                {
                    dd = d;
                    return true;
                }
            }
            if (l.HasStairsDown)
            {
                if (l.GetStairsDown().Location == p)
                {
                    dd = l.GetStairsDown();
                    return true;
                }
            }
            if (l.HasStairsUp)
            {
                if (l.GetStairsUp().Location == p)
                {
                    dd = l.GetStairsUp();
                    return true;
                }
            }
            if (l.HasSavePoint)
            {
                if (l.GetSavePoint().Location == p)
                {
                    dd = l.GetSavePoint();
                    return true;
                }
            }
            return false;
        }
        public static bool DoesCellContainEnemy(CellPoint p, Level l, out Enemy en)
        {
            en = null;
            foreach (Enemy e in l.GetEnemies())
            {
                if (e.Location == p)
                {
                    en = e;
                    return true;
                }
            }
            return false;
        }
        public static double EuclidianDistance(CellPoint p1, CellPoint p2)
        {
            double x = p2.Column - p1.Column;
            double y = p2.Row - p1.Row;
            return Math.Sqrt((x * x) + (y * y));
        }
        public static double EuclidianDistance(Point p1, Point p2)
        {
            double x = p2.X - p1.X;
            double y = p2.Y - p1.Y;
            return Math.Sqrt((x * x) + (y * y));
        }
        public static double RoguianDistance(CellPoint p1, CellPoint p2)
        {
            double x = Math.Abs(p2.Column - p1.Column);
            double y = Math.Abs(p2.Row - p1.Row);
            return Math.Max(x, y);
        }
        public static Compass GetOppositeDirection(Compass c)
        {
            switch (c)
            {
                case Compass.N:
                    return Compass.S;
                case Compass.S:
                    return Compass.N;
                case Compass.E:
                    return Compass.W;
                case Compass.W:
                    return Compass.E;
                case Compass.NE:
                    return Compass.SW;
                case Compass.NW:
                    return Compass.SE;
                case Compass.SE:
                    return Compass.SW;
                case Compass.SW:
                    return Compass.SE;
                default:
                    return Compass.Null;
            }
        }
        public static Compass GetRandomCardinalDirection(ref Random r)
        {
            double d = r.NextDouble();
            if (d > 0.75)
                return Compass.N;

            if (d > 0.5)
                return Compass.S;

            if (d > 0.25)
                return Compass.E;

            return Compass.W;
        }
        public static Compass GetDirectionToAdjacentDoorOpeningPosition(Level lvl, CellPoint p, Compass desiredDirectionOfMovement)
        {
            List<Cell> adjCells = GetAdjacentCells(p.Row, p.Column, lvl.Grid);
            switch (desiredDirectionOfMovement)
            {
                case Compass.NE:
                    {
                        Cell c = lvl.Grid.GetCell(p.Column, p.Row - 1);
                        if (c != null)
                        {
                            if (c.IsDoor)
                                return Compass.N;
                            else
                                return Compass.E;
                        }
                        c = lvl.Grid.GetCell(p.Column + 1, p.Row);
                        if (c != null)
                            return Compass.E;
                    }
                    break;
                case Compass.SE:
                    {
                        Cell c = lvl.Grid.GetCell(p.Column, p.Row + 1);
                        if (c != null)
                        {
                            if (c.IsDoor)
                                return Compass.S;
                            else
                                return Compass.E;
                        }
                        c = lvl.Grid.GetCell(p.Column + 1, p.Row);
                        if (c != null)
                            return Compass.E;
                    }
                    break;
                case Compass.NW:
                    {
                        Cell c = lvl.Grid.GetCell(p.Column, p.Row - 1);
                        if (c != null)
                        {
                            if (c.IsDoor)
                                return Compass.N;
                            else
                                return Compass.W;
                        }
                        c = lvl.Grid.GetCell(p.Column - 1, p.Row);
                        if (c != null)
                            return Compass.W;
                    }
                    break;
                case Compass.SW:
                    {
                        Cell c = lvl.Grid.GetCell(p.Column, p.Row + 1);
                        if (c != null)
                        {
                            if (c.IsDoor)
                                return Compass.S;
                            else
                                return Compass.W;
                        }
                        c = lvl.Grid.GetCell(p.Column - 1, p.Row);
                        if (c != null)
                            return Compass.W;
                    }
                    break;
            }
            return desiredDirectionOfMovement;
        }

        //Some Level Accessor Functions
        public static List<Cell> GetAdjacentCells(int row, int col, LevelGrid dg)
        {
            List<Cell> c = new List<Cell>();
            if (row > 0)
            {
                c.Add(dg[col, row - 1]);
                if (col > 0)
                {
                    c.Add(dg[col - 1, row]);
                    c.Add(dg[col - 1, row - 1]);
                }
                if (col < dg.GetBounds().Right - 1)
                {
                    c.Add(dg[col + 1, row]);
                    c.Add(dg[col + 1, row - 1]);
                }
            }
            if (row < dg.GetBounds().Bottom - 1)
            {
                c.Add(dg[col, row + 1]);
                if (col > 0)
                    c.Add(dg[col - 1, row + 1]);

                if (col < dg.GetBounds().Right - 1)
                    c.Add(dg[col + 1, row + 1]);
            }
            c.Add(dg[col, row]);

            for (int i = c.Count - 1; i >= 0; i--)
            {
                if (c[i] == null)
                    c.RemoveAt(i);
            }
            return c;
        }
        public static Cell[] GetCadinallyAdjacentCells(CellPoint p, LevelGrid dg)
        {
            List<Cell> c = new List<Cell>();
            int row = p.Row;
            int col = p.Column;
            if (row > 0)
            {
                c.Add(dg[col, row - 1]);
                if (col > 0)
                    c.Add(dg[col - 1, row]);
                if (col < dg.GetBounds().Right - 1)
                    c.Add(dg[col + 1, row]);
            }
            if (row < dg.GetBounds().Bottom - 1)
                c.Add(dg[col, row + 1]);

            for (int i = c.Count - 1; i >= 0; i--)
            {
                if (c[i] == null)
                    c.RemoveAt(i);
            }
            return c.ToArray();
        }
        public static CellPoint GetPointInDirection(CellPoint origin, Compass dir)
        {
            switch (dir)
            {
                case Compass.N: return new CellPoint(origin.Row - 1, origin.Column);
                case Compass.S: return new CellPoint(origin.Row + 1, origin.Column);
                case Compass.E: return new CellPoint(origin.Row, origin.Column + 1);
                case Compass.W: return new CellPoint(origin.Row, origin.Column - 1);
                case Compass.NE: return new CellPoint(origin.Row - 1, origin.Column + 1);
                case Compass.NW: return new CellPoint(origin.Row - 1, origin.Column - 1);
                case Compass.SE: return new CellPoint(origin.Row + 1, origin.Column + 1);
                case Compass.SW: return new CellPoint(origin.Row + 1, origin.Column - 1);
                default: return origin;
            }
        }
        public static Compass GetDirectionBetweenAdjacentPoints(CellPoint c1, CellPoint c2)
        {
            int i = c2.Column - c1.Column;
            int j = c2.Row - c1.Row;

            if (i == -1)
            {
                switch (j)
                {
                    case -1: return Compass.NW;
                    case 0: return Compass.W;
                    case 1: return Compass.SW;
                }
            }
            if (i == 0)
            {
                switch (j)
                {
                    case -1: return Compass.N;
                    case 0: return Compass.Null;
                    case 1: return Compass.S;
                }
            }
            if (i == 1)
            {
                switch (j)
                {
                    case -1: return Compass.NE;
                    case 0: return Compass.E;
                    case 1: return Compass.SE;
                }
            }
            return Compass.Null;
        }
        public static CellPoint GetRandomCellPoint(Level l, Random r)
        {
            return l.Grid.GetRandomCell(r).Location;
        }
    }
}

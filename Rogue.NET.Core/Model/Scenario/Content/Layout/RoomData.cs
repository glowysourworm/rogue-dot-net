using System;
using System.Runtime.Serialization;

namespace Rogue.NET.Core.Model.Scenario.Content.Layout
{
    /// <summary>
    /// Serializable data structure to store calculated room information
    /// </summary>
    [Serializable]
    public class RoomData : ISerializable
    {
        public CellPoint[] Cells { get; private set; }
        public CellPoint[] EdgeCells { get; private set; }
        public CellRectangle Bounds { get; private set; }

        public RoomData(CellPoint[] cells, CellPoint[] edgeCells, CellRectangle bounds)
        {
            this.Cells = cells;
            this.EdgeCells = edgeCells;
            this.Bounds = bounds;
        }
        public RoomData(SerializationInfo info, StreamingContext context)
        {
            this.Cells = new CellPoint[info.GetInt32("CellsLength")];
            this.EdgeCells = new CellPoint[info.GetInt32("EdgeCellsLength")];
            this.Bounds = (CellRectangle)info.GetValue("Bounds", typeof(CellRectangle));

            for (int i = 0; i < this.Cells.Length; i++)
                this.Cells[i] = (CellPoint)info.GetValue("Cell" + i.ToString(), typeof(CellPoint));

            for (int i = 0; i < this.EdgeCells.Length; i++)
                this.EdgeCells[i] = (CellPoint)info.GetValue("EdgeCell" + i.ToString(), typeof(CellPoint));
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("CellsLength", this.Cells.Length);
            info.AddValue("EdgeCellsLength", this.EdgeCells.Length);
            info.AddValue("Bounds", this.Bounds);

            var counter = 0;

            foreach (var cell in this.Cells)
                info.AddValue("Cell" + counter++.ToString(), cell);

            counter = 0;

            foreach (var cell in this.EdgeCells)
                info.AddValue("EdgeCell" + counter++.ToString(), cell);
        }
    }
}

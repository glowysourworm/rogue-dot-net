using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;

using System;
using System.Runtime.Serialization;

namespace Rogue.NET.Core.Processing.Model.Extension
{
    public class GridLocator : IGridLocator
    {
        public int Column { get; set; }
        public int Row { get; set; }

        public GridLocator(int column, int row)
        {
            this.Column = column;
            this.Row = row;
        }

        public GridLocator(IGridLocator locator)
        {
            this.Column = locator.Column;
            this.Row = locator.Row;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotSupportedException();
        }
    }
}

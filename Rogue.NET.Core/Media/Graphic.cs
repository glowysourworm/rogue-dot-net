using System;
using System.Windows.Shapes;
using System.Windows.Media;

namespace Rogue.NET.Core.Media
{
    public abstract class Graphic : Shape
    {
        Geometry _g;
        protected override Geometry DefiningGeometry
        {
            get { return _g; }
        }
        public void SetGeometry(Geometry g)
        {
            _g = g;
        }
    }
}

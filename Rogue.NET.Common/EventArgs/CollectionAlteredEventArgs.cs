using Rogue.NET.Common.Collections;

namespace Rogue.NET.Common.EventArgs
{
    public class CollectionAlteredEventArgs : System.EventArgs
    {
        public object Item { get; set; }
        public CollectionAction Action { get; set; }
    }
}

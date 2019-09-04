using System;
using System.Threading.Tasks;

namespace Rogue.NET.Common.Extension.Event
{
    public delegate Task SimpleAsyncEventHandler(object sender);
    public delegate Task SimpleAsyncEventHandler<T>(T sender);
    public delegate void SimpleEventHandler();
}

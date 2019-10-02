using System;
using System.Threading.Tasks;

namespace Rogue.NET.Common.Extension.Event
{
    public delegate Task SimpleAsyncEventHandler(object sender);
    public delegate Task SimpleAsyncEventHandler<T>(T sender);
    public delegate void SimpleEventHandler();
    public delegate void SimpleEventHandler<T>(T sender);
    public delegate void SimpleEventHandler<T1, T2>(T1 item1, T2 item2);
    public delegate void SimpleEventHandler<T1, T2, T3>(T1 item1, T2 item2, T3 item3);
}

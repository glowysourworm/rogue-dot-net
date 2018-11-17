using System;
using Rogue.NET.Utility;

namespace Rogue.NET
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            MapperInit.Initialize();

            var application = new RogueApplication();

            application.Run();
        }
    }
}

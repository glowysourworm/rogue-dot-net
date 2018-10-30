using System;

namespace Rogue.NET
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            var application = new RogueApplication();

            application.Run();
        }
    }
}

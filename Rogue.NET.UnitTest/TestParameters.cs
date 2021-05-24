using Rogue.NET.Common.Utility;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.UnitTest
{
    /// <summary>
    /// Simple parameters for setting up and running the tests (directory locations, output paths, etc..)
    /// </summary>
    public static class TestParameters
    {
        public static string SavedGameDirectory = Path.Combine(Assembly.GetExecutingAssembly().Location, "..\\..\\", ResourceConstants.SavedGameDirectory);
        public static string ScenarioDirectory = Path.Combine(Assembly.GetExecutingAssembly().Location, "..\\..\\", ResourceConstants.ScenarioDirectory);
        public static string TempDirectory = Path.Combine(Assembly.GetExecutingAssembly().Location, "..\\..\\", ResourceConstants.TempDirectory);
        public static string DebugOutputDirectory = Path.Combine(Assembly.GetExecutingAssembly().Location, "..\\..\\", ResourceConstants.DebugOutputDirectory);

    }
}

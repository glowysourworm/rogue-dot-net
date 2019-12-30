using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Math.Algorithm
{
    /// <summary>
    /// Provides an integer factorial 
    /// </summary>
    public static class FactorialAlgorithm
    {
        /// <summary>
        /// TODO: Use Prime Swing algorithm.
        /// </summary>
        public static int Run(int n)
        {
            if (n < 0)
                throw new ArgumentException("Trying to take factorial of negative number");

            if (n == 0)
                return 1;

            else
                return n * Run(n - 1);
        }
    }
}

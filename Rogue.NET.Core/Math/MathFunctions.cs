using System;
using System.Linq;

namespace Rogue.NET.Core.Math
{
    public static class MathFunctions
    {
        /// <summary>
        /// TODO: Use Prime Swing algorithm.
        /// </summary>
        public static int Factorial(int integer)
        {
            if (integer < 0)
                throw new ArgumentException("Trying to take factorial of negative number");

            if (integer == 0)
                return 1;

            else
                return integer * Factorial(integer - 1);
        }

        public static double Max(params double[] numbers)
        {
            if (!numbers.Any())
                throw new ArgumentException("Trying to run MathFunctions.Max with no numbers");

            return numbers.Max();
        }

        public static double Min(params double[] numbers)
        {
            if (!numbers.Any())
                throw new ArgumentException("Trying to run MathFunctions.Min with no numbers");

            return numbers.Min();
        }

        public static int Max(params int[] numbers)
        {
            if (!numbers.Any())
                throw new ArgumentException("Trying to run MathFunctions.Max with no numbers");

            return numbers.Max();
        }

        public static int Min(params int[] numbers)
        {
            if (!numbers.Any())
                throw new ArgumentException("Trying to run MathFunctions.Min with no numbers");

            return numbers.Min();
        }

        public static int Abs(int number)
        {
            return System.Math.Abs(number);
        }
    }
}

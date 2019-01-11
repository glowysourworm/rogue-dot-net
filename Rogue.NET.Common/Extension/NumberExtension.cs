using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Common.Extension
{
    public static class NumberExtension
    {
        public static string Sign(this double number)
        {
            return number >= 0 ? "+" : "-";
        }

        public static double Abs(this double number)
        {
            return Math.Abs(number);
        }

        public static double RoundOrderMagnitudeUp(this double number)
        {
            // Round up the log_10 of the number - which gives the inverse order-of-magnitude of
            // the number (or, the number of digits in the exponent as a ceiling)
            var numberDigits = Math.Ceiling(Math.Log10(number));

            return Math.Pow(10, numberDigits);
        }
    }
}

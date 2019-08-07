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

        public static int Clip(this int number, int lowLimit = 0, int highLimit = 1)
        {
            return Math.Min(Math.Max(lowLimit, number), highLimit);
        }

        public static double Clip(this double number, double lowLimit = 0, double highLimit = 1)
        {
            return Math.Min(Math.Max(lowLimit, number), highLimit);
        }

        public static double LowLimit(this double number, double lowLimit = 0)
        {
            return Math.Max(number, lowLimit);
        }

        public static double HighLimit(this double number, double highLimit = 1)
        {
            return Math.Min(number, highLimit);
        }

        public static double RoundOrderMagnitudeUp(this double number)
        {
            // Round up the log_10 of the number - which gives the inverse order-of-magnitude of
            // the number (or, the number of digits in the exponent as a ceiling)
            var numberDigits = Math.Ceiling(Math.Log10(number));

            return Math.Pow(10, numberDigits);
        }

        /// <summary>
        /// Returns the number's ordinal string
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string ToOrdinal(this int number)
        {
            // Start with the most common extension.
            string extension = "th";

            // Examine the last 2 digits.
            int last_digits = number % 100;

            // If the last digits are 11, 12, or 13, use th. Otherwise:
            if (last_digits < 11 || last_digits > 13)
            {
                // Check the last digit.
                switch (last_digits % 10)
                {
                    case 1:
                        extension = "st";
                        break;
                    case 2:
                        extension = "nd";
                        break;
                    case 3:
                        extension = "rd";
                        break;
                }
            }

            return number.ToString() + extension;
        }
    }
}

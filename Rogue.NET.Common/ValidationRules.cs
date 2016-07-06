using System;
using System.Globalization;
using System.Windows.Controls;

namespace Rogue.NET.Common
{
    public class DoubleRangeValidationRule : ValidationRule
    {
        public double Min { get; set; }
        public double Max { get; set; }
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string errmsg = "Must be a number between " + this.Min + " - " + this.Max;
            double num = 0;
            double.TryParse(value.ToString(), out num);
            if (num >= this.Min && num <= this.Max)
                return ValidationResult.ValidResult;

            else
                return new ValidationResult(false, errmsg);
        }
    }
    public class IntegerRangeValidationRule : ValidationRule
    {
        public int Min { get; set; }
        public int Max { get; set; }
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string errmsg = "Must be an integer between " + this.Min + " - " + this.Max;


            int seed = 0;
            int.TryParse(value.ToString(), out seed);
            if (seed > this.Min && seed <= this.Max)
                return ValidationResult.ValidResult;

            else
                return new ValidationResult(false, errmsg);
        }
    }
    /// <summary>
    /// Either Non-null / empty or else string matching rule
    /// </summary>
    public class StringValidationRule : ValidationRule
    {
        public bool NonNullEmptyRule { get; set; }
        public string MatchingString { get; set; }

        /// <summary>
        /// Expression used in Error Msg: "Please Enter " + [UserEntryExpression]
        /// </summary>
        public string UserEntryExpression {get;set;}
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string errmsg = "Please Enter " + this.UserEntryExpression;
            if (this.NonNullEmptyRule)
            {
                bool valid = !string.IsNullOrEmpty(value.ToString());
                return new ValidationResult(valid, errmsg);
            }
            else
            {
                bool valid = value.ToString() == this.MatchingString;
                return new ValidationResult(valid, errmsg);
            }
        }
    }
}

using Rogue.NET.Core.Logic.Static;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Rogue.NET.ScenarioEditor.Converter
{
    public class SkillPointMultiplierExampleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Binding.DoNothing;

            var skillPointMultiplier = (double)value;

            var pointsEarned = 0;
            var levelCounter = 0;

            while (levelCounter <= 30)
            {
                var experienceNextPoint = PlayerCalculator.CalculateExperienceNextSkillPoint(pointsEarned, skillPointMultiplier);
                var experienceNextLevel = PlayerCalculator.CalculateExperienceNext(levelCounter);

                // If equal, then increment both
                if (experienceNextLevel == experienceNextPoint)
                {
                    pointsEarned++;
                    levelCounter++;
                }

                // Add points until next level is reached
                if (experienceNextPoint < experienceNextLevel)
                {
                    pointsEarned++;
                }

                // (OR) Add Level and continue
                if (experienceNextLevel < experienceNextPoint)
                {
                    levelCounter++;
                }
            }

            return pointsEarned;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

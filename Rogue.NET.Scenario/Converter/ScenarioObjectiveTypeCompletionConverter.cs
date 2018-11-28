using Rogue.NET.Core.Model.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Rogue.NET.Scenario.Converter
{
    public class ScenarioObjectiveTypeCompletionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Binding.DoNothing;

            switch ((DungeonMetaDataObjectTypes)value)
            {
                case DungeonMetaDataObjectTypes.Doodad:
                    return "Seek out and use this special object once...";

                case DungeonMetaDataObjectTypes.Enemy:
                    return "Find and destroy this enemy...";

                case DungeonMetaDataObjectTypes.Item:
                    return "Find and keep this item with you...";

                case DungeonMetaDataObjectTypes.Skill:
                default:
                    throw new Exception("Unhandled Objective Type");
            }            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

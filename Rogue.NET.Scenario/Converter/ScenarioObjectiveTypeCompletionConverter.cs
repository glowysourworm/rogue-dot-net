using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Scenario.Content.ViewModel.Content;
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
            var viewModel = value as ObjectiveViewModel;

            if (viewModel == null)
                return Binding.DoNothing;

            switch (viewModel.ObjectType)
            {
                case DungeonMetaDataObjectTypes.Doodad:
                    return string.Format("Seek Out and Use \"{0}\" Once...", viewModel.RogueName);

                case DungeonMetaDataObjectTypes.Character:
                    return string.Format("Find and Destroy this Enemy \"{0}\"...", viewModel.RogueName);

                case DungeonMetaDataObjectTypes.Item:
                    return string.Format("Find and Keep this Item \"{0}\"...", viewModel.RogueName);

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

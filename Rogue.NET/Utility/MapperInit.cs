using AgileObjects.AgileMapper;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Core.Model.Scenario.Content.Skill;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using Rogue.NET.Scenario.Content.ViewModel.Content;
using Rogue.NET.Scenario.ViewModel.ItemGrid;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Rogue.NET.Utility
{
    public static class MapperInit
    {
        static MapperInit()
        {
            Mapper.WhenMapping
                  .From<Equipment>()
                  .To<EquipmentViewModel>()
                  .CreateInstancesUsing((mappingData) => new EquipmentViewModel(mappingData.Source));
        }

        public static void Initialize() { }
    }
}

using Rogue.NET.ScenarioEditor.Service.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.Difficulty.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Common.Extension;
using Rogue.NET.ScenarioEditor.ViewModel.Difficulty;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Core.Model.Scenario.Character.Extension;
using Rogue.NET.Core.Model;
using Rogue.NET.Core.Processing.Model.Static;

namespace Rogue.NET.ScenarioEditor.Service
{
    [Export(typeof(IScenarioOverviewCalculationService))]
    public class ScenarioOverviewCalculationService : IScenarioOverviewCalculationService
    {

    }
}

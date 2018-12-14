﻿using Rogue.NET.Core.Model.ScenarioConfiguration;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.Validation.Interface
{
    public interface IScenarioValidationRule
    {
        IEnumerable<IScenarioValidationMessage> Validate(ScenarioConfigurationContainer configuration);
    }
}

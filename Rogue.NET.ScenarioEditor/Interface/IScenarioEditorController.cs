﻿using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;

namespace Rogue.NET.ScenarioEditor.Interface
{
    public interface IScenarioEditorController
    {
        ScenarioConfigurationContainerViewModel CurrentConfig { get; }

        void New();
        void Open(string name, bool builtIn);
        void Save();
        void Validate();
    }
}

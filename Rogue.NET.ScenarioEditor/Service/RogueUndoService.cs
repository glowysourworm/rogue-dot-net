﻿using Prism.Events;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.Service.Interface;
using Rogue.NET.ScenarioEditor.Utility.Undo;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;
using System;
using System.ComponentModel.Composition;

namespace Rogue.NET.ScenarioEditor.Service
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IRogueUndoService))]
    public class RogueUndoService : IRogueUndoService
    {
        readonly IEventAggregator _eventAggregator;

        UndoAccumulator<ScenarioConfigurationContainerViewModel> _undoAccumulator;

        [ImportingConstructor]
        public RogueUndoService(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public event EventHandler ChangeEvent;

        public bool CanRedo()
        {
            if (_undoAccumulator == null)
                return false;

            return _undoAccumulator.CanRedo();
        }

        public bool CanUndo()
        {
            if (_undoAccumulator == null)
                return false;

            return _undoAccumulator.CanUndo();
        }

        public void Clear()
        {
            _undoAccumulator.Clear();
        }

        public void Redo()
        {
            _undoAccumulator.Redo();
        }

        public void Register(ScenarioConfigurationContainerViewModel root)
        {
            if (_undoAccumulator != null)
            {
                _undoAccumulator.UndoChangedEvent -= OnUndoChanged;
                _undoAccumulator.Unhook();
                _undoAccumulator = null;
            }

            _undoAccumulator = new UndoAccumulator<ScenarioConfigurationContainerViewModel>(root);
            _undoAccumulator.UndoChangedEvent += OnUndoChanged;
        }

        public void Undo()
        {
            _undoAccumulator.Undo();
        }

        private void OnUndoChanged(object sender, string message)
        {
            _eventAggregator.GetEvent<ScenarioEditorMessageEvent>().Publish(new ScenarioEditorMessageEventArgs()
            {
                Message = message
            });

            if (ChangeEvent != null)
                ChangeEvent(this, new EventArgs());
        }
    }
}

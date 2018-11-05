using MonitoredUndo;
using Rogue.NET.ScenarioEditor.Service.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;
using System;
using System.ComponentModel.Composition;

namespace Rogue.NET.ScenarioEditor.Service
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IRogueUndoService))]
    public class RogueUndoService : IRogueUndoService
    {
        private ScenarioConfigurationContainerViewModel _root;

        public event EventHandler ChangeEvent;

        public bool CanRedo()
        {
            if (_root == null)
                return false;

            return UndoService.Current[_root].CanRedo;
        }
        public bool CanUndo()
        {
            if (_root == null)
                return false;

            return UndoService.Current[_root].CanUndo;
        }
        public void Clear()
        {
            if (_root == null)
                throw new Exception("Scenario Configuration not registered with IRogueUndoService");

            UndoService.Current.Clear();

            UndoService.Current[_root].RedoStackChanged -= OnChange;
            UndoService.Current[_root].UndoStackChanged -= OnChange;

            _root = null;
        }
        public void Redo()
        {
            if (_root == null)
                throw new Exception("Scenario Configuration not registered with IRogueUndoService");

            UndoService.Current[_root].Redo();
        }
        public void Register(ScenarioConfigurationContainerViewModel root)
        {
            if (_root != null)
                throw new Exception("Scenario Configuration already registered with IRogueUndoService");

            UndoService.SetCurrentDocumentInstance(root);
            UndoService.Current[root].RedoStackChanged += OnChange;
            UndoService.Current[root].UndoStackChanged += OnChange;
        }
        public void Undo()
        {
            if (_root == null)
                throw new Exception("Scenario Configuration not registered with IRogueUndoService");

            UndoService.Current[_root].Undo();
        }

        protected void OnChange(object sender, EventArgs e)
        {
            if (ChangeEvent != null)
                ChangeEvent(sender, e);
        }
    }
}

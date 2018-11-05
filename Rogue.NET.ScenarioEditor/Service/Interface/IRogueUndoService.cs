using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.ScenarioEditor.Service.Interface
{
    public interface IRogueUndoService
    {
        void Register(ScenarioConfigurationContainerViewModel root);

        /// <summary>
        /// Clears all Undo / Redo stacks and registration
        /// </summary>
        void Clear();

        /// <summary>
        /// Occurs when a change is made to the Undo or Redo stack
        /// </summary>
        event EventHandler ChangeEvent;

        bool CanUndo();
        bool CanRedo();

        void Undo();
        void Redo();
    }
}

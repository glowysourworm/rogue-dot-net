using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.ScenarioEditor.Service.Interface
{
    public interface IScenarioConfigurationUndoService
    {
        void Register(ScenarioConfigurationContainerViewModel root);

        /// <summary>
        /// Clears all Undo / Redo stacks and registration
        /// </summary>
        void Clear();

        /// <summary>
        /// Prevents new changes from accumulating on the Undo Accumulator stack
        /// </summary>
        void Block();

        /// <summary>
        /// Allows new changes to be accumulated on the Undo stack
        /// </summary>
        void UnBlock();

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

using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Core.Logic.Processing.Interface;
using Rogue.NET.Core.Model.Scenario.Alteration;
using System.Collections.Generic;

namespace Rogue.NET.Core.Logic.Processing
{
    public class DialogUpdate : IDialogUpdate
    {
        public DialogEventType Type { get; set; }

        public string NoteMessage { get; set; }
        public string NoteTitle { get; set; }

        /// <summary>
        /// ***REQUIRED:  ATTACK ATTRIBUTES USED FOR IMBUE PROCESSING.
        /// </summary>
        public IEnumerable<AttackAttribute> ImbueAttackAttributes { get; set; }
    }
}

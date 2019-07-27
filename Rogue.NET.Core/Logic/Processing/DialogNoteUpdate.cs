using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Core.Logic.Processing.Interface;

namespace Rogue.NET.Core.Logic.Processing
{
    public class DialogNoteUpdate : IDialogNoteUpdate
    {
        public string NoteTitle { get; set; }
        public string NoteMessage { get; set; }
        public DialogEventType Type { get; set; }
    }
}

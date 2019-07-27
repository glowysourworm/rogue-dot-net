using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Logic.Processing.Interface
{
    public interface IDialogNoteUpdate : IDialogUpdate
    {
        string NoteTitle { get; set; }
        string NoteMessage { get; set; }
    }
}

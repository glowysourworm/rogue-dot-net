using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Processing.Service.FileDatabase.Interface
{
    /// <summary>
    /// Component that represents a directory-based file database with the primary entry list stored
    /// in the .roguedb file.
    /// </summary>
    public interface IRogueFileDatabase
    {
        /// <summary>
        /// Returns a record from the database - name should match the directory name
        /// </summary>
        RogueFileDatabaseEntry Get(string name);

        /// <summary>
        /// Returns records matching the given predicate
        /// </summary>
        IEnumerable<RogueFileDatabaseEntry> Search(Func<RogueFileDatabaseEntry, bool> predicate);

        /// <summary>
        /// Adds a record to the database - name of the directory will be the name of the entry
        /// </summary>
        RogueFileDatabaseEntry Add(string entryName, Type entryType, string baseDirectory = null);

        /// <summary>
        /// Returns true if the entry exists in the database
        /// </summary>
        bool Contains(string entryName);

        /// <summary>
        /// Deletes both the entry in memory and all contents from disk recursively
        /// </summary>
        void Delete(string entryName);
    }
}

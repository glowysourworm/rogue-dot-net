using Rogue.NET.Common.Utility;
using Rogue.NET.Core.Processing.Service.FileDatabase.Interface;

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;

namespace Rogue.NET.Core.Processing.Service
{
    /// <summary>
    /// Represents a static file database on disk. The location of the data should be the .roguedb file in
    /// the binary folder.
    /// </summary>
    public class RogueFileDatabase : IRogueFileDatabase, IDisposable
    {
        protected static IDictionary<string, RogueFileDatabaseEntry> Entries = new Dictionary<string, RogueFileDatabaseEntry>();
        protected static bool IsInitialized = false;

        public RogueFileDatabase()
        {
            if (!RogueFileDatabase.IsInitialized)
                Initialize();

            // Hook up entries
            foreach (var entry in RogueFileDatabase.Entries)
            {
                // Hook up update event
                entry.Value.RogueFileEntryUpdateEvent += Save;
            }
        }

        /// <summary>
        /// Opens / read the contents of the .roguedb file. Returns the static entries for use during initial application load ONLY.
        /// </summary>
        /// <exception cref="Exception">Throw exceptions if there are any missing files or corrupted / non-recognized entries</exception>
        public void Initialize()
        {
            // Clear static entries
            RogueFileDatabase.Entries.Clear();

            // Create initial collection / database file
            if (!File.Exists(ResourceConstants.RogueFileDatabase))
            {
                BinarySerializer.SerializeToFile(ResourceConstants.RogueFileDatabase,
                                                 RogueFileDatabase.Entries,
                                                 BinarySerializer.SerializationMode.MSFT);
            }           
            else
            {
                // Set new entries from file
                RogueFileDatabase.Entries = BinarySerializer.DeserializeFromFile<Dictionary<string, RogueFileDatabaseEntry>>(
                    ResourceConstants.RogueFileDatabase, BinarySerializer.SerializationMode.MSFT);

                // Validate Entries
                foreach (var entry in RogueFileDatabase.Entries)
                {
                    // Validate records
                    if (!entry.Value.ValidateRecords())
                        throw new Exception("Missing record found IRogueFileDatabase:  " + entry.Value.Name);

                    // Hook up update event
                    entry.Value.RogueFileEntryUpdateEvent += Save;
                }
            }

            RogueFileDatabase.IsInitialized = true;
        }

        public IEnumerable<RogueFileDatabaseEntry> Search(Func<RogueFileDatabaseEntry, bool> predicate)
        {
            return RogueFileDatabase.Entries.Values.Where(predicate);
        }

        public RogueFileDatabaseEntry Get(string entryName)
        {
            return RogueFileDatabase.Entries[entryName];
        }

        public RogueFileDatabaseEntry Add(string entryName, Type entryType, string baseDirectory = null)
        {
            if (Directory.Exists(entryName))
                throw new Exception("Trying to create duplicate RogueFileDatabase directories:  " + entryName);

            var entry = new RogueFileDatabaseEntry()
            {
                Name = entryName,
                BaseDirectory = baseDirectory ?? "",
                EntryType = entryType
            };

            // Hook up entry update event to the save process
            entry.RogueFileEntryUpdateEvent += Save;

            RogueFileDatabase.Entries.Add(entryName, entry);

            // Save entries to file
            Save();

            // CREATES DIRECTORY FOR THE ENTRY
            Directory.CreateDirectory(Path.Combine(baseDirectory ?? "", entryName));

            return entry;
        }

        public bool Contains(string entryName)
        {
            return RogueFileDatabase.Entries.ContainsKey(entryName);
        }

        public void Delete(string entryName)
        {
            // Get entry from collection
            var entry = RogueFileDatabase.Entries[entryName];

            // Remove entry prior to calling it's delete
            RogueFileDatabase.Entries.Remove(entryName);

            // Unhook update event
            entry.RogueFileEntryUpdateEvent -= Save;

            // Run delete routine on entry
            entry.Delete();

            // Delete directory for the entry
            Directory.Delete(entryName);
        }

        public void Dispose()
        {
            Save();

            // Unhook entries
            foreach (var entry in RogueFileDatabase.Entries)
                entry.Value.RogueFileEntryUpdateEvent -= Save;
        }

        private void Save()
        {
            BinarySerializer.SerializeToFile(ResourceConstants.RogueFileDatabase,
                                             RogueFileDatabase.Entries,
                                             BinarySerializer.SerializationMode.MSFT);
        }
    }
}

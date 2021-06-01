using Rogue.NET.Common.Extension.Event;
using Rogue.NET.Common.Utility;
using Rogue.NET.Core.Processing.Service.Interface;

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;

namespace Rogue.NET.Core.Processing.Service
{
    /// <summary>
    /// Represents one database entry - which is a folder containing all the
    /// files for this type
    /// </summary>
    [Serializable]
    public class RogueFileDatabaseEntry : ISerializable, IRogueFileDatabaseSerializer
    {
        /// <summary>
        /// Primary data records for the entry - NOT STORED WITH THIS ENTRY - READ FROM DIRECTORY
        /// </summary>
        protected IDictionary<string, RogueFileDatabaseRecord> Records = new Dictionary<string, RogueFileDatabaseRecord>();

        /// <summary>
        /// Event to listen for updates on the entry
        /// </summary>
        public event SimpleEventHandler RogueFileEntryUpdateEvent;

        /// <summary>
        /// The physical location of the folder on disk
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Directory offset for the entry
        /// </summary>
        public string BaseDirectory { get; set; }

        /// <summary>
        /// The type represented by the entry
        /// </summary>
        public Type EntryType { get; set; }

        public RogueFileDatabaseEntry()
        {
            this.Records = new Dictionary<string, RogueFileDatabaseRecord>();
        }

        public RogueFileDatabaseEntry(SerializationInfo info, StreamingContext context) : this()
        {
            this.Name = info.GetString("Name");
            this.BaseDirectory = info.GetString("BaseDirectory");
            this.EntryType = (Type)info.GetValue("EntryType", typeof(Type));

            if (!System.IO.Directory.Exists(CreateDirectory()))
                throw new Exception("Missing archive directory:  " + this.Name);

            var recordCount = info.GetInt32("RecordCount");

            // Add file names for each record to the database
            for (int i = 0; i < recordCount; i++)
            {
                var fileName = info.GetString("File" + i);
                var recordName = info.GetString("Record" + i);

                if (!File.Exists(fileName))
                    throw new Exception("Missing file from archive:  " + this.Name);

                // VERIFIED RECORD (it exists on file)
                this.Records.Add(recordName, new RogueFileDatabaseRecord(fileName, recordName));
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", this.Name);
            info.AddValue("BaseDirectory", this.BaseDirectory);
            info.AddValue("EntryType", this.EntryType);
            info.AddValue("RecordCount", this.Records.Count);

            // Add file names for each record to the database
            var counter = 0;
            foreach (var record in this.Records)
            {
                info.AddValue("File" + counter, record.Value.FileName);
                info.AddValue("Record" + counter++, record.Value.Name);
            }
        }

        public V Fetch<V>(string recordName)
        {
            var record = this.Records[recordName];

            return (V)record.Load<V>();
        }

        public void AddOrUpdate<V>(string recordName, V value)
        {
            // Add
            if (!this.Records.ContainsKey(recordName))
                this.Records.Add(recordName, new RogueFileDatabaseRecord(CreateFileName(recordName), recordName));

            // Update
            var record = this.Records[recordName];

            record.Save(value);

            // Fire update event
            if (this.RogueFileEntryUpdateEvent != null)
                this.RogueFileEntryUpdateEvent();
        }

        /// <summary>
        /// Removes all records from disk for the entry and leaves anything not accounted for. If directory is empty,
        /// then it is also removed
        /// </summary>
        public void Delete()
        {
            foreach (var record in this.Records.Values)
                record.Delete();

            if (Directory.GetFiles(this.Name).Length == 0)
                Directory.Delete(this.Name);

            // Fire update event
            if (this.RogueFileEntryUpdateEvent != null)
                this.RogueFileEntryUpdateEvent();
        }

        /// <summary>
        /// Checks for existance of record file
        /// </summary>
        public bool ValidateRecords()
        {
            foreach (var record in this.Records.Values)
            {
                if (!File.Exists(record.FileName))
                    return false;
            }

            return true;
        }

        protected string CreateDirectory()
        {
            if (!string.IsNullOrWhiteSpace(this.BaseDirectory))
                return Path.Combine(this.BaseDirectory, this.Name);

            else
                return this.Name;
        }

        protected string CreateFileName(string recordName)
        {
            if (!string.IsNullOrWhiteSpace(this.BaseDirectory))
                return Path.Combine(this.BaseDirectory, this.Name, recordName + "." + ResourceConstants.RogueFileDatabaseRecordExtension);

            else
                return Path.Combine(this.Name, recordName + "." + ResourceConstants.RogueFileDatabaseRecordExtension);
        }
    }
}

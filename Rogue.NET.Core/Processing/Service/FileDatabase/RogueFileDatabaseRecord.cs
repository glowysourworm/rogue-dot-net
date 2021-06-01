using Rogue.NET.Common.Utility;

using System;
using System.IO;

namespace Rogue.NET.Core.Processing.Service
{
    public class RogueFileDatabaseRecord
    {
        /// <summary>
        /// File name for the record 
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Name for the record 
        /// </summary>
        public string Name { get; private set; }

        public RogueFileDatabaseRecord(string fileName, string recordName)
        {
            this.FileName = fileName;
            this.Name = recordName;
        }

        /// <summary>
        /// Attempts to load data from disk
        /// </summary>
        /// <exception cref="Exception">Throw exception if loading fails</exception>
        public T Load<T>()
        {
            // Load bytes from file
            var buffer = File.ReadAllBytes(this.FileName);

            // Decompress the data
            buffer = ZipEncoder.Decompress(buffer);

            return BinarySerializer.Deserialize<T>(buffer, BinarySerializer.SerializationMode.RecursiveSerializer);
        }

        public void Save<T>(T newData)
        {
            // Store data to buffer
            var buffer = BinarySerializer.Serialize(newData, BinarySerializer.SerializationMode.RecursiveSerializer);

            // Compress data
            buffer = ZipEncoder.Compress(buffer);

            // Save buffer to file
            File.WriteAllBytes(this.FileName, buffer);
        }

        /// <summary>
        /// Deletes record file
        /// </summary>
        public void Delete()
        {
            if (File.Exists(this.FileName))
                File.Delete(this.FileName);
        }
    }
}

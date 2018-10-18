﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using Rogue.NET.Common;
using Rogue.NET.Scenario.Model;
using Rogue.NET.Common.Collections;
using Rogue.NET.Model.Scenario;
using System.Collections.Concurrent;

namespace Rogue.NET.Model
{
    /// <summary>
    /// Container for compressed dungeon file contents - used for lazy loading of levels into memory to
    /// minimize memory footprint
    /// </summary>
    public class ScenarioFile
    {
        public const string CONFIG = "Config";
        public const string PLAYER = "Player";
        public const string SEED = "Seed";
        public const string CURRENT_LEVEL = "Current Level";
        public const string SURVIVOR_MODE = "Survivor Mode";
        public const string OBJECTIVE_ACHEIVED = "Objective Acheived";
        public const string ENCYCLOPEDIA = "Encyclopedia";
        public const string START_TIME = "StartTime";
        public const string COMPLETED_TIME = "CompletedTime";
        public const string TICKS = "Ticks";

        /// <summary>
        /// Prepend this to level number to identify level section (LEVEL_PREVIX + level.ToString())
        /// </summary>
        public const string LEVEL_PREFIX = "Level";

        ScenarioFileHeader _header = null;
        byte[] _dungeonBuffer = null;

        private ScenarioFile(ScenarioFileHeader header, byte[] dungeonBuffer)
        {
            _header = header;
            _dungeonBuffer = dungeonBuffer;
        }

        /// <summary>
        /// Updates in memory compressed objects
        /// </summary>
        /// <exception cref="ArgumentException">throws exception if there's an error syncing dungeon object</exception>
        public void Update(ScenarioContainer dungeon)
        {
            //Stream to build new buffer to dump back to file
            MemoryStream dungeonFileStream = new MemoryStream();

            byte[] configuration = ResourceManager.SerializeToCompressedBuffer(dungeon.StoredConfig);
            byte[] player = ResourceManager.SerializeToCompressedBuffer(dungeon.Player1);
            byte[] seed = BitConverter.GetBytes(dungeon.Seed);
            byte[] currentLevel = BitConverter.GetBytes(dungeon.CurrentLevel);
            byte[] survivorMode = BitConverter.GetBytes(dungeon.SurvivorMode);
            byte[] encyclopedia = ResourceManager.SerializeToCompressedBuffer(dungeon.ItemEncyclopedia);
            byte[] objectiveAcheived = BitConverter.GetBytes(dungeon.ObjectiveAcheived);
            byte[] startTime = ResourceManager.SerializeToCompressedBuffer(dungeon.StartTime);
            byte[] completedTime = ResourceManager.SerializeToCompressedBuffer(dungeon.CompletedTime);
            byte[] ticks = ResourceManager.SerializeToCompressedBuffer(dungeon.TotalTicks);

            //Build the new header object and buffer simultaneously
            _header.StaticObjects.Clear();

            int offset = 0;
            _header.StaticObjects.Add(new IndexedObject(typeof(ScenarioConfiguration), CONFIG, offset, configuration.Length));
            dungeonFileStream.Write(configuration, 0, configuration.Length);

            offset += configuration.Length;
            _header.StaticObjects.Add(new IndexedObject(typeof(Player), PLAYER, offset, player.Length));
            dungeonFileStream.Write(player, 0, player.Length);

            offset += player.Length;
            _header.StaticObjects.Add(new IndexedObject(typeof(int), SEED, offset, seed.Length));
            dungeonFileStream.Write(seed, 0, seed.Length);

            offset += seed.Length;
            _header.StaticObjects.Add(new IndexedObject(typeof(int), CURRENT_LEVEL, offset, currentLevel.Length));
            dungeonFileStream.Write(currentLevel, 0, currentLevel.Length);

            offset += currentLevel.Length;
            _header.StaticObjects.Add(new IndexedObject(typeof(int), SURVIVOR_MODE, offset, survivorMode.Length));
            dungeonFileStream.Write(survivorMode, 0, survivorMode.Length);

            offset += survivorMode.Length;
            _header.StaticObjects.Add(new IndexedObject(typeof(SerializableDictionary<string, ScenarioMetaData>), ENCYCLOPEDIA, offset, encyclopedia.Length));
            dungeonFileStream.Write(encyclopedia, 0, encyclopedia.Length);

            offset += encyclopedia.Length;
            _header.StaticObjects.Add(new IndexedObject(typeof(bool), OBJECTIVE_ACHEIVED, offset, objectiveAcheived.Length));
            dungeonFileStream.Write(objectiveAcheived, 0, objectiveAcheived.Length);

            offset += objectiveAcheived.Length;
            _header.StaticObjects.Add(new IndexedObject(typeof(DateTime), START_TIME, offset, startTime.Length));
            dungeonFileStream.Write(startTime, 0, startTime.Length);

            offset += startTime.Length;
            _header.StaticObjects.Add(new IndexedObject(typeof(DateTime), COMPLETED_TIME, offset, completedTime.Length));
            dungeonFileStream.Write(completedTime, 0, completedTime.Length);

            offset += completedTime.Length;
            _header.StaticObjects.Add(new IndexedObject(typeof(int), TICKS, offset, ticks.Length));
            dungeonFileStream.Write(ticks, 0, ticks.Length);

            offset += ticks.Length;
            //Dynamic contents
            for (int i=1;i<=dungeon.StoredConfig.DungeonTemplate.NumberOfLevels;i++)
            {
                IndexedObject indexedLevel = _header.DynamicObjects.FirstOrDefault(z => z.Identifier == LEVEL_PREFIX + i.ToString());
                if (indexedLevel == null)
                    throw new IOException("Error saving dungeon contents to file - couldn't find indexed level");

                //Look for loaded level to replace existing level
                Level level = dungeon.LoadedLevels.FirstOrDefault(lvl => lvl.Number == i);
                if (level == null)
                {
                    //Copy contents of old buffer to new one for serialization
                    byte[] existingLevelBuffer = new byte[indexedLevel.Length];
                    Array.Copy(_dungeonBuffer, indexedLevel.Offset, existingLevelBuffer, 0, (int)indexedLevel.Length);

                    //serialize buffer to stream
                    dungeonFileStream.Write(existingLevelBuffer, 0, existingLevelBuffer.Length);

                    //just update offset of level
                    indexedLevel.Offset = offset;
                }
                else
                {
                    //Store the data for serialization
                    byte[] compressedLevel = ResourceManager.SerializeToCompressedBuffer(level);

                    //serialize buffer to stream
                    dungeonFileStream.Write(compressedLevel, 0, compressedLevel.Length);

                    //Update the offset and length of the level buffer
                    indexedLevel.Offset = offset;
                    indexedLevel.Length = compressedLevel.Length;
                }

                //update running offset
                offset += indexedLevel.Length;
            }

            _dungeonBuffer = dungeonFileStream.GetBuffer();
        }

        /// <summary>
        /// Returns a dungeon object with just the static contents unpacked (everything except the levels)
        /// </summary>
        public ScenarioContainer Unpack()
        {
            ScenarioContainer dungeon = new ScenarioContainer();

            foreach (IndexedObject indexedObject in _header.StaticObjects)
            {
                byte[] buffer = new byte[indexedObject.Length];
                Array.Copy(_dungeonBuffer, indexedObject.Offset, buffer, 0, indexedObject.Length);

                switch (indexedObject.Identifier)
                {
                    case CONFIG:
                        dungeon.StoredConfig = ResourceManager.DeserializeCompressedBuffer<ScenarioConfiguration>(buffer);
                        break;
                    case PLAYER:
                        dungeon.Player1 = ResourceManager.DeserializeCompressedBuffer<Player>(buffer);
                        break;
                    case SEED:
                        dungeon.Seed = BitConverter.ToInt32(buffer, 0);
                        break;
                    case CURRENT_LEVEL:
                        dungeon.CurrentLevel = BitConverter.ToInt32(buffer, 0);
                        break;
                    case SURVIVOR_MODE:
                        dungeon.SurvivorMode = BitConverter.ToBoolean(buffer, 0);
                        break;
                    case ENCYCLOPEDIA:
                        dungeon.ItemEncyclopedia = ResourceManager.DeserializeCompressedBuffer<SerializableDictionary<string, ScenarioMetaData>>(buffer);
                        break;
                    case OBJECTIVE_ACHEIVED:
                        dungeon.ObjectiveAcheived = BitConverter.ToBoolean(buffer, 0);
                        break;
                    case START_TIME:
                        dungeon.StartTime = ResourceManager.DeserializeCompressedBuffer<DateTime>(buffer);
                        break;
                    case COMPLETED_TIME:
                        dungeon.CompletedTime = ResourceManager.DeserializeCompressedBuffer<DateTime>(buffer);
                        break;
                    case TICKS:
                        dungeon.TotalTicks = ResourceManager.DeserializeCompressedBuffer<int>(buffer);
                        break;
                }
            }
            return dungeon;
        }

        /// <summary>
        /// Returns unpacked level object
        /// </summary>
        /// <param name="levelNumber">level number (indexed at 1)</param>
        public Level Checkout(int levelNumber)
        {
            IndexedObject indexedObject = _header.DynamicObjects.FirstOrDefault(obj => obj.Identifier == LEVEL_PREFIX + levelNumber.ToString());
            if (indexedObject == null)
                throw new ArgumentException("Level number not found in dungeon file header");

            byte[] buffer = new byte[indexedObject.Length];
            Array.Copy(_dungeonBuffer, indexedObject.Offset, buffer, 0, indexedObject.Length);
            Level level = ResourceManager.DeserializeCompressedBuffer<Level>(buffer);
            return level;
        }

        /// <summary>
        /// Saves contents of existing dungeon file to disk
        /// </summary>
        public bool Save(string file)
        {
            try
            {
                MemoryStream headerStream = new MemoryStream();
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(headerStream, _header);

                byte[] headerBuffer = headerStream.GetBuffer();
                byte[] headerLengthBuffer = BitConverter.GetBytes(headerBuffer.Length);

                using (FileStream fileStream = File.OpenWrite(file))
                {
                    fileStream.Write(headerLengthBuffer, 0, headerLengthBuffer.Length);
                    fileStream.Write(headerBuffer, 0, headerBuffer.Length);
                    fileStream.Write(_dungeonBuffer, 0, _dungeonBuffer.Length);
                }
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show("Error saving game progress" + e.Message);
                return false;
            }
        }

        /// <summary>
        /// Opens the dungeon file with all contents packed - header is available for unpacking
        /// </summary>
        public static ScenarioFile Open(string file)
        {
            try
            {
                byte[] headerBuffer = null;
                byte[] dungeonBuffer = null;

                //First 32-bits of file is the length of the header
                byte[] headerLengthBuffer = new byte[4];

                using (FileStream fileStream = File.OpenRead(file))
                {
                    //Get length of header
                    fileStream.Read(headerLengthBuffer, 0, 4);
                    int headerLength = BitConverter.ToInt32(headerLengthBuffer, 0);

                    //Read file header
                    headerBuffer = new byte[headerLength];
                    fileStream.Read(headerBuffer, 0, headerLength);

                    //create dungeon buffer
                    dungeonBuffer = new byte[fileStream.Length - headerBuffer.Length - 4];

                    //Read the rest of the contents into memory
                    fileStream.Read(dungeonBuffer, 0, (int)(fileStream.Length - headerBuffer.Length - 4));
                }

                MemoryStream headerStream = new MemoryStream(headerBuffer);
                BinaryFormatter formatter = new BinaryFormatter();
                ScenarioFileHeader header = (ScenarioFileHeader)formatter.Deserialize(headerStream);

                return new ScenarioFile(header, dungeonBuffer);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Opens just header section of file and returns
        /// </summary>
        public static ScenarioFileHeader OpenHeader(string file)
        {
            try
            {
                byte[] headerBuffer = null;

                //First 32-bits of file is the length of the header
                byte[] headerLengthBuffer = new byte[4];

                using (FileStream fileStream = File.OpenRead(file))
                {
                    //Get length of header
                    fileStream.Read(headerLengthBuffer, 0, 4);
                    int headerLength = BitConverter.ToInt32(headerLengthBuffer, 0);

                    //Read file header
                    headerBuffer = new byte[headerLength];
                    fileStream.Read(headerBuffer, 0, headerLength);
                }

                MemoryStream headerStream = new MemoryStream(headerBuffer);
                BinaryFormatter formatter = new BinaryFormatter();
                ScenarioFileHeader header = (ScenarioFileHeader)formatter.Deserialize(headerStream);
                return header;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Creates an in memory dungeon file from the given dungeon
        /// </summary>
        public static ScenarioFile Create(ScenarioContainer dungeon)
        {
            ScenarioFileHeader header = new ScenarioFileHeader();

            //Store the player's colors for showing in the intro screens
            header.SmileyLineColor = dungeon.Player1.SymbolInfo.SmileyLineColor;
            header.SmileyBodyColor = dungeon.Player1.SymbolInfo.SmileyBodyColor;

            //Stream to build new buffer to dump back to file
            MemoryStream dungeonFileStream = new MemoryStream();

            byte[] configuration = ResourceManager.SerializeToCompressedBuffer(dungeon.StoredConfig);
            //Add to allow other thread processing
            Thread.Sleep(10);

            byte[] player = ResourceManager.SerializeToCompressedBuffer(dungeon.Player1);
            //Add to allow other thread processing
            Thread.Sleep(10);

            byte[] seed = BitConverter.GetBytes(dungeon.Seed);
            byte[] currentLevel = BitConverter.GetBytes(dungeon.CurrentLevel);
            byte[] survivorMode = BitConverter.GetBytes(dungeon.SurvivorMode);
            byte[] objectiveAcheived = BitConverter.GetBytes(dungeon.ObjectiveAcheived);
            
            byte[] encyclopedia = ResourceManager.SerializeToCompressedBuffer(dungeon.ItemEncyclopedia);
            //Add to allow other thread processing
            Thread.Sleep(10);

            byte[] startTime = ResourceManager.SerializeToCompressedBuffer(dungeon.StartTime);
            //Add to allow other thread processing
            Thread.Sleep(10);

            byte[] completedTime = ResourceManager.SerializeToCompressedBuffer(dungeon.CompletedTime);
            //Add to allow other thread processing
            Thread.Sleep(10);

            byte[] ticks = ResourceManager.SerializeToCompressedBuffer(dungeon.TotalTicks);
            //Add to allow other thread processing
            Thread.Sleep(10);

            //Build the new header object and buffer simultaneously
            header.StaticObjects.Clear();

            int offset = 0;
            header.StaticObjects.Add(new IndexedObject(typeof(ScenarioConfiguration), CONFIG, offset, configuration.Length));
            dungeonFileStream.Write(configuration, 0, configuration.Length);
            //Add to allow other thread processing
            Thread.Sleep(10);

            offset += configuration.Length;
            header.StaticObjects.Add(new IndexedObject(typeof(Player), PLAYER, offset, player.Length));
            dungeonFileStream.Write(player, 0, player.Length);

            offset += player.Length;
            header.StaticObjects.Add(new IndexedObject(typeof(int), SEED, offset, seed.Length));
            dungeonFileStream.Write(seed, 0, seed.Length);

            offset += seed.Length;
            header.StaticObjects.Add(new IndexedObject(typeof(int), CURRENT_LEVEL, offset, currentLevel.Length));
            dungeonFileStream.Write(currentLevel, 0, currentLevel.Length);

            offset += currentLevel.Length;
            header.StaticObjects.Add(new IndexedObject(typeof(int), SURVIVOR_MODE, offset, survivorMode.Length));
            dungeonFileStream.Write(survivorMode, 0, survivorMode.Length);

            offset += survivorMode.Length;
            header.StaticObjects.Add(new IndexedObject(typeof(SerializableDictionary<string, ScenarioMetaData>), ENCYCLOPEDIA, offset, encyclopedia.Length));
            dungeonFileStream.Write(encyclopedia, 0, encyclopedia.Length);

            offset += encyclopedia.Length;
            header.StaticObjects.Add(new IndexedObject(typeof(bool), OBJECTIVE_ACHEIVED, offset, objectiveAcheived.Length));
            dungeonFileStream.Write(objectiveAcheived, 0, objectiveAcheived.Length);

            offset += objectiveAcheived.Length;
            header.StaticObjects.Add(new IndexedObject(typeof(DateTime), START_TIME, offset, startTime.Length));
            dungeonFileStream.Write(startTime, 0, startTime.Length);

            offset += startTime.Length;
            header.StaticObjects.Add(new IndexedObject(typeof(DateTime), COMPLETED_TIME, offset, completedTime.Length));
            dungeonFileStream.Write(completedTime, 0, completedTime.Length);

            offset += completedTime.Length;
            header.StaticObjects.Add(new IndexedObject(typeof(int), TICKS, offset, ticks.Length));
            dungeonFileStream.Write(ticks, 0, ticks.Length);

            offset += ticks.Length;

            // compress levels on multiple threads
            var compressedDictionary = new ConcurrentDictionary<int, byte[]>();
            Parallel.ForEach(dungeon.LoadedLevels, (level) =>
            {
                var data = ResourceManager.SerializeToCompressedBuffer(level);
                while (!compressedDictionary.TryAdd(level.Number,data)) Thread.Sleep(10);
            });

            // Order entries by level- add data to in memory file
            foreach (var entry in compressedDictionary.OrderBy(z => z.Key))
            {
                //Create an entry in the header
                IndexedObject indexedLevel = new IndexedObject(typeof(Level), LEVEL_PREFIX + entry.Key.ToString(), offset, entry.Value.Length);
                header.DynamicObjects.Add(indexedLevel);

                //serialize buffer to stream
                dungeonFileStream.Write(entry.Value, 0, entry.Value.Length);

                //update running offset
                offset += indexedLevel.Length;
            }

            byte[] dungeonBuffer = dungeonFileStream.GetBuffer();
            return new ScenarioFile(header, dungeonBuffer);
        }
    }

    [Serializable]
    public class ScenarioFileHeader
    {
        public string SmileyBodyColor { get; set; }
        public string SmileyLineColor { get; set; }
        public List<IndexedObject> StaticObjects { get; set; }
        public List<IndexedObject> DynamicObjects { get; set; }
        public ScenarioFileHeader()
        {
            this.StaticObjects = new List<IndexedObject>();
            this.DynamicObjects = new List<IndexedObject>();
            this.SmileyBodyColor = Colors.Yellow.ToString();
            this.SmileyLineColor = Colors.Black.ToString();
        }
    }

    [Serializable]
    public class IndexedObject
    {
        /// <summary>
        /// CLR Type of object
        /// </summary>
        public Type ObjectType { get; set; }

        /// <summary>
        /// String identifier for object (example:  "Level x")
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        /// stream index of object
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// Stream length of object
        /// </summary>
        public int Length { get; set; }

        public IndexedObject()
        {
            this.ObjectType = typeof(object);
            this.Identifier = "";
            this.Offset = -1;
            this.Length = 0;
        }
        public IndexedObject(Type objectType, string identifier, int offset, int length)
        {
            this.ObjectType = objectType;
            this.Identifier = identifier;
            this.Offset = offset;
            this.Length = length;
        }
    }
}

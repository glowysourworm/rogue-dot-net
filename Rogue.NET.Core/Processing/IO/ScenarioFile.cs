using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Collections.Concurrent;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Common.Utility;

namespace Rogue.NET.Core.Processing.IO
{
    /// <summary>
    /// Container for compressed dungeon file contents - used for lazy loading of levels into memory to
    /// minimize memory footprint
    /// </summary>
    public class ScenarioFile
    {
        public const string CONFIG = "Config";
        public const string PLAYER = "Player";
        public const string SAVELOCATION = "Save Location";
        public const string SAVELEVEL = "Save Level";
        public const string SEED = "Seed";
        public const string CURRENT_LEVEL = "Current Level";
        public const string SURVIVOR_MODE = "Survivor Mode";
        public const string ENCYCLOPEDIA = "Encyclopedia";
        public const string STATISTICS = "Statistics";

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

            byte[] configuration = BinarySerializer.SerializeAndCompress(dungeon.Configuration);
            byte[] player = BinarySerializer.SerializeAndCompress(dungeon.Player);
            byte[] saveLocation = BinarySerializer.SerializeAndCompress(dungeon.SaveLocation);
            byte[] saveLevel = BinarySerializer.SerializeAndCompress(dungeon.SaveLevel);
            byte[] seed = BitConverter.GetBytes(dungeon.Seed);
            byte[] currentLevel = BitConverter.GetBytes(dungeon.CurrentLevel);
            byte[] survivorMode = BitConverter.GetBytes(dungeon.SurvivorMode);
            byte[] encyclopedia = BinarySerializer.SerializeAndCompress(dungeon.ScenarioEncyclopedia);
            byte[] statistics = BinarySerializer.SerializeAndCompress(dungeon.Statistics);

            //Build the new header object and buffer simultaneously
            _header.StaticObjects.Clear();

            int offset = 0;
            _header.StaticObjects.Add(new IndexedObject(typeof(ScenarioConfigurationContainer), CONFIG, offset, configuration.Length));
            dungeonFileStream.Write(configuration, 0, configuration.Length);

            offset += configuration.Length;
            _header.StaticObjects.Add(new IndexedObject(typeof(Player), PLAYER, offset, player.Length));
            dungeonFileStream.Write(player, 0, player.Length);

            offset += player.Length;
            _header.StaticObjects.Add(new IndexedObject(typeof(Player), SAVELOCATION, offset, saveLocation.Length));
            dungeonFileStream.Write(saveLocation, 0, saveLocation.Length);

            offset += saveLocation.Length;
            _header.StaticObjects.Add(new IndexedObject(typeof(Player), SAVELEVEL, offset, saveLevel.Length));
            dungeonFileStream.Write(saveLevel, 0, saveLevel.Length);

            offset += saveLevel.Length;
            _header.StaticObjects.Add(new IndexedObject(typeof(int), SEED, offset, seed.Length));
            dungeonFileStream.Write(seed, 0, seed.Length);

            offset += seed.Length;
            _header.StaticObjects.Add(new IndexedObject(typeof(int), CURRENT_LEVEL, offset, currentLevel.Length));
            dungeonFileStream.Write(currentLevel, 0, currentLevel.Length);

            offset += currentLevel.Length;
            _header.StaticObjects.Add(new IndexedObject(typeof(int), SURVIVOR_MODE, offset, survivorMode.Length));
            dungeonFileStream.Write(survivorMode, 0, survivorMode.Length);

            offset += survivorMode.Length;
            _header.StaticObjects.Add(new IndexedObject(typeof(Dictionary<string, ScenarioMetaData>), ENCYCLOPEDIA, offset, encyclopedia.Length));
            dungeonFileStream.Write(encyclopedia, 0, encyclopedia.Length);

            offset += encyclopedia.Length;
            _header.StaticObjects.Add(new IndexedObject(typeof(ScenarioStatistics), STATISTICS, offset, statistics.Length));
            dungeonFileStream.Write(statistics, 0, statistics.Length);

            offset += statistics.Length;
            
            //Dynamic contents
            for (int i=1;i<=dungeon.Configuration.DungeonTemplate.NumberOfLevels;i++)
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
                    byte[] compressedLevel = BinarySerializer.SerializeAndCompress(level);

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
                        dungeon.Configuration = BinarySerializer.DeserializeAndDecompress<ScenarioConfigurationContainer>(buffer);
                        break;
                    case PLAYER:
                        dungeon.Player = BinarySerializer.DeserializeAndDecompress<Player>(buffer);
                        break;
                    case SAVELOCATION:
                        dungeon.SaveLocation = BinarySerializer.DeserializeAndDecompress<PlayerStartLocation>(buffer);
                        break;
                    case SAVELEVEL:
                        dungeon.SaveLevel = BinarySerializer.DeserializeAndDecompress<int>(buffer);
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
                        dungeon.ScenarioEncyclopedia = BinarySerializer.DeserializeAndDecompress<Dictionary<string, ScenarioMetaData>>(buffer);
                        break;
                    case STATISTICS:
                        dungeon.Statistics = BinarySerializer.DeserializeAndDecompress<ScenarioStatistics>(buffer);
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
            Level level = BinarySerializer.DeserializeAndDecompress<Level>(buffer);
            return level;
        }

        /// <summary>
        /// Saves contents of existing dungeon file to disk
        /// </summary>
        public byte[] Save()
        {
            MemoryStream headerStream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(headerStream, _header);

            byte[] headerBuffer = headerStream.GetBuffer();
            byte[] headerLengthBuffer = BitConverter.GetBytes(headerBuffer.Length);

            using (var stream = new MemoryStream())
            {
                stream.Write(headerLengthBuffer, 0, headerLengthBuffer.Length);
                stream.Write(headerBuffer, 0, headerBuffer.Length);
                stream.Write(_dungeonBuffer, 0, _dungeonBuffer.Length);

                return stream.GetBuffer();
            }
        }

        /// <summary>
        /// Opens the dungeon file with all contents packed - header is available for unpacking
        /// </summary>
        public static ScenarioFile Open(byte[] buffer)
        {
            byte[] headerBuffer = null;
            byte[] dungeonBuffer = null;

            //First 32-bits of file is the length of the header
            byte[] headerLengthBuffer = new byte[4];

            using (MemoryStream stream = new MemoryStream(buffer))
            {
                //Get length of header
                stream.Read(headerLengthBuffer, 0, 4);
                int headerLength = BitConverter.ToInt32(headerLengthBuffer, 0);

                //Read file header
                headerBuffer = new byte[headerLength];
                stream.Read(headerBuffer, 0, headerLength);

                //create dungeon buffer
                dungeonBuffer = new byte[stream.Length - headerBuffer.Length - 4];

                //Read the rest of the contents into memory
                stream.Read(dungeonBuffer, 0, (int)(stream.Length - headerBuffer.Length - 4));
            }

            MemoryStream headerStream = new MemoryStream(headerBuffer);
            BinaryFormatter formatter = new BinaryFormatter();
            ScenarioFileHeader header = (ScenarioFileHeader)formatter.Deserialize(headerStream);

            return new ScenarioFile(header, dungeonBuffer);
        }

        /// <summary>
        /// Opens just header section of file and returns
        /// </summary>
        public static ScenarioFileHeader OpenHeader(byte[] buffer)
        {
            byte[] headerBuffer = null;

            //First 32-bits of file is the length of the header
            byte[] headerLengthBuffer = new byte[4];

            using (var stream = new MemoryStream(buffer))
            {
                //Get length of header
                stream.Read(headerLengthBuffer, 0, 4);
                int headerLength = BitConverter.ToInt32(headerLengthBuffer, 0);

                //Read file header
                headerBuffer = new byte[headerLength];
                stream.Read(headerBuffer, 0, headerLength);
            }

            MemoryStream headerStream = new MemoryStream(headerBuffer);
            BinaryFormatter formatter = new BinaryFormatter();
            ScenarioFileHeader header = (ScenarioFileHeader)formatter.Deserialize(headerStream);
            return header;
        }

        /// <summary>
        /// Creates an in memory dungeon file from the given dungeon
        /// </summary>
        public static ScenarioFile Create(ScenarioContainer dungeon)
        {
            ScenarioFileHeader header = new ScenarioFileHeader();

            //Store the player's colors for showing in the intro screens
            header.SmileyLineColor = dungeon.Player.SmileyLineColor;
            header.SmileyBodyColor = dungeon.Player.SmileyBodyColor;
            header.SmileyExpression = dungeon.Player.SmileyExpression;

            //Stream to build new buffer to dump back to file
            MemoryStream dungeonFileStream = new MemoryStream();

            byte[] configuration = BinarySerializer.SerializeAndCompress(dungeon.Configuration);
            //Add to allow other thread processing
            Thread.Sleep(10);

            byte[] player = BinarySerializer.SerializeAndCompress(dungeon.Player);
            //Add to allow other thread processing
            Thread.Sleep(10);

            byte[] saveLocation = BinarySerializer.SerializeAndCompress(dungeon.SaveLocation);
            byte[] saveLevel = BinarySerializer.SerializeAndCompress(dungeon.SaveLevel);
            byte[] seed = BitConverter.GetBytes(dungeon.Seed);
            byte[] currentLevel = BitConverter.GetBytes(dungeon.CurrentLevel);
            byte[] survivorMode = BitConverter.GetBytes(dungeon.SurvivorMode);
            byte[] statistics = BinarySerializer.SerializeAndCompress(dungeon.Statistics);
            
            byte[] encyclopedia = BinarySerializer.SerializeAndCompress(dungeon.ScenarioEncyclopedia);
            //Add to allow other thread processing
            Thread.Sleep(10);

            //Build the new header object and buffer simultaneously
            header.StaticObjects.Clear();

            int offset = 0;
            header.StaticObjects.Add(new IndexedObject(typeof(ScenarioConfigurationContainer), CONFIG, offset, configuration.Length));
            dungeonFileStream.Write(configuration, 0, configuration.Length);
            //Add to allow other thread processing
            Thread.Sleep(10);

            offset += configuration.Length;
            header.StaticObjects.Add(new IndexedObject(typeof(Player), PLAYER, offset, player.Length));
            dungeonFileStream.Write(player, 0, player.Length);

            offset += player.Length;
            header.StaticObjects.Add(new IndexedObject(typeof(PlayerStartLocation), SAVELOCATION, offset, saveLocation.Length));
            dungeonFileStream.Write(saveLocation, 0, saveLocation.Length);

            offset += saveLocation.Length;
            header.StaticObjects.Add(new IndexedObject(typeof(int), SAVELEVEL, offset, saveLevel.Length));
            dungeonFileStream.Write(saveLevel, 0, saveLevel.Length);

            offset += saveLevel.Length;
            header.StaticObjects.Add(new IndexedObject(typeof(int), SEED, offset, seed.Length));
            dungeonFileStream.Write(seed, 0, seed.Length);

            offset += seed.Length;
            header.StaticObjects.Add(new IndexedObject(typeof(int), CURRENT_LEVEL, offset, currentLevel.Length));
            dungeonFileStream.Write(currentLevel, 0, currentLevel.Length);

            offset += currentLevel.Length;
            header.StaticObjects.Add(new IndexedObject(typeof(int), SURVIVOR_MODE, offset, survivorMode.Length));
            dungeonFileStream.Write(survivorMode, 0, survivorMode.Length);

            offset += survivorMode.Length;
            header.StaticObjects.Add(new IndexedObject(typeof(Dictionary<string, ScenarioMetaData>), ENCYCLOPEDIA, offset, encyclopedia.Length));
            dungeonFileStream.Write(encyclopedia, 0, encyclopedia.Length);

            offset += encyclopedia.Length;
            header.StaticObjects.Add(new IndexedObject(typeof(bool), STATISTICS, offset, statistics.Length));
            dungeonFileStream.Write(statistics, 0, statistics.Length);

            offset += statistics.Length;

            // compress levels on multiple threads
            var compressedDictionary = new ConcurrentDictionary<int, byte[]>();
            Parallel.ForEach(dungeon.LoadedLevels, (level) =>
            {
                var data = BinarySerializer.SerializeAndCompress(level);
                while (!compressedDictionary.TryAdd(level.Number, data)) Thread.Sleep(10);
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
        public SmileyExpression SmileyExpression { get; set; }
        public List<IndexedObject> StaticObjects { get; set; }
        public List<IndexedObject> DynamicObjects { get; set; }
        public ScenarioFileHeader()
        {
            this.StaticObjects = new List<IndexedObject>();
            this.DynamicObjects = new List<IndexedObject>();
            this.SmileyBodyColor = Colors.Yellow.ToString();
            this.SmileyLineColor = Colors.Black.ToString();
            this.SmileyExpression = SmileyExpression.Happy;
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

using System.IO;
using System.IO.Compression;

namespace Rogue.NET.Common.Utility
{
    /// <summary>
    /// Encodes / decodes byte arrays using the LZ78 algorithm
    /// 
    /// http://oldwww.rasip.fer.hr/research/compress/algorithms/fund/lz/lz78.html
    /// </summary>
    public static class ZipEncoder
    {
        public static byte[] Compress(byte[] buffer)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var zipStream = new GZipStream(memoryStream, CompressionMode.Compress))
                {
                    using (var inputStream = new MemoryStream(buffer))
                    {
                        inputStream.CopyTo(zipStream);
                    }
                }
                return memoryStream.ToArray();
            }
        }

        public static byte[] Decompress(byte[] buffer)
        {
            using (var memoryStream = new MemoryStream(buffer))
            {
                using (var zipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    using (var resultStream = new MemoryStream())
                    {
                        zipStream.CopyTo(resultStream);
                        zipStream.Flush();
                        zipStream.Close();
                        return resultStream.ToArray();
                    }
                }
            }
        }
    }
}

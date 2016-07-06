using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Rogue.NET.Online
{
    public class Client
    {

#if DEBUG
        private static string BASE_URL = "http://localhost:45981";
#else
        private static string BASE_URL = "http://www.roguedotnet.com";
#endif

        private static readonly byte[] KEY = new byte[] { 0x12, 0x32, 0xef, 0x45, 0x23, 0x38, 0x88, 0x09, 0x12, 0x32, 0xef, 0x45, 0x23, 0x38, 0x88, 0x09 };
        private static readonly byte[] IV = new byte[] { 0x24, 0x33, 0x24, 0xee, 0xaa, 0x3b, 0xab, 0xd9, 0x12, 0x32, 0xef, 0x45, 0x23, 0x38, 0x88, 0x09 };

        /// <summary>
        /// Returns true unless user cancels
        /// </summary>
        public bool UploadEncryptedMessage(string message, string user, string pass)
        {
            try
            {
                RijndaelManaged rijndael = new RijndaelManaged();
                rijndael.Key = KEY;
                rijndael.IV = IV;
                rijndael.Padding = PaddingMode.Zeros;

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, rijndael.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        byte[] dataToEncrypt = Encoding.UTF8.GetBytes(message);
                        cryptoStream.Write(dataToEncrypt, 0, dataToEncrypt.Length);
                    }
                    byte[] data = memoryStream.ToArray();

                    WebClient client = new WebClient();
                    client.BaseAddress = BASE_URL;
                    client.Credentials = new NetworkCredential(user, pass);
                    byte[] returnData = client.UploadData("/Home/UploadGameStats", data);
                    return returnData.Length == 0;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}

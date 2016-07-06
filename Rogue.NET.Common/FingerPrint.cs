using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Common
{
    /// <summary>
    /// This is admittedly copied from the internet... Looks like only standard cookbook to get 
    /// machine fingerprint
    /// </summary>
    public static class FingerPrint
    {
        public static string CreateComputerFingerPrint()
        {
            return GetIdentifier();
        }

        private static string GetIdentifier()
        {
            string result = "";

            ManagementClass management = new ManagementClass("WIN32_BaseBoard");
            foreach (ManagementObject mo in management.GetInstances())
            {
                result = mo["SerialNumber"].ToString();
                break;
            }
            management.Dispose();
            management = null;
            return result;
        }
    }
}

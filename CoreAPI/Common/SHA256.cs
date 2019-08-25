using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace CoreAPI.Common
{
    public class Signature_SHA256
    {
        public static string Sign(string prestr)
        {            
            prestr = prestr + "&shared_key=" + Startup.SHA256Key;
            Console.WriteLine(prestr);
            return sha256(prestr);
        }

        static string sha256(string randomString)
        {
            var crypt = new System.Security.Cryptography.SHA256Managed();
            var hash = new System.Text.StringBuilder();
            byte[] crypto = crypt.ComputeHash(System.Text.Encoding.UTF8.GetBytes(randomString));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString().ToLower();
        }
    }
}

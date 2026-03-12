using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TestLinks.Utils
{
    public static class CryptoUtils
    {

        public static string CreateMD5Hash(string input)
        {
            // Step 1, calculate MD5 hash from input
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            // Step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }
            return sb.ToString();
        }

        public static string CreateToken(string studentID, string secrete)
        {
            var str = $"{studentID}{secrete}";
            var token = CreateMD5Hash(str);
            return token;
        }

    }
}

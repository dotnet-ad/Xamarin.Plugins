using System.Text;
using Plugin.Pdf.Abstractions;
using System.Security.Cryptography;
using System;

namespace Plugin.Pdf
{
    public class Hash : IHash
    {
        public string Create(string original)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(original);

            using (var algorithm = SHA1.Create())
            {
                algorithm.ComputeHash(bytes);
                return BitConverter.ToString(algorithm.Hash);
            }
        }
    }
}
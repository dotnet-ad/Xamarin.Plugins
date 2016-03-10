using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Plugin.PdfRasterizer.Abstractions;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;

namespace Plugin.PdfRasterizer
{
    public class Hash : IHash
    {
        public string Create(string original)
        {
            var buffer = CryptographicBuffer.ConvertStringToBinary(original, BinaryStringEncoding.Utf8);
            var hashAlgorithm = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha1);
            var hashBuffer = hashAlgorithm.HashData(buffer);
            return CryptographicBuffer.EncodeToHexString(hashBuffer);
        }
    }
}
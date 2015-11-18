using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Security.Plugin.Abstractions;
using Windows.Storage.Streams;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using System.Runtime.InteropServices.WindowsRuntime;

namespace Security.Plugin
{
    public class Hash : Abstractions.IHash
    {
        private static string GetNativeAlgorithm(HashAlgorithm algorithm)
        {
            switch (algorithm)
            {
                case HashAlgorithm.Sha1: return HashAlgorithmNames.Sha1;
                case HashAlgorithm.Sha256: return HashAlgorithmNames.Sha256;
                case HashAlgorithm.Sha512: return HashAlgorithmNames.Sha512;
                case HashAlgorithm.MD5: return HashAlgorithmNames.Md5;
                default: throw new NotSupportedException("Algorithm not supported on this platform");
            }

        }

        public string Compute(HashAlgorithm algorithm, string content)
        {
            var data = Encoding.UTF8.GetBytes(content);
            var computed = this.Compute(algorithm, data);
            return Encoding.UTF8.GetString(computed);
        }

        public byte[] Compute(HashAlgorithm algorithm, byte[] content)
        {
            var nativeAlgorithm = GetNativeAlgorithm(algorithm);
            var buffer = content.AsBuffer();
            HashAlgorithmProvider hashAlgorithm = HashAlgorithmProvider.OpenAlgorithm(nativeAlgorithm);
            IBuffer hashBuffer = hashAlgorithm.HashData(buffer);
            return hashBuffer.ToArray();
        }
    }
}

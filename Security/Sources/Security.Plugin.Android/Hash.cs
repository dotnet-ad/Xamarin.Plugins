using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Security.Cryptography;

namespace Security.Plugin
{
    public class Hash : Abstractions.IHash
    {
		private static HashAlgorithm GetNativeAlgorithm(Security.Plugin.Abstractions.HashAlgorithm algorithm)
        {
            switch (algorithm)
            {
				case Security.Plugin.Abstractions.HashAlgorithm.Sha1: return SHA1.Create();
				case Security.Plugin.Abstractions.HashAlgorithm.Sha256: return SHA256.Create();
				case Security.Plugin.Abstractions.HashAlgorithm.Sha512: return SHA512.Create();
				case Security.Plugin.Abstractions.HashAlgorithm.MD5: return MD5.Create();
                default: throw new NotSupportedException("Algorithm not supported on this platform");
            }
        }

		public string Compute(Security.Plugin.Abstractions.HashAlgorithm algorithm, string content)
        {
            var data = Encoding.UTF8.GetBytes(content);
            var computed = this.Compute(algorithm, data);
            return Encoding.UTF8.GetString(computed);
        }

		public byte[] Compute(Security.Plugin.Abstractions.HashAlgorithm algorithm, byte[] content)
        {
            var nativeAlgorithm = GetNativeAlgorithm(algorithm);
			return nativeAlgorithm.ComputeHash(content);
        }
    }
}
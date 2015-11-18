using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Security.Plugin.Abstractions
{
    public interface IHash
    {
        byte[] Compute(HashAlgorithm algorithm, byte[] content);

        string Compute(HashAlgorithm algorithm, string content);
    }
}

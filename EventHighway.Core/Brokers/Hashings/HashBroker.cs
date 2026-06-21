// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Security.Cryptography;
using System.Text;

namespace EventHighway.Core.Brokers.Hashings
{
    internal class HashBroker : IHashBroker
    {
        public string GenerateSha256Hash(string content)
        {
            byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(content));

            return Convert.ToHexStringLower(bytes);
        }
    }
}

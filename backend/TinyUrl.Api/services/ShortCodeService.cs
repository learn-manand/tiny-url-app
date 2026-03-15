using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace TinyUrl.Api.services
{
    public class ShortCodeService
    {
        private const string AllowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        public static string GenerateCode(int length = 6)
        {
            var chars = new char[length];
            var buffer = new byte[length];

            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(buffer);

            for (int i = 0; i < length; i++)
            {
                chars[i] = AllowedChars[buffer[i] % AllowedChars.Length];
            }

            return new string(chars);
        }
    }
}

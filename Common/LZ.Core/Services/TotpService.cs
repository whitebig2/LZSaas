using LZ.Core.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Core.Services
{
    public class TotpService: ITotpService,ISingletonDependency
    {
        private const int CodeLength = 6;
        private const int TimeStep = 30;
        private const int Window = 1;
        private readonly int _window = Window;
        private readonly HashAlgorithmName _hashAlgorithm = HashAlgorithmName.SHA1;
        public string GenerateCode(string base32Secret)
        {
            var key = DecodeBase32(base32Secret);
            var counter = GetTimeCounter(DateTime.UtcNow);
            return GenerateCode(key, counter);
        }

        public bool ValidateCode(string base32Secret, string code, int? window = null)
        {
            var effectiveWindow = window ?? _window;
            var key = DecodeBase32(base32Secret);
            var currentCounter = GetTimeCounter(DateTime.UtcNow);

            for (int i = -effectiveWindow; i <= 0; i++)
            {
                var counter = currentCounter + i;
                var expected = GenerateCode(key, counter);
                if (expected == code) return true;
            }
            return false;
        }

        public string GenerateSecretKey(int byteLength = 20)
        {
            var bytes = RandomNumberGenerator.GetBytes(byteLength);
            return Base32Encoding.ToString(bytes);
        }

        private static long GetTimeCounter(DateTime now)
        {
            var unixTime = (long)(now.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
            return unixTime / TimeStep;
        }

        private static string GenerateCode(byte[] key, long counter)
        {
            var counterBytes = BitConverter.GetBytes(counter);
            if (BitConverter.IsLittleEndian) Array.Reverse(counterBytes);

            //using var hmac = _hashAlgorithm == HashAlgorithmName.SHA256
            //    ? new HMACSHA256(key)
            //    : new HMACSHA1(key);


            using var hmac = new HMACSHA1(key);

            var hash = hmac.ComputeHash(counterBytes);

            var offset = hash[^1] & 0x0F;
            var binary =
                ((hash[offset] & 0x7F) << 24) |
                ((hash[offset + 1] & 0xFF) << 16) |
                ((hash[offset + 2] & 0xFF) << 8) |
                (hash[offset + 3] & 0xFF);

            var code = binary % (int)Math.Pow(10, CodeLength);
            return code.ToString($"D{CodeLength}");
        }

        private static byte[] DecodeBase32(string base32)
        {
            var output = new List<byte>();
            var buffer = 0;
            var bitsRemaining = 0;
            const string base32Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

            foreach (var c in base32.ToUpperInvariant().Where(base32Chars.Contains))
            {
                var value = base32Chars.IndexOf(c);
                buffer = (buffer << 5) | value;
                bitsRemaining += 5;

                if (bitsRemaining >= 8)
                {
                    output.Add((byte)(buffer >> (bitsRemaining - 8)));
                    bitsRemaining -= 8;
                }
            }
            return output.ToArray();
        }
    }
    public static class Base32Encoding
    {
        private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

        public static string ToString(byte[] data)
        {
            var sb = new StringBuilder((data.Length * 8 + 4) / 5);
            int buffer = data.Length > 0 ? data[0] : 0;
            int next = 1;
            int bitsLeft = 8;

            while (bitsLeft > 0 || next < data.Length)
            {
                if (next < data.Length)
                {
                    buffer <<= 8;
                    buffer |= data[next++];
                    bitsLeft += 8;
                }

                int index = (buffer >> (bitsLeft - 5)) & 0x1F;
                bitsLeft -= 5;
                sb.Append(Alphabet[index]);
            }
            return sb.ToString();
        }
    }
}

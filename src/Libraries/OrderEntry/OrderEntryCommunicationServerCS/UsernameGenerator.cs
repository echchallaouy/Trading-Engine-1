using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace TradingEngineServer.OrderEntryCommunicationServer
{
    sealed class UsernameGenerator
    {
        private static readonly char[] chars =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();

        public static string GenerateRandomUsername(int size)
        {
            byte[] data = new byte[4 * size];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
                crypto.GetBytes(data);
            StringBuilder result = new StringBuilder(size);
            for (int startLocation = 0; startLocation < size; startLocation++)
            {
                var rnd = BitConverter.ToUInt32(data, startLocation * 4);
                var idx = rnd % chars.Length;
                result.Append(chars[idx]);
            }

            return result.ToString();
        }
    }
}

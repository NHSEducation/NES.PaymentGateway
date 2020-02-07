using System.Security.Cryptography;
using System.Text;

namespace Common.Helpers
{
    public class KeyGenerator
    {
        #region singleton

        private KeyGenerator()
        {
        }

        public static KeyGenerator Instance { get; } = new KeyGenerator();

        #endregion

        public string GetUniqueKey(int maxSize = 15)
        {
            var chars = new char[62];
            chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            var data = new byte[1];

            using (var crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetNonZeroBytes(data);
                data = new byte[maxSize];
                crypto.GetNonZeroBytes(data);
            }

            var result = new StringBuilder(maxSize);

            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }

            return result.ToString();
        }
    }
}

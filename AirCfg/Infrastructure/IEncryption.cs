using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Ist.Pir.AirCfg.Infrastructure
{
    public interface IEncryption
    {
        #region Public Methods

        string DecryptString(string encryptedText, string key);

        string EncryptString(string plainText, string key);

        #endregion Public Methods
    }

    public class Encryption : IEncryption
    {
        #region Private Fields

        private string iv = @"s8df5w86h5e2vj6e";

        #endregion Private Fields

        #region Public Methods

        public string DecryptString(string encryptedText, string key)
        {
            if (key.Length > 32)
                key = key.Substring(32);
            else if (key.Length < 32)
                key = key.PadRight(32, '0');

            Aes aes = Aes.Create();
            aes.Mode = CipherMode.CBC;
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Padding = PaddingMode.Zeros;
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = Encoding.UTF8.GetBytes(iv);
            using (MemoryStream ms = new MemoryStream())
            {
                using (ICryptoTransform ct = aes.CreateDecryptor())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(ms, ct, CryptoStreamMode.Write))
                    {
                        byte[] cipherBytes = Convert.FromBase64String(encryptedText);
                        cryptoStream.Write(cipherBytes, 0, cipherBytes.Length);
                        cryptoStream.FlushFinalBlock();
                        return Encoding.UTF8.GetString(ms.ToArray());
                    }
                }
            }
        }

        public string EncryptString(string plainText, string key)
        {
            if (key.Length > 32)
                key = key.Substring(32);
            else if (key.Length < 32)
                key = key.PadRight(32, '0');

            byte[] plainBytes;
            byte[] cipherBytes;
            Aes aes = Aes.Create();
            aes.Mode = CipherMode.CBC;
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Padding = PaddingMode.Zeros;
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = Encoding.UTF8.GetBytes(iv);
            using (MemoryStream ms = new MemoryStream())
            {
                using (ICryptoTransform ct = aes.CreateEncryptor())
                {
                    using (CryptoStream cs = new CryptoStream(ms, ct, CryptoStreamMode.Write))
                    {
                        plainBytes = Encoding.UTF8.GetBytes(plainText);
                        cs.Write(plainBytes, 0, plainBytes.Length);
                        cs.FlushFinalBlock();
                    }
                    cipherBytes = ms.ToArray();
                }
            }
            return Convert.ToBase64String(cipherBytes, 0, cipherBytes.Length);
        }

        #endregion Public Methods
    }
}
using System.Security.Cryptography;
using System.Text;

namespace VRCGalleryManager.Core.Helpers
{
    internal class CryptAuth
    {
        private static readonly byte[] key = Encoding.UTF8.GetBytes("VRCGMCryptAuthXX");

        public static string Encrypt(string plainText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.GenerateIV();
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream())
                {
                    ms.Write(aes.IV, 0, aes.IV.Length);
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    using (StreamWriter writer = new StreamWriter(cs))
                    {
                        writer.Write(plainText);
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        public static string Decrypt(string cipherText)
        {
            byte[] fullCipher = Convert.FromBase64String(cipherText);
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                byte[] iv = new byte[aes.BlockSize / 8];
                Array.Copy(fullCipher, 0, iv, 0, iv.Length);
                aes.IV = iv;

                int cipherTextLength = fullCipher.Length - iv.Length;
                byte[] cipher = new byte[cipherTextLength];
                Array.Copy(fullCipher, iv.Length, cipher, 0, cipherTextLength);

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using (MemoryStream ms = new MemoryStream(cipher))
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (StreamReader reader = new StreamReader(cs))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}

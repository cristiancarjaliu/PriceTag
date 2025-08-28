using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace PriceTag.App.Services
{
    public class DbCredentials
    {
        public string? Server { get; set; }
        public string? Database { get; set; }
        public string? User { get; set; }
        public string? Password { get; set; }
    }

    public static class SecureConfig
    {
        private static readonly string FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "utilities.enc");
        private static readonly string Key = "k9fp7vZ8!Y3#xB1qR6@uZ7$Nd9*Lr2pW";

        public static DbCredentials? Load()
        {
            return Load(Key);
        }

        private static DbCredentials? Load(string encryptionKey)
        {
            using var fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read);
            using var br = new BinaryReader(fs);

            int saltLen = br.ReadInt32();
            byte[] salt = br.ReadBytes(saltLen);

            int ivLen = br.ReadInt32();
            byte[] iv = br.ReadBytes(ivLen);

            int cipherLen = br.ReadInt32();
            byte[] cipherText = br.ReadBytes(cipherLen);

            using var aes = Aes.Create();
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var keyDerivation = new Rfc2898DeriveBytes(encryptionKey, salt, 100000, HashAlgorithmName.SHA256);
            aes.Key = keyDerivation.GetBytes(32);
            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor();
            byte[] plainBytes = decryptor.TransformFinalBlock(cipherText, 0, cipherText.Length);
            string json = Encoding.UTF8.GetString(plainBytes);

            return JsonSerializer.Deserialize<DbCredentials>(json);
        }
    }
}

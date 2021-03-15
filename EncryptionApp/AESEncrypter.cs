using System;
using System.IO;
using SimpleAES;

namespace EncryptionApp {
    using Util;
    
    public class AESFile {
        public static WriteEffect encrypt(string plainText, string key, string path) {
            string cipherText = AES256.Encrypt(plainText, key);

            var dirPath = Path.GetDirectoryName(path);
            var encFileName = Path.GetFileNameWithoutExtension(path) + "_cipher" + Path.GetExtension(path);
            var encFilePath = Path.Combine(dirPath, encFileName);

            return new WriteEffect(cipherText, encFilePath, () => Console.WriteLine("Wrote encrypted file to: {0}", encFilePath));
        }

        public static WriteEffect decrypt(string cipherText, string key, string path) {
            string plaintext = AES256.Decrypt(cipherText, key);

            var fileName = Path.GetFileNameWithoutExtension(path);
            var dirPath = Path.GetDirectoryName(path);
            string decFileName = fileName.Contains("cipher")
                ? fileName.Replace("cipher", "plain") + Path.GetExtension(path)
                : fileName + "_plain" + Path.GetExtension(path);
            var decFilePath = Path.Combine(dirPath, decFileName);

            return new WriteEffect(plaintext, decFilePath, () => Console.WriteLine("Wrote decrypted file to: {0}", decFilePath));
        }
    }
}
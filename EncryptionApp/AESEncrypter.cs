using System;
using System.IO;

using SimpleAES;
using LanguageExt;
using static LanguageExt.Prelude;

using EncryptionApp.Util;

namespace EncryptionApp {

    public class AESText {
        public static string Encrypt(string plainText, string key) => AES256.Encrypt(plainText, key);
        public static string Decrypt(string cipherText, string key) => AES256.Decrypt(cipherText, key);
    }

    public class AESFile {
        public static WriteEffect Encrypt(string plainText, string key, string outputPath) {

            string cipherText = AES256.Encrypt(plainText, key);

            return new WriteEffect(cipherText, outputPath, () => Console.WriteLine("Wrote encrypted file to: {0}", outputPath));
        }

        public static WriteEffect Decrypt(string cipherText, string key, string outputPath) {

            string plaintext = AES256.Decrypt(cipherText, key);

            return new WriteEffect(plaintext, outputPath, () => Console.WriteLine("Wrote decrypted file to: {0}", outputPath));
        }


        public static Option<WriteEffect> ProcessFile(string key, bool isEncryptMode, string filePath, string outputPath) {

            Option<WriteEffect> effect = None;

            string fullOutputPath = getFullOutputPath(filePath, outputPath);
            string text = System.IO.File.ReadAllText(filePath);


            try {
                WriteEffect e = isEncryptMode
                    ? AESFile.Encrypt(text, key, fullOutputPath)
                    : AESFile.Decrypt(text, key, fullOutputPath);
                return Some(e);
            } catch {
                if (isEncryptMode) {
                    Console.WriteLine($"Error: Could not encrypt data in '{filePath}'.");
                } else {
                    Console.WriteLine($"Error: Could not decrypt data in '{filePath}'. Key may be invalid.");
                }
                return None;
            }
        }


        private static string getFullOutputPath(string filePath, string outputPath) {

            if (Util.File.IsDirectory(outputPath)) {
                return Path.Join(outputPath, Path.GetFileName(filePath));
            }
            return outputPath;
        }

    }
}
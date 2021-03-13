using System;
using System.Collections.Generic;
using System.IO;

using SimpleAES;

namespace CSEncryption {
    class Program {

        readonly struct Args {
            public readonly bool isEncryptMode { get; }
            public readonly string key { get; }
            public readonly string path { get; }

            public Args(bool isEncryptMode, string key, string path) {
                this.isEncryptMode = isEncryptMode;
                this.key = key;
                this.path = Path.GetFullPath(path);
            }
        }

        private static Args GetArgs(string[] args) {
            var argsList = new List<string>(args);
            bool isEncryptMode = argsList.Contains("-e");

            var (keyIdx, pathIdx) = (argsList.IndexOf("-key"), argsList.IndexOf("-path"));
            bool hasRequiredArgs = (keyIdx != -1 && pathIdx != -1) && (keyIdx + 1 < args.Length && pathIdx + 1 < args.Length);
            if (!hasRequiredArgs) {
                string help = "-key and -path arguments are required.\nAdd -e flag to encrypt file contents instead of decrypting.";
                Console.WriteLine(help);
                System.Environment.Exit(1);
            }

            string key = argsList[keyIdx + 1];
            string path = argsList[pathIdx + 1];

            return new Args(isEncryptMode, key, path);
        }

        private static Action encrypt(string plainText, string key, string path) {
            string cipherText = AES256.Encrypt(plainText, key);

            var dirPath = Path.GetDirectoryName(path);
            var encFileName = Path.GetFileNameWithoutExtension(path) + "_encrypted" + Path.GetExtension(path);
            var encFilePath = Path.Combine(dirPath, encFileName);

            return () => {
                File.WriteAllText(encFilePath, cipherText);
                Console.WriteLine("Wrote encrypted file to: {0}", encFilePath);
            };
        }

        private static Action decrypt(string cipherText, string key, string path) {
            string plaintext = AES256.Decrypt(cipherText, key);

            var fileName = Path.GetFileNameWithoutExtension(path);
            var dirPath = Path.GetDirectoryName(path);
            string decFileName = fileName.Contains("encrypted")
                ? fileName.Replace("encrypted", "decrypted") + Path.GetExtension(path)
                : fileName + "_decrypted" + Path.GetExtension(path);
            var decFilePath = Path.Combine(dirPath, decFileName);

            return () => {
                File.WriteAllText(decFilePath, plaintext);
                Console.WriteLine("Wrote decrypted file to: {0}", decFilePath);
            };
        }

        static void Main(string[] cliArgs) {
            Args args = GetArgs(cliArgs);
            if (args.isEncryptMode) {
                Console.WriteLine("Encrypt mode active");
            }
            else {
                Console.WriteLine("Decrypt mode active");
            }
            
            string text = System.IO.File.ReadAllText(args.path);

            if (string.IsNullOrEmpty(text)) {
                Console.WriteLine("Error: File contains no content.");
                System.Environment.Exit(2);
            }
            if (string.IsNullOrEmpty(args.key)) {
                Console.WriteLine("Error: Key cannot be null");
                System.Environment.Exit(3);
            }

            try {
                Action writeFile = args.isEncryptMode
                    ? encrypt(text, args.key, args.path)
                    : decrypt(text, args.key, args.path);
                try {
                    writeFile();
                } catch {
                    Console.WriteLine("Exception while writing file");
                }
            } catch {
                if (args.isEncryptMode) {
                    Console.WriteLine("Exception while encrypting data.");
                } else {
                    Console.WriteLine("Exception while decrypting data. Key may be invalid.");
                }
            }
        }
    }
}

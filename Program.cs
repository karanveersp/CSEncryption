using System;
using System.Collections.Generic;
using System.IO;

namespace CSEncryption {
    class Program {

        readonly struct Args {
            public readonly bool isEncryptMode { get; }
            public readonly string key { get; }
            public readonly string txtFilePath { get; }

            public Args(bool isEncryptMode, string key, string txtFilePath) {
                this.isEncryptMode = isEncryptMode;
                this.key = key;
                this.txtFilePath = txtFilePath;
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
            string txtFilePath = argsList[pathIdx + 1];

            return new Args(isEncryptMode, key, txtFilePath);
        }

        static void Main(string[] args) {
            Args parsedArgs = GetArgs(args);
            string text = System.IO.File.ReadAllText(parsedArgs.txtFilePath);

            var fp = Path.GetFullPath(parsedArgs.txtFilePath);
            var dirPath = Path.GetDirectoryName(fp);

            if (parsedArgs.isEncryptMode) {
                string cipherText = AES.encrypt(parsedArgs.key, text);

                // write to file
                var encFileName = Path.GetFileNameWithoutExtension(fp) + "_encrypted" + Path.GetExtension(fp);
                var encFilePath = Path.Combine(dirPath, encFileName);
                File.WriteAllText(encFilePath, cipherText);

                return;
            }

            string plaintext = AES.decrypt(parsedArgs.key, text);

            // write to file
            var fileName = Path.GetFileNameWithoutExtension(fp);
            string decFileName = fileName.Contains("encrypted")
                ? fileName.Replace("encrypted", "decrypted") + Path.GetExtension(fp)
                : fileName + "_decrypted" + Path.GetExtension(fp);
            var decFilePath = Path.Combine(dirPath, decFileName);
            File.WriteAllText(decFilePath, plaintext);

            return;
        }
    }
}

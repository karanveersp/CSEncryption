using System;
using System.Collections.Generic;
using System.IO;

namespace EncryptionApp {
    using Util;
    
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
                WriteEffect writeFile = args.isEncryptMode
                    ? AESFile.encrypt(text, args.key, args.path)
                    : AESFile.decrypt(text, args.key, args.path);
                try {
                    writeFile.write();
                } catch {
                    Console.WriteLine("Exception while writing file");
                    System.Environment.Exit(6);
                }
            } catch {
                if (args.isEncryptMode) {
                    Console.WriteLine("Exception while encrypting data.");
                    System.Environment.Exit(4);
                } else {
                    Console.WriteLine("Exception while decrypting data. Key may be invalid.");
                    System.Environment.Exit(5);
                }
            }
        }
    }
}

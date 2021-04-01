using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine;
using CommandLine.Text;
using EncryptionApp.Util;
using LanguageExt;

namespace EncryptionApp {

    class Program {

        [Verb("file", HelpText = "Act on file(s).")]
        class FileOptions {
            [Option('e', "encrypt", Required = false, HelpText = "To encrypt file contents. Will decrypt if not provided.", Default = false)]
            public bool IsEncryptMode { get; set; }

            [Option('k', "key", Required = true, HelpText = "Key for encryption/decryption.")]
            public string Key { get; set; }

            [Option('p', "path", Required = true, HelpText = ".txt file path target. If path is a directory, all .txt files inside are targeted using the same key.")]
            public string Path { get; set; }

            [Option('o', "output", Required = true, HelpText = "Path to output directory or file. If directory, the output file will have the same name as the input file.")]
            public string OutputPath { get; set; }

            [Usage(ApplicationAlias = "csencryption")]
            public static IEnumerable<Example> Examples {
                get {
                    yield return new Example("Encrypt a file", new FileOptions { IsEncryptMode = true, Key = "secretKey", Path = "path/to/plain.txt", OutputPath = "out/dir" });
                    yield return new Example("Decrypt a file", new FileOptions { Key = "secretKey", Path = "path/to/cipher.txt", OutputPath = "out/dir/plain.txt" });
                    yield return new Example("Encrypt all .txt files in a folder", new FileOptions { IsEncryptMode = true, Key = "secretKey", Path = "path/to/dir", OutputPath = "out/dir" });
                    yield return new Example("Decrypt all .txt files in a folder", new FileOptions { Key = "secretKey", Path = "path/to/dir", OutputPath = "out/dir" });
                }
            }
        }

        [Verb("string", HelpText = "Act on given string.")]
        class StringOptions {
            [Option('e', "encrypt", Required = false, HelpText = "To encrypt given string. Will decrypt if not provided.", Default = false)]
            public bool IsEncryptMode { get; set; }

            [Option('k', "key", Required = true, HelpText = "Key for encryption/decryption.")]
            public string Key { get; set; }

            [Option('s', "string", Required = true, HelpText = "String text target.")]
            public string Text { get; set; }

            [Usage(ApplicationAlias = "csencryption")]
            public static IEnumerable<Example> Examples {
                get {
                    yield return new Example("Encrypt a string", new StringOptions { IsEncryptMode = true, Key = "secretKey", Text = "plaintext" });
                    yield return new Example("Decrypt a string", new StringOptions { Key = "secretKey", Text = "ciphertext" });
                }
            }
        }

        static int RunStringMode(StringOptions opts) {

            if (opts.IsEncryptMode) {
                try {
                    string result = AESText.Encrypt(opts.Text, opts.Key);
                    Console.WriteLine("Encrypted text:");
                    Console.WriteLine(result);
                } catch {
                    Console.WriteLine("Error: Could not encrypt the provided string.");
                    return 2;
                }
            } else {
                try {
                    string result = AESText.Decrypt(opts.Text, opts.Key);
                    Console.WriteLine("Decrypted text:");
                    Console.WriteLine(result);
                } catch {
                    Console.WriteLine("Error: Could not decrypt the provided string. Key may be invalid.");
                    return 3;
                }
            }
            return 0;
        }

        static int RunFileMode(FileOptions opts) {

            // assume path provided is a single file
            string[] files = new string[] { opts.Path };

            if (Util.File.IsDirectory(opts.Path)) {
                files = Directory.GetFiles(opts.Path, "*.txt");
            }

            var effects = files
                .Choose<string, WriteEffect>(filePath => {
                    return AESFile.ProcessFile(opts.Key, opts.IsEncryptMode, filePath, opts.OutputPath);
                })
                .ToList();
            
            if (effects.Length() == 0) {
                return 4;
            }

            // list all files that can be written
            Console.WriteLine("File(s) that can be written successfully: ");
            effects.ForEach(e => Console.WriteLine(e.path));
            Console.WriteLine("Proceed (y/n) ? ");

            string proceed = Console.ReadLine();
            if (string.IsNullOrEmpty(proceed) || proceed == "n") {
                Console.WriteLine("Exiting...");
            } else {
                effects.ToList().ForEach(e => e.write());
            }
            return 0;
        }

        static int Main(string[] args) {
            return Parser.Default.ParseArguments<FileOptions, StringOptions>(args)
                .MapResult(
                    (FileOptions opts) => RunFileMode(opts),
                    (StringOptions opts) => RunStringMode(opts),
                    errs => 1
                );
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine;
using CommandLine.Text;

namespace CypherCLI {
    using AESLib;

    class Program {

        [Verb("file", HelpText = "Act on file(s).")]
        class FileOptions {
            [Option('e', "encrypt", Required = false, HelpText = "To encrypt file contents. Will decrypt if not provided.", Default = false)]
            public bool IsEncryptMode { get; set; }

            [Option('k', "key", Required = true, HelpText = "Key for encryption/decryption.")]
            public string Key { get; set; }

            [Option('i', "input", Required = true, HelpText = ".txt file path target. If path is a directory, all .txt files inside are targeted using the same key.")]
            public string InputPath { get; set; }

            [Option('o', "output", Required = false, HelpText = "Path to output directory or file. If not provided, the output is shown in the console.")]
            public string OutputPath { get; set; }

            [Usage(ApplicationAlias = "cypher")]
            public static IEnumerable<Example> Examples {
                get {
                    yield return new Example("Encrypt file contents and output in the console", new FileOptions { IsEncryptMode = true, Key = "secretKey", InputPath = "path/to/plain.txt" });
                    yield return new Example("Encrypt file contents and write to a new file", new FileOptions { IsEncryptMode = true, Key = "secretKey", InputPath = "path/to/plain.txt", OutputPath = "path/to/cipher.txt" });
                    yield return new Example("Decrypt a file and write output to given directory with same filename", new FileOptions { Key = "secretKey", InputPath = "path/to/cipher.txt", OutputPath = "out/dir/" });
                    yield return new Example("Encrypt all .txt files in a folder and write to given directory", new FileOptions { IsEncryptMode = true, Key = "secretKey", InputPath = "path/to/dir", OutputPath = "out/dir" });
                    yield return new Example("Decrypt all .txt files in a folder and write to given directory", new FileOptions { Key = "secretKey", InputPath = "path/to/dir", OutputPath = "out/dir" });
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

            [Usage(ApplicationAlias = "cypher")]
            public static IEnumerable<Example> Examples {
                get {
                    yield return new Example("Encrypt a string", new StringOptions { IsEncryptMode = true, Key = "secretKey", Text = "plaintext" });
                    yield return new Example("Decrypt a string", new StringOptions { Key = "secretKey", Text = "ciphertext" });
                }
            }
        }

        static int RunStringMode(StringOptions opts) {

            var result = opts.IsEncryptMode
                ? AES.Encrypt(opts.Text, opts.Key)
                : AES.Decrypt(opts.Text, opts.Key);

            if (opts.IsEncryptMode) {

                return result.Match(
                    Some: cipher => {
                        Console.WriteLine("Encrypted text:\n");
                        Console.WriteLine(cipher);
                        return 0;
                    },
                    None: () => {
                        Console.WriteLine("Error: Could not encrypt data.");
                        return 2;
                    }
                );

            }

            return result.Match(
                Some: plain => {
                    Console.WriteLine("Decrypted text:\n");
                    Console.WriteLine(plain);
                    return 0;
                },
                None: () => {
                    Console.WriteLine("Error: Could not decrypt data. Key may be invalid.");
                    return 3;
                }
            );
        }

        static int RunFileMode(FileOptions opts) {

            // assume path provided is a single file
            string[] files = new string[] { opts.InputPath };

            if (PathUtils.IsDirectory(opts.InputPath)) {
                if (PathUtils.IsFile(opts.OutputPath)) {
                    // cannot write results of processing all .txt files in a directory
                    // to a single file.
                    Console.WriteLine("OutputPath cannot be a file when InputPath is a directory.");
                    return 5;
                }
                files = Directory.GetFiles(opts.InputPath, "*.txt");
            }

            if (string.IsNullOrEmpty(opts.OutputPath)) {
                // print process results to console for each file
                var returnCodes = files.ToList().Map(filePath => {
                    StringOptions newOpts = new StringOptions();
                    newOpts.Key = opts.Key;
                    newOpts.Text = System.IO.File.ReadAllText(filePath);
                    newOpts.IsEncryptMode = opts.IsEncryptMode;

                    Console.WriteLine(new String(filePath).PadLeft(filePath.Length + 10,'-'));

                    int code = RunStringMode(newOpts);

                    Console.WriteLine(new String('-', filePath.Length + 10));
                    Console.WriteLine();

                    return code;

                });
                return returnCodes.Max();
            }

            var effects = files
                .Choose<string, WriteEffect>(filePath => {
                    return AES.ProcessFile(opts.Key, opts.IsEncryptMode, filePath, opts.OutputPath);
                })
                .ToList();

            if (effects.Length() == 0) {
                // error case where no file was successfully processed
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

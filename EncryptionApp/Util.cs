using System;
using System.IO;

namespace EncryptionApp.Util {
    public class File {
        public static bool IsDirectory(string path) {
            return System.IO.File.GetAttributes(path).HasFlag(FileAttributes.Directory);
        }
    }
    
    public class WriteEffect {
        public readonly string data;
        public readonly string path;
        public readonly Action write;

        public WriteEffect(string data, string path) {
            this.data = data;
            this.path = path;
            write = () => System.IO.File.WriteAllText(path, data);
        }

        public WriteEffect(string data, string path, Action afterWriting) {
            this.data = data;
            this.path = path;
            write = () => {
                System.IO.File.WriteAllText(path, data);
                afterWriting();
            };
        }
    }
}
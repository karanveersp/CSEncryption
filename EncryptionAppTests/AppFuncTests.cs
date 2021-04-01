using System;
using Xunit;
using EncryptionApp;
using EncryptionApp.Util;
using System.IO;

namespace EncryptionAppTests {
    public class AppFuncTests {
        [Fact]
        public void encodeReturnsNonNullWriteEffect() {
            var (plainText, key, outputPath) = ("Some data", "myKey", "OutputPath");
            WriteEffect writeEffect = AESFile.Encrypt(plainText, key, outputPath);

            Assert.NotNull(writeEffect);
        }

        [Fact]
        public void encryptReturnsEffectWithExpectedPath() {
            var (plainText, key, outputPath) = ("Some data", "myKey", "OutputPath\\Plain.txt");
            WriteEffect e = AESFile.Encrypt(plainText, key, outputPath);

            Assert.Equal("OutputPath\\Plain.txt", e.path);
        }

        [Fact]
        public void decryptReturnsEffectWithExpectedPathAndData() {
            var (cipher, key, outputPath) = ("rb2QZTrhyr0Sfpgo7OzCNLt5jdtk5IH97VOG3STur4qAJVzRemUgb9B74EVOPxy/", "myKey", "Path\\To\\Plain.txt");
            WriteEffect e = AESFile.Decrypt(cipher, key, outputPath);

            Assert.Equal("Path\\To\\Plain.txt", e.path);
            Assert.Equal("Some data", e.data);
        }
    }
}

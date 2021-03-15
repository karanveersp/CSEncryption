using System;
using Xunit;
using EncryptionApp;
using EncryptionApp.Util;

namespace EncryptionAppTests {
    public class AppFuncTests {
        [Fact]
        public void encodeReturnsNonNullWriteEffect() {
            var (plaintext, key, dummyPath) = ("Some data", "myKey", "Path/To/Plain.txt");
            WriteEffect writeEffect = AESFile.encrypt(plaintext, key, dummyPath);

            Assert.NotNull(writeEffect);
        }

        [Fact]
        public void encryptReturnsEffectWithExpectedPath() {
            var (plaintext, key, dummyPath) = ("Some data", "myKey", "Path/To/Plain.txt");
            WriteEffect e = AESFile.encrypt(plaintext, key, dummyPath);

            Assert.Equal("Path\\To\\Plain_cipher.txt", e.path);
        }

        [Fact]
        public void decryptReturnsEffectWithExpectedPathAndData() {
            var (cipher, key, dummyPath) = ("rb2QZTrhyr0Sfpgo7OzCNLt5jdtk5IH97VOG3STur4qAJVzRemUgb9B74EVOPxy/", "myKey", "Path/To/Cipher.txt");
            WriteEffect e = AESFile.decrypt(cipher, key, dummyPath);

            Assert.Equal("Path\\To\\Cipher_plain.txt", e.path);
            Assert.Equal("Some data", e.data);
        }
    }
}

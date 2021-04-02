using Xunit;
using AESLib;

namespace EncryptionAppTests {
    public class AppFuncTests {
        [Fact]
        public void encodeReturnsNonNullWriteEffect() {
            var (plainText, key, outputPath) = ("Some data", "myKey", "OutputPath");
            var writeEffect = AES.Encrypt(plainText, key, outputPath);

            Assert.True(writeEffect.IsSome);
        }

        [Fact]
        public void encryptReturnsEffectWithExpectedPath() {
            var (plainText, key, outputPath) = ("Some data", "myKey", "OutputPath\\Plain.txt");
            var e = AES.Encrypt(plainText, key, outputPath);

            string actualPath = e.Match<string>(
                Some: v => v.path, 
                None: () => ""
            );

            Assert.Equal("OutputPath\\Plain.txt", actualPath);
        }

        [Fact]
        public void decryptReturnsEffectWithExpectedPathAndData() {
            var (cipher, key, outputPath) = ("rb2QZTrhyr0Sfpgo7OzCNLt5jdtk5IH97VOG3STur4qAJVzRemUgb9B74EVOPxy/", "myKey", "Path\\To\\Plain.txt");
            var e = AES.Decrypt(cipher, key, outputPath);
            
            WriteEffect actual = e.Match(
                Some: v => v,
                None: () => null
            );
            
            Assert.Equal("Path\\To\\Plain.txt", actual.path);
            Assert.Equal("Some data", actual.data);
        }
    }
}

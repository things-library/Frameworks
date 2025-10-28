using ThingsLibrary.Security;
using System.Security.Cryptography;

namespace ThingsLibrary.Tests.Security
{
    [TestClass, ExcludeFromCodeCoverage]
    public class EncryptionTests
    {
        public const string AES_CRYPT_KEY = "6Xz7PlKu1QrzG6Ob9f5zT7Z3IJvCVOs45K1yq3VEnNo=";
        public const string AES_CRYPT_BASEIV = "uldqViSK5+VX4L8NNkB3Pw==";

        [TestMethod]
        public void Constructor()
        {
            var cipher = new Encryption(Encryption.EncryptionAlgorithmType.AES);

            Assert.AreEqual(Encryption.EncryptionAlgorithmType.AES, cipher.Algorithm);
            Assert.IsTrue(cipher.SymmetricAlgorithm is System.Security.Cryptography.Aes);            
        }

        [TestMethod]
        public void ConstructorWithKey()
        {
            var cipher = new Encryption(Encryption.EncryptionAlgorithmType.AES, AES_CRYPT_KEY, AES_CRYPT_BASEIV);

            Assert.AreEqual(Encryption.EncryptionAlgorithmType.AES, cipher.Algorithm);
            Assert.IsTrue(cipher.SymmetricAlgorithm is System.Security.Cryptography.Aes);
            Assert.AreEqual(AES_CRYPT_KEY, cipher.CryptKey);
            Assert.AreEqual(AES_CRYPT_BASEIV, cipher.CryptBaseIV);
        }

        [DataTestMethod]        
        [DataRow(6)]    //AES
        public void Encrypt(int algorithmTypeId)
        {
            var algorithmType = (Encryption.EncryptionAlgorithmType)algorithmTypeId;

            var data = "Hello world, this is a test of encryption! #10?";

            var cipher = new Encryption(algorithmType);

            // ENCRYPT IT
            var encryptedData = cipher.EncryptToBase64String(data);
            Assert.IsTrue(encryptedData.IsBase64());

            // DECRYPT IT
            var decryptedData = cipher.DecryptFromBase64String(encryptedData);
            Assert.AreEqual(data, decryptedData);
        }

        [TestMethod]
        public void EncryptBytes()
        {               
            var cipher = new Encryption(Encryption.EncryptionAlgorithmType.AES);

            // ENCRYPT IT
            Assert.Throws<CryptographicException>(() => cipher.Encrypt(null));

            // DECRYPT IT
            Assert.Throws<CryptographicException>(() => cipher.Decrypt(null));
        }

        [DataTestMethod]        
        [DataRow(6)]    //AES
        public void EncryptWithSalt(int algorithmTypeId)
        {
            var algorithmType = (Encryption.EncryptionAlgorithmType)algorithmTypeId;

            var data = "Hello world, this is a test of encryption! #10?";
            var salt = "S@lT!V@lue!";

            var cipher = new Encryption(algorithmType);

            // ENCRYPT IT
            var encryptedData = cipher.EncryptToBase64String(data, salt);
            Assert.IsTrue(encryptedData.IsBase64());

            // DECRYPT IT
            var decryptedData = cipher.DecryptFromBase64String(encryptedData, salt);
            Assert.AreEqual(data, decryptedData);


            // shrink the data to be smallar than the salt
            data = "Hi";
            encryptedData = cipher.EncryptToBase64String(data, salt);

            Assert.Throws<CryptographicException>(() => cipher.DecryptFromBase64String(encryptedData, "BAD"));
            Assert.Throws<CryptographicException>(() => cipher.DecryptFromBase64String(encryptedData, "BAD SALT Value 12345678901234567890123456789"));
        }

        [TestMethod]
        public void Decryption_Empty()
        {
            var cipher = new Encryption(Encryption.EncryptionAlgorithmType.AES);

            Assert.AreEqual(string.Empty, cipher.DecryptFromBase64String(string.Empty));
            Assert.AreEqual(string.Empty, cipher.DecryptFromBase64String(string.Empty, "SALT"));
        }

        #region --- Static Tests ---

        [TestMethod]
        public void CreateSaltString()
        {
            var saltString = Encryption.CreateSaltString(20);

            Assert.AreEqual(20, saltString.Length);

            Assert.Throws<ArgumentOutOfRangeException>(() => Encryption.CreateSaltString(0));
        }

        [TestMethod]
        public void GetSymmetricAlgorithm()
        {
            var algorithm = Encryption.GetSymmetricAlgorithm(Encryption.EncryptionAlgorithmType.AES);
            Assert.IsTrue(algorithm is System.Security.Cryptography.Aes);

            Assert.Throws<CryptographicException>(() => Encryption.GetSymmetricAlgorithm(Encryption.EncryptionAlgorithmType.DES));
            Assert.Throws<CryptographicException>(() => Encryption.GetSymmetricAlgorithm(Encryption.EncryptionAlgorithmType.RC2));
            Assert.Throws<CryptographicException>(() => Encryption.GetSymmetricAlgorithm(Encryption.EncryptionAlgorithmType.Rijndael));
        }

        #endregion

    }
}

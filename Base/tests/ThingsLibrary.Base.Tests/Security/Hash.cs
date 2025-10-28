using ThingsLibrary.Security;
using System.Security.Cryptography;
using static ThingsLibrary.Security.Hashing;

namespace ThingsLibrary.Tests.Security
{
    [TestClass, ExcludeFromCodeCoverage]
    public class HashTests
    {
        public const string HASH_BASE = "tEsTLdfmvqK2ePT1RTdVu8PgnUFewYeVgMuR8ej8+QVTck7eXiWsTmFtm1B5kw";

        [DataTestMethod]
        [DataRow((int)HashAlgorithmType.SHA256)]
        [DataRow((int)HashAlgorithmType.SHA384)]
        [DataRow((int)HashAlgorithmType.SHA512)]
        [DataRow((int)HashAlgorithmType.MD5)]
        public void Constructor(int hashAlgorithmTypeId)
        {
            var hashAlgorithmType = (HashAlgorithmType)hashAlgorithmTypeId;

            var hashAlgorithm = new Hashing(hashAlgorithmType, HASH_BASE);

            Assert.AreEqual(hashAlgorithmType, hashAlgorithm.Algorithm);
            Assert.IsTrue(hashAlgorithm.HashAlgorithm is HMAC);
        }

        [DataTestMethod]
        [DataRow((int)HashAlgorithmType.SHA1)]
        [DataRow((int)HashAlgorithmType.SHA256)]
        [DataRow((int)HashAlgorithmType.SHA384)]
        [DataRow((int)HashAlgorithmType.SHA512)]
        [DataRow((int)HashAlgorithmType.MD5)]
        public void Hash(int hashAlgorithmTypeId)
        {
            var hashAlgorithmType = (HashAlgorithmType)hashAlgorithmTypeId;

            var hashAlgorithm = new Hashing(hashAlgorithmType, HASH_BASE);

            // HASH
            var hash = hashAlgorithm.ComputeHashBase64("Test123456780ABC!#@$%^&*()");
                        
            Assert.IsTrue(hashAlgorithm.HashAlgorithm is HMAC);
            Assert.AreEqual(HASH_BASE, hashAlgorithm.HashBase);
            Assert.IsTrue(hash.IsBase64());

            // HASH WITH SALT
            hash = hashAlgorithm.ComputeHashBase64("Test123456780ABC!#@$%^&*()", "SALTVALUE");

            Assert.IsTrue(hashAlgorithm.HashAlgorithm is HMAC);
            Assert.AreEqual(HASH_BASE, hashAlgorithm.HashBase);
            Assert.IsTrue(hash.IsBase64());
        }

        [TestMethod]  
        public void Outlyers()
        {
            Assert.Throws<CryptographicException>(() => Hashing.GetHashAlgorithm(Hashing.HashAlgorithmType.Unknown, HASH_BASE));

            var hashAlgorithm = new Hashing(Hashing.HashAlgorithmType.MD5, HASH_BASE);
            Assert.Throws<CryptographicException>(() => hashAlgorithm.ComputeHash(null));
        }

    }
}

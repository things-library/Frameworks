using System.Security.Cryptography;

namespace ThingsLibrary.Security
{
    public class Hashing
    {
        public HashAlgorithmType Algorithm { get; init; }
        public HashAlgorithm HashAlgorithm { get; private set; }

        public string HashBase { get; private set; }

        public Hashing(HashAlgorithmType algorithm, string hashBase)
        {
            this.Algorithm = algorithm;
            this.HashAlgorithm = GetHashAlgorithm(algorithm, hashBase);
            this.HashBase = hashBase;
        }

        public string ComputeHashBase64(string value, string? salt = null)
        {
            byte[] hashBytes;
            if (string.IsNullOrEmpty(salt))
            {
                hashBytes = this.ComputeHash(value);
            }
            else
            {
                hashBytes = this.ComputeHash(string.Concat(value, salt));
            }

            return Convert.ToBase64String(hashBytes);
        }


        public byte[] ComputeHash(string value)
        {
            // encrypt the data; writing it to the memory stream.
            try
            {
                var valueBytes = Encoding.UTF8.GetBytes(value);
                var hashedBytes = this.HashAlgorithm.ComputeHash(valueBytes);

                return hashedBytes;
            }
            catch (Exception ex)
            {
                throw new CryptographicException($"Failed to compute hash: {ex.Message}", ex);
            }
        }

        public static HashAlgorithm GetHashAlgorithm(HashAlgorithmType algorithmType, string hashBase)
        {
            var keyBytes = Encoding.UTF8.GetBytes(hashBase);

            // return the specified algorithm.
            switch (algorithmType)
            {
                case HashAlgorithmType.SHA1: { return new HMACSHA1(keyBytes); }
                case HashAlgorithmType.SHA256: { return new HMACSHA256(keyBytes); }
                case HashAlgorithmType.SHA384: { return new HMACSHA384(keyBytes); }
                case HashAlgorithmType.SHA512: { return new HMACSHA512(keyBytes); }
                case HashAlgorithmType.MD5: { return new HMACMD5(keyBytes); }

                default:
                    {
                        throw new CryptographicException($"Hash algorithm '{algorithmType}' not supported.");
                    }
            }
        }

        /// <summary>
        /// Specifies the symmetric cryptographic algorithm to use for data encryption or decryption.
        /// </summary>
        public enum HashAlgorithmType : byte
        {
            [Obsolete("No longer valid")]
            Unknown = 0,

            SHA1 = 1,
            SHA256 = 2,
            SHA384 = 3,
            SHA512 = 4,
            MD5 = 5
        }
    }

}

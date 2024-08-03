using System.Security.Cryptography;

namespace ThingsLibrary.Security
{
    public class Encryption
    {
        /// <summary>
        /// Encryption Algorithm
        /// </summary>
        public EncryptionAlgorithmType Algorithm { get; init; }

        /// <summary>
        /// Crypt Key
        /// </summary>
        public string CryptKey { get; init; }

        /// <summary>
        /// Crypt Base (Init Vector)
        /// </summary>
        public string CryptBaseIV { get;  init; }

        public SymmetricAlgorithm SymmetricAlgorithm { get; init; }

        public Encryption(EncryptionAlgorithmType algorithm)
        {
            this.Algorithm = algorithm;
            this.SymmetricAlgorithm = Encryption.GetSymmetricAlgorithm(algorithm);

            this.CryptKey = Convert.ToBase64String(this.SymmetricAlgorithm.Key);
            this.CryptBaseIV = Convert.ToBase64String(this.SymmetricAlgorithm.IV);
        }

        /// <summary>
        /// Encryption class with base algorithm and keys
        /// </summary>
        /// <param name="algorithm"></param>
        /// <param name="cryptKey"></param>
        /// <param name="cryptBaseIV"></param>
        public Encryption(EncryptionAlgorithmType algorithm, string cryptKey, string cryptBaseIV)
        {
            this.Algorithm = algorithm;
            this.SymmetricAlgorithm = Encryption.GetSymmetricAlgorithm(algorithm);

            this.CryptKey = cryptKey;
            this.CryptBaseIV = cryptBaseIV;
        }

        #region --- Encryption ---

        /// <summary>
        /// Encrypts a string using the specified secret key and initalization vector (IV) property values.
        /// The encrypted value is returned as a base-64 encoded string.
        /// </summary>
        /// <param name="data">The string to encrypt.</param>
        /// <returns>The encrypted string, encoded as a base-64 string.</returns>
        public string EncryptToBase64String(string data)
        {
            // convert string to byte array, encrypt the array, and encode result as Base 64 string
            var result = this.Encrypt(System.Text.Encoding.ASCII.GetBytes(data));

            return Convert.ToBase64String(result);
        }

        /// <summary>
        /// Encrypts a salted string using the application crypto key.
        /// The encrypted value is returned as a base-64 encoded string.
        /// </summary>
        /// <param name="data">string to encrypt.</param>
        /// <param name="salt">The salt value use.</param>
        /// <returns>The encrypted string, encoded as a base-64 string.</returns>
        public string EncryptToBase64String(string data, string salt)
        {
            return this.EncryptToBase64String(salt + data);
        }

        /// <summary>
        /// Encrypts a data value using the secret key and initalization vector (IV) properties.
        /// </summary>
        /// <param name="data">The data value to encrypt.</param>
        /// <returns>The encrypted data value.</returns>
        public byte[] Encrypt(byte[] data)
        {
            // setup the encryption stream
            using ICryptoTransform transform = this.SymmetricAlgorithm.CreateEncryptor(Convert.FromBase64String(this.CryptKey), Convert.FromBase64String(this.CryptBaseIV));

            // setup the stream that will hold the encrypted data.
            var memStreamEncryptedData = new MemoryStream();

            var encStream = new CryptoStream(memStreamEncryptedData, transform, CryptoStreamMode.Write);

            // encrypt the data; writing it to the memory stream.
            try
            {
                encStream.Write(data, 0, data.Length);
                encStream.FlushFinalBlock();
                encStream.Close();
            }
            catch (Exception ex)
            {
                throw new CryptographicException("Failed to write encrypted data to memory stream: " + ex.Message, ex);
            }

            // send the encrypted data back to caller
            return memStreamEncryptedData.ToArray();            
        }

        #endregion

        #region --- Decryption ---

        /// <summary>
        /// Decrypts a base-64 encoded string using the specified secret key and
        /// initalization vector (IV) properties values.
        /// </summary>
        /// <param name="value">The string to decrypt (encoded as a base-64 string).</param>
        /// <returns>The decrypted string.</returns>
        public string DecryptFromBase64String(string value)
        {
            // nothing to decrypt
            if (string.IsNullOrEmpty(value)) { return value; }

            // convert string to byte array, decrypt the array, and return the result as a plain string
            var result = this.Decrypt(Convert.FromBase64String(value));

            return Encoding.ASCII.GetString(result);
        }

        /// <summary>
        /// Decrypts a salted base-64 encoded string using the application crypto key.
        /// </summary>
        /// <param name="value">string to decrypt (encoded as a base-64 string).</param>
        /// <param name="salt">Salt value used to encrypt the string.</param>
        /// <returns>The decrypted string.</returns>
        /// <exception cref="CryptographicException">If salt value does not match</exception>
        public string DecryptFromBase64String(string value, string salt)
        {
            // nothing to decrypt
            if (string.IsNullOrEmpty(value)) { return value; }

            //
            var saltedData = DecryptFromBase64String(value);
            if (saltedData.Length < salt.Length || saltedData.Substring(0, salt.Length) != salt)
            {                
                throw new CryptographicException($"Decryption failed: Salt value '{salt}' does not match the one found in encrypted data.");
            }

            return saltedData.Substring(salt.Length);
        }

        /// <summary>
        /// Decrypts a data value using the secret key and initalization vector (IV) properties.
        /// </summary>
        /// <param name="data">The data value to decrypt.</param>
        /// <returns>The decrypted data value.</returns>
        public byte[] Decrypt(byte[] data)
        {            
            var algorithm = GetSymmetricAlgorithm(this.Algorithm);

            // setup the decryption stream
            using (var transform = algorithm.CreateDecryptor(Convert.FromBase64String(this.CryptKey), Convert.FromBase64String(this.CryptBaseIV)))
            {
                return Decrypt(transform, data);
            }
        }


        /// <summary>
        /// Decrypts a data value using the secret key and initalization vector (IV) properties.
        /// </summary>
        /// <param name="value">The data value to decrypt.</param>
        /// <returns>The decrypted data value.</returns>
        /// <exception cref="CryptographicException">When decryption fails</exception>
        public byte[] Decrypt(ICryptoTransform transform, byte[] value)
        {
            // set up the memory stream for the decrypted data.
            var memStreamDecryptedData = new MemoryStream();

            // setup the decryption stream
            var decStream = new CryptoStream(memStreamDecryptedData, transform, CryptoStreamMode.Write);

            // decrypt the data; writing it to the memory stream.
            try
            {
                decStream.Write(value, 0, value.Length);
            }
            catch (Exception ex)
            {
                throw new CryptographicException($"Failed to write decrypted data to memory stream: {ex.Message}", ex);
            }
            decStream.FlushFinalBlock();
            decStream.Close();

            // send the decrypted data back to caller
            return memStreamDecryptedData.ToArray();
        }

        #endregion


        /// <summary>
        /// Generates a cryptographically strong sequence of base-64 characters.
        /// </summary>
        /// <param name="length">Length of the string to generate.</param>
        /// <returns>string containing random characters.</returns>
        /// <exception cref="ArgumentOutOfRangeException">When length <= 0</exception>
        public static string CreateSaltString(int length)
        {
            if (length < 1) { throw new ArgumentOutOfRangeException("length", "Length must be one or greater."); }

            // generate enough random bytes so that we have enough after base-64 encoding them (but not too many extra)
            var rng = RandomNumberGenerator.Create();
            var buffer = new byte[length * 6 / 8 + 1];
            rng.GetBytes(buffer);

            // encode the bytes a return the correct length of string
            return Convert.ToBase64String(buffer).Substring(0, length);
        }

        /// <summary>
        /// Gets the symmetric algorithm for the provided type.
        /// </summary>
        /// <param name="algorithm">Algorithm Type</param>
        /// <returns><see cref="SymmetricAlgorithm" /></returns>
        /// <exception cref="CryptographicException"></exception>
        public static SymmetricAlgorithm GetSymmetricAlgorithm(EncryptionAlgorithmType algorithmType)
        {
            // return the specified algorithm.
            switch (algorithmType)
            {
                case EncryptionAlgorithmType.AES:
                    {                        
                        return Aes.Create();                        
                    }
                                        
                //case EncryptionAlgorithmType.TripleDes: // LEGECY
                //    {
                //        return TripleDES.Create();
                //    }

                default:
                    {
                        throw new CryptographicException($"Algorithm '{algorithmType}' not supported.");
                    }
            }
        }

        /// <summary>
        /// Specifies the symmetric cryptographic algorithm to use for data encryption or decryption.
        /// </summary>
        public enum EncryptionAlgorithmType : byte
        {            
            /// <summary>
            /// Indicates the Data Encryption Standard (DES) algorithm.
            /// </summary>
            [Obsolete("DES works with 56-bit keys allow attacks via exhaustive search")]
            DES = 1,
            
            /// <summary>
            /// Indicates the RC2 symmetric encryption algorithm.
            /// </summary>
            [Obsolete("RC2 is vulnerable to a related-key attack")]
            RC2 = 2,
            
            /// <summary>
            /// Indicates the Rijndael symmetric encryption algorithm.
            /// </summary>
            [Obsolete("Is vulnerable to attack")]
            Rijndael = 3,
            
            /// <summary>
            /// Indicates the Triple Data Encryption Standard algorithm.
            /// </summary>
            [Obsolete("Triple DES is vulnerable to meet-in-the-middle attack")]
            TripleDes = 4,            
            
            /// <summary>
            /// Advanced Encryption Standard
            /// </summary>
            AES = 6
        }
    }
}

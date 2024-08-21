using CloudProvider = ThingsLibrary.Storage.Gcp;

namespace Starlight.Cloud.File.Tests
{    
    [TestClass, ExcludeFromCodeCoverage]
    public class Gcp
    {
        public static string BucketName { get; set; }

        private string ConnectionString => "DataSource=;Name=TestDabase";

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            BucketName = $"test{System.Math.Abs(Guid.NewGuid().GetHashCode())}";
        }

        [DataTestMethod]
        [DataRow(null, "testtable")]   // Null connectionstring
        [DataRow("", "testtable")]     // Empty connection string
        [DataRow("Something;something;something", null)] // Null table name
        [DataRow("Something;something;something", "")]   // Empty table name
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_TestBadParameters(string connectionString, string bucketName)
        {
            new CloudProvider.FileStore(connectionString, bucketName);
        }

        /// <summary>
        /// Tests Bucket naming violations
        /// </summary>
        /// <param name="bucketName">Bucket Name</param>
        /// <remarks>
        /// Bucket Naming Requirements: https://cloud.google.com/storage/docs/naming-buckets        
        /// 1. Bucket names can only contain lowercase letters, numeric characters, dashes (-), underscores (_), and dots(.). Spaces are not allowed. Names containing dots require verification.
        /// 2. Bucket names must start and end with a number or letter.
        /// 3. Bucket names must contain 3 - 63 characters.Names containing dots can contain up to 222 characters, but each dot - separated component can be no longer than 63 characters.
        /// 4. Bucket names cannot be represented as an IP address in dotted - decimal notation(for example, 192.168.5.4).
        /// 5. Bucket names cannot begin with the "goog" prefix.
        /// 6. Bucket names cannot contain "google" or close misspellings, such as "g00gle".
        /// </remarks>
        [DataTestMethod]
        [DataRow("aBbcdef")] // 1. Bucket names can only contain lowercase letters, numeric characters, dashes (-), underscores (_), and dots(.). Spaces are not allowed.
        [DataRow("space character")]    //1. Bucket names can only contain lowercase letters, numeric characters, dashes (-), underscores (_), and dots(.). Spaces are not allowed.
        [DataRow("bad#character")]      //1. Bucket names can only contain lowercase letters, numeric characters, dashes (-), underscores (_), and dots(.). Spaces are not allowed.
        [DataRow("bad%character")]      //1. Bucket names can only contain lowercase letters, numeric characters, dashes (-), underscores (_), and dots(.). Spaces are not allowed.
        [DataRow("_badcharacter")]      //2. Bucket names must start and end with a number or letter.
        [DataRow("badcharacter_")]      //2. Bucket names must start and end with a number or letter.
        [DataRow("#badcharacter")]      //2. Bucket names must start and end with a number or letter.
        [DataRow("badcharacter#")]      //2. Bucket names must start and end with a number or letter.
        [DataRow("doc-example-bucket-")]      //2. Bucket names must start and end with a number or letter.        
        [DataRow("1")]  // 3. Bucket names must contain 3 - 63 characters
        [DataRow("12")] // 3. Bucket names must contain 3 - 63 characters        
        [DataRow("1234567890123456789012345678901234567890123456789012345678901234")]    //3. Bucket names must contain 3 - 63 characters
        [DataRow("192.168.5.4")]        //4. Bucket names cannot be represented as an IP address in dotted - decimal notation(for example, 192.168.5.4).
        [DataRow("googsomething")]      //5. Bucket names cannot begin with the "goog" prefix.
        [DataRow("somegooglething")]    //6. Bucket names cannot contain "google" or close misspellings, such as "g00gle".
        [DataRow("someg00glething")]    //6. Bucket names cannot contain "google" or close misspellings, such as "g00gle".Bucket names cannot contain "google" or close misspellings, such as "g00gle".        
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_TestTableName(string bucketName)
        {
            new CloudProvider.FileStore(ConnectionString, bucketName);
        }
    }
}


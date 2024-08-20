//using CloudProvider = ThingsLibrary.Storage.Local;

//namespace ThingsLibrary.Storage.Tests
//{
//    [TestClass, ExcludeFromCodeCoverage]
//    public class Local
//    {
//        public static string BucketName { get; set; }

//        private string ConnectionString => "RootStorePath=./TestDirectoryFilename=:memory:";

//        [ClassInitialize]
//        public static void ClassInitialize(TestContext testContext)
//        {
//            BucketName = $"test{System.Math.Abs(Guid.NewGuid().GetHashCode())}";
//        }

//        [DataTestMethod]
//        [DataRow(null, "testtable")]   // Null connectionstring
//        [DataRow("", "testtable")]     // Empty connection string
//        [DataRow("Something;something;something", null)] // Null table name
//        [DataRow("Something;something;something", "")]   // Empty table name
//        [ExpectedException(typeof(ArgumentNullException))]
//        public void Constructor_TestBadParameters(string connectionString, string bucketName)
//        {
//            new CloudProvider.FileStore(connectionString, bucketName);
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="bucketName"></param>
//        /// <remarks>
//        /// A container name must be a valid DNS name, conforming to the following naming rules:
//        /// 1. Container names must be from 3 through 63 characters long.
//        /// 2. Container names must start or end with a letter or number
//        /// 3. Container names can contain only letters, numbers, and the dash(-) character.
//        /// 4. Every dash(-) character must be immediately preceded and followed by a letter or number; consecutive dashes are not permitted in container names.
//        /// 5. All letters in a container name must be lowercase.
//        /// </remarks>
//        [DataTestMethod]
//        [DataRow("a")]                  // 1. Container names must be from 3 through 63 characters long.
//        [DataRow("a2")]                 // 1. Container names must be from 3 through 63 characters long.
//        [DataRow("a234567890123456789012345678901234567890123456789012345678901234")]    //1. Container names must be from 3 through 63 characters long.
//        [DataRow("-badcharacter")]      // 2. Container names must start or end with a letter or number
//        [DataRow("badcharacter-")]      // 2. Container names must start or end with a letter or number
//        [DataRow("space character")]    // 3. Container names can contain only letters, numbers, and the dash(-) character.
//        [DataRow("bad#Character")]      // 3. Container names can contain only letters, numbers, and the dash(-) character.
//        [DataRow("bad_Character")]      // 3. Container names can contain only letters, numbers, and the dash(-) character.
//        [DataRow("bad%character")]      // 3. Container names can contain only letters, numbers, and the dash(-) character.
//        [DataRow("bad--character")]     // 4. Every dash(-) character must be immediately preceded and followed by a letter or number; consecutive dashes are not permitted in container names.
//        [DataRow("abCd")]               // 5. All letters in a container name must be lowercase.
//        [ExpectedException(typeof(ArgumentException))]
//        public void Constructor_TestTableName(string bucketName)
//        {
//            new CloudProvider.FileStore(ConnectionString, bucketName);
//        }
//    }
//}


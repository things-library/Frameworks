using CloudProvider = ThingsLibrary.Storage.Aws;

namespace ThingsLibrary.Storage.Tests
{
    [TestClass, ExcludeFromCodeCoverage]
    public class Aws
    {
        private string ConnectionString => "DataSource=;Name=TestDabase";

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            //nothing
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
        /// The following rules apply for naming buckets in Amazon S3:  https://docs.aws.amazon.com/AmazonS3/latest/userguide/bucketnamingrules.html
        /// 1. Bucket names must be between 3 (min) and 63 (max) characters long.
        /// 2. Bucket names can consist only of lowercase letters, numbers, dots (.), and hyphens (-).
        /// 3. Bucket names must begin and end with a letter or number.
        /// 4. Bucket names must not be formatted as an IP address (for example, 192.168.5.4).
        /// 5. Bucket names must not start with the prefix xn--.
        /// 6. Bucket names must not end with the suffix -s3alias. This suffix is reserved for access point alias names. For more information, see Using a bucket-style alias for your access point.
        /// 7. Bucket names must be unique across all AWS accounts in all the AWS Regions within a partition. A partition is a grouping of Regions. AWS currently has three partitions: aws (Standard Regions), aws-cn (China Regions), and aws-us-gov (AWS GovCloud (US)).
        /// 8. A bucket name cannot be used by another AWS account in the same partition until the bucket is deleted.
        /// 9. Buckets used with Amazon S3 Transfer Acceleration can't have dots (.) in their names. For more information about Transfer Acceleration, see Configuring fast, secure file transfers using Amazon S3 Transfer Acceleration.
        /// </remarks>
        [DataTestMethod]
        [DataRow("1")]                  // 1. Bucket names must be between 3 (min) and 63 (max) characters long.
        [DataRow("12")]                 // 1. Bucket names must be between 3 (min) and 63 (max) characters long.
        [DataRow("1234567890123456789012345678901234567890123456789012345678901234")]    // 1. Bucket names must be between 3 (min) and 63 (max) characters long.
        [DataRow("aBbcdef")]            // 2. Bucket names can consist only of lowercase letters, numbers, dots (.), and hyphens (-).        
        [DataRow("space character")]    // 2. Bucket names can consist only of lowercase letters, numbers, dots (.), and hyphens (-).
        [DataRow("bad#character")]      // 2. Bucket names can consist only of lowercase letters, numbers, dots (.), and hyphens (-).
        [DataRow("bad%character")]      // 2. Bucket names can consist only of lowercase letters, numbers, dots (.), and hyphens (-).
        [DataRow("_badcharacter")]      // 3. Bucket names must begin and end with a letter or number.
        [DataRow("badcharacter_")]      // 3. Bucket names must begin and end with a letter or number.
        [DataRow("#badcharacter")]      // 3. Bucket names must begin and end with a letter or number.
        [DataRow("badcharacter#")]      // 3. Bucket names must begin and end with a letter or number.
        [DataRow("192.168.5.4")]        // 4. Bucket names must not be formatted as an IP address (for example, 192.168.5.4).
        [DataRow("xn--something")]      // 5. Bucket names must not start with the prefix xn--.
        [DataRow("something-s3alias")]  // 6. Bucket names must not end with the suffix -s3alias.
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_TestTableName(string bucketName)
        {
            new CloudProvider.FileStore(ConnectionString, bucketName);
        }
    }
}


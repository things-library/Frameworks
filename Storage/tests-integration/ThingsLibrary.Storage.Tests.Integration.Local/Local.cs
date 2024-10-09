// ================================================================================
// <copyright file="Local.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

//namespace ThingsLibrary.Storage.Tests.Integration.Local
//{
//    [TestClassIf, IgnoreIf(nameof(IgnoreTests)), ExcludeFromCodeCoverage]
//    public class LocalTests : Base.IBaseTests
//    {
//        private const FileStoreType FILE_STORE_TYPE = FileStoreType.Local;
//        public static string BucketName { get; set; } = $"TestBucket";
//        public static Loc.FileStore FileStore { get; set; } = null;

//        #region --- Provider ---

//        // ======================================================================
//        // Called once before ALL tests
//        // ======================================================================
//        [ClassInitialize]
//        public static void ClassInitialize(TestContext testContext)
//        {
//            var configuration = typeof(LocalTests).GetConfigurationRoot();

//            // start test environment
//            await TestEnvironment.StartAsync();

//            // see if we have any reason to just exit and ignore tests
//            if (TestEnvironment.IgnoreTests()) { return; }

//            // set up the static properties
//            FileStore = new Loc.FileStore(connectionString, BucketName);
//        }

//        // ======================================================================
//        // Called once AFTER all tests
//        // ======================================================================
//        [ClassCleanup]
//        public static void ClassCleanup()
//        {
//            // dispose of the File store / closing database connection
//            if(FileStore != null)
//            {
//                var bucketFolderPath = FileStore.BucketDirectoryPath;

//                FileStore.DataContext.Dispose();
//                FileStore = null;

//                if (!string.IsNullOrEmpty(bucketFolderPath))
//                {
//                    IO.Directory.TryDeleteDirectory(bucketFolderPath);
//                }
//            }      
//        }

//        public static bool IgnoreTests()
//        {
//            return (FileStore == null);
//        }

//        #endregion

//        #region --- Provider Specific Tests


//        #endregion

//        #region --- BASE TESTS ---

//        [TestMethodIf]
//        public void StoreType()
//        {
//            Assert.AreEqual(FILE_STORE_TYPE, FileStore.StorageType);
//        }

//        [TestMethodIf]
//        public void TestFile()
//        {
//            BaseTests.TestFile(FileStore);
//        }

//        [TestMethodIf]
//        public void TestImageFile()
//        {
//            BaseTests.TestImageFile(FileStore);
//        }

//        #endregion
//    }
//}

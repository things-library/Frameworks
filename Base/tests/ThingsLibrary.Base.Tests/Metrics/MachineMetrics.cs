using ThingsLibrary.Metrics;
using System.IO;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

namespace ThingsLibrary.Tests.Metrics
{
    [TestClass, ExcludeFromCodeCoverage]
    public class MachineMetricsTests
    {
        private static string TestDirectoryPath { get; set; }

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            TestDirectoryPath = testContext.TestRunDirectory ?? throw new ArgumentException("Unable to get test run directory.");
        }

        [TestMethod]
        public void Constructor()
        {
            Assert.AreEqual(Environment.MachineName, MachineMetrics.MachineName());
            Assert.AreEqual(Convert.ToInt16(Environment.ProcessorCount), MachineMetrics.CpuCount());

            Assert.AreEqual(RuntimeInformation.IsOSPlatform(OSPlatform.OSX), MachineMetrics.IsMac());
            Assert.AreEqual(RuntimeInformation.IsOSPlatform(OSPlatform.Linux), MachineMetrics.IsLinux());
            Assert.AreEqual(RuntimeInformation.IsOSPlatform(OSPlatform.Windows), MachineMetrics.IsWindows());

            var isUnix = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD);
            Assert.AreEqual(isUnix, MachineMetrics.IsUnix());
        }

        //[TestMethod]
        //public void MemoryUsed()
        //{
        //    //Environment.WorkingSet / 1000
        //    var metricsMemUsed = MachineMetrics.ProcessUsedMemoryKB;
        //    var methodMemUsed = Environment.WorkingSet / 1000;

        //    var diff = metricsMemUsed - methodMemUsed;

        //    Assert.IsTrue(System.Math.Abs(diff) < 10d);
        //}

        [TestMethod]
        public void NetworkMetrics()
        {
            Assert.AreEqual(NetworkInterface.GetIsNetworkAvailable(), MachineMetrics.IsNetworkAvailable());

            Assert.AreEqual(Environment.OSVersion.ToString(), MachineMetrics.OsVersion());

            Assert.IsTrue(MachineMetrics.LocalIPAddresses().Any());
            Assert.IsTrue(MachineMetrics.LocalIPAddress() != null);

            Assert.IsTrue(MachineMetrics.MacAddresses().Any());
            Assert.IsTrue(MachineMetrics.MacAddress() != null);
        }

        //[TestMethod]
        //public void IneternetAvailable()
        //{
        //    Assert.IsTrue(MachineMetrics.IsInternetAvailable());
        //}


        [TestMethod]
        public void GetAvailableHardDriveSpaceKB()
        {
            Assert.IsTrue(MachineMetrics.GetAvailableHardDriveSpaceKB() > 0);

            Assert.IsTrue(MachineMetrics.GetAvailableHardDriveSpaceKB(TestDirectoryPath) > 0);

            //BAD Tests
            Assert.ThrowsException<ArgumentException>(() => MachineMetrics.GetAvailableHardDriveSpaceKB(Path.Combine(TestDirectoryPath, "BAD_FOLDER")));
        }

        [TestMethod]
        public void GetWifiDetails()
        {
            //var details = MachineMetrics.GetWifiDetails();
            //Assert.IsTrue(details.Count > 0);
        }
    }
}

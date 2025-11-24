using ThingsLibrary.Metrics;
using System.Reflection;

namespace ThingsLibrary.Tests.Metrics
{
    [TestClass, ExcludeFromCodeCoverage]
    public class AssemblyMetricsTests
    {
        [TestMethod]
        public void Constructor()
        {
            var assemblyMetrics = new AssemblyMetrics();

            // test to make sure the empty constructor is using calling assembly
            var assembly = Assembly.GetEntryAssembly();
            Assert.AreSame(assembly, assemblyMetrics.Assembly);

            Assert.AreEqual(System.IO.Path.GetDirectoryName(assemblyMetrics.Assembly.Location), assemblyMetrics.DirectoryPath());
            Assert.AreEqual(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), assemblyMetrics.Name()), assemblyMetrics.AppDataPath());
            Assert.IsTrue(assemblyMetrics.TempDirectoryPath().StartsWith(System.IO.Path.GetTempPath()));
            Assert.AreEqual(assembly.GetName()?.Version, assemblyMetrics.FileVersion());
            Assert.AreEqual(assemblyMetrics.FileVersion().ToDotString(), assemblyMetrics.FileVersionStr());
            
            // this will change over time but good to test what it is currently set to
            //Assert.AreEqual(".NETCoreApp,Version=v6.0", metrics.NetFrameworkVersion());
        }

        [TestMethod]
        public void Constructor_Specific()
        {
            // test to make sure the empty constructor is using calling assembly
            var assembly = this.GetType().Assembly;
            var metrics = new AssemblyMetrics(assembly);

            // From assembly file
            Assert.AreEqual(new Guid("10000000-1000-1000-1000-100000000000"), metrics.Id());

            // make sure we are actually using what we passed
            Assert.AreSame(assembly, metrics.Assembly);

            // From Project file
            //<PropertyGroup>
            //	<ProductName>ThingsLibrary.Tests</ProductName>
            //	<Title>Base Tests</Title>
            //	<Description>This is the canvas tests project</Description>
            //	<Authors>Mark Lanning</Authors>
            //	<Company>Services</Company>
            //  <Copyright>2022</Copyright>
            //</PropertyGroup>
            Assert.AreEqual("ThingsLibrary.Core.Tests", metrics.ProductName());
            Assert.AreEqual("ThingsLibrary.Core.Tests", metrics.Name());
            Assert.AreEqual(assembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title, metrics.Title());
            //Assert.AreEqual("Base Tests", metrics.Title());   //for some reason assembly picks up productname as also title
            Assert.AreEqual("This is the core tests project", metrics.Description());
            Assert.AreEqual(assembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company, metrics.Company());
            Assert.AreEqual("1998", metrics.Copyright());

            Assert.AreEqual(assembly.GetName().Name, metrics.Namespace());
            
            // File Version
            Assert.AreEqual(assembly.GetName().Version, metrics.FileVersion());            
            Assert.AreEqual(assembly.GetName().Version, new Version(metrics.FileVersionStr()));
            Assert.AreEqual(assembly.GetName().Version.ToLong(), metrics.FileVersionLong());

            Assert.AreEqual($"{metrics.Namespace()}/{metrics.Version()}", metrics.AgentString());

            var createdOn = System.IO.File.GetCreationTime(assembly.Location).ToUniversalTime();
            Assert.AreEqual(createdOn, metrics.CreatedOn());

            Assert.AreEqual(assembly.Location, metrics.Path());
            Assert.AreEqual(System.IO.Path.GetDirectoryName(assembly.Location), metrics.DirectoryPath());
        }

        [TestMethod]
        public void Constructor_Instance()
        {
            var assembly = this.GetType().Assembly;

            AssemblyMetrics.Instance = new AssemblyMetrics(assembly);

            Assert.AreSame(assembly, AssemblyMetrics.Instance.Assembly);
        }

        [TestMethod]
        public void GetAssemblyId()
        {
            var assembly = this.GetType().Assembly;

            Assert.AreEqual(new Guid("10000000-1000-1000-1000-100000000000"), AssemblyMetrics.GetId(assembly));

            // assembly that doesn't have a id
            assembly = typeof(MachineMetrics).Assembly;

            Assert.AreEqual(Guid.Empty, AssemblyMetrics.GetId(assembly));
        
        }

        [TestMethod]
        public void GetDependencyAssemblies()
        {
            var assembly = this.GetType().Assembly;

            var versions = AssemblyMetrics.GetDependencyVersions(assembly);

            Assert.AreEqual(25, versions.Count);
        }
    }
}

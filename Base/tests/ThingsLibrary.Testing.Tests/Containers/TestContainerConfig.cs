using System.Diagnostics;

using ThingsLibrary.Testing.Containers;
using ThingsLibrary.Testing.Extensions;

namespace ThingsLibrary.Testing.Tests.Containers
{
    [TestClass, ExcludeFromCodeCoverage]
    public class TestContainerConfigTests
    {
        [TestMethod]
        public void Constructor()
        {
            var config = new TestContainerOptions
            {
                Image = "imagepath",
                Name = "test",
                Ports = new List<string> { "8080:8008", "9001:9002", "7001:7002" },
                Environment = new Dictionary<string, string>()
                {
                    { "TZ", "America/Chicago" }
                }
            };

            Assert.AreEqual("imagepath", config.Image);
            Assert.AreEqual("test", config.Name);
            Assert.IsTrue(config.Ports.Contains("9001:9002"));

            Assert.AreEqual("America/Chicago", config.Environment["TZ"]);
        }

        [TestMethod]
        public void GetContainerBuilder()
        {
            var config = new TestContainerOptions
            {
                Image = "imagepath",
                Name = "test",
                Ports = new List<string> { "8080:8008", "9001:9002", "7001:7002" },
                Environment = new Dictionary<string, string>()
                {
                    { "TZ", "America/Chicago" }
                }
            };

            var builder = config.GetContainerBuilder();            
            var built = builder.Build();
            
            Assert.AreEqual("imagepath", built.Image.Name);            
        }

        [TestMethod]
        public void GetContainerBuilder_BadData()
        {
            var validImage = "imagepath:latest";
            var validName = "container_name";
            var validPorts = new List<string> { "8080:8080", "9001:9002", "7001:7002" };

            // THIS SHOULD NOT THROW ANY ERRORS
            _ = new TestContainerOptions { Image = validImage, Name = validName, Ports = validPorts }.GetContainerBuilder();

            // Image Name Test
            Assert.ThrowsException<ArgumentException>(() => new TestContainerOptions { Image = null, Name = validName, Ports = validPorts }.GetContainerBuilder()); ;   // No Image Name
            Assert.ThrowsException<ArgumentException>(() => new TestContainerOptions { Image = "", Name = validName, Ports = validPorts }.GetContainerBuilder());       // No Image Name

            // Container Name Test 
            //  must be all lower-case, must start or end with a letter or number, and can contain only letters, numbers, and the dash(-) character
            Assert.ThrowsException<ArgumentException>(() => new TestContainerOptions { Image = validImage, Name = null, Ports = validPorts }.GetContainerBuilder());    // No Container Name
            Assert.ThrowsException<ArgumentException>(() => new TestContainerOptions { Image = validImage, Name = "", Ports = validPorts }.GetContainerBuilder());      // No Container Name
            Assert.ThrowsException<ArgumentException>(() => new TestContainerOptions { Image = validImage, Name = "TEST", Ports = validPorts }.GetContainerBuilder());  // Capital letter 

            // Ports Test
            Assert.ThrowsException<ArgumentException>(() => new TestContainerOptions { Image = validImage, Name = validName }.GetContainerBuilder());
            Assert.ThrowsException<ArgumentException>(() => new TestContainerOptions { Image = validImage, Name = validName, Ports = new List<string>() }.GetContainerBuilder());  // No Ports
            Assert.ThrowsException<FormatException>(() => new TestContainerOptions { Image = validImage, Name = validName, Ports = new List<string> { "8080:" } }.GetContainerBuilder());  // No Ports
            Assert.ThrowsException<FormatException>(() => new TestContainerOptions { Image = validImage, Name = validName, Ports = new List<string> { "bad:bad" } }.GetContainerBuilder());  // No Ports
        }

        [TestMethod]
        public async Task CreateContainer()
        {            
            var builder = new TestContainerOptions 
            { 
                Image = "testcontainers/helloworld:1.1.0", 
                Name = "test_container", 
                Ports = new List<string> { "8080" } 
            }.GetContainerBuilder();

            var container = builder.Build();
                
            await container.StartAsync().ConfigureAwait(false);

            // Create a new instance of HttpClient to send HTTP requests.
            var httpClient = new HttpClient();

            // Construct the request URI by specifying the scheme, hostname, assigned random host port, and the endpoint "uuid".
            var requestUri = new UriBuilder(Uri.UriSchemeHttp, container.Hostname, container.GetMappedPublicPort(8080), "uuid").Uri;

            // Send an HTTP GET request to the specified URI and retrieve the response as a string.
            var guid = await httpClient.GetStringAsync(requestUri)
              .ConfigureAwait(false);

            // Ensure that the retrieved UUID is a valid GUID.
            Debug.Assert(Guid.TryParse(guid, out _));
        }
    }
}
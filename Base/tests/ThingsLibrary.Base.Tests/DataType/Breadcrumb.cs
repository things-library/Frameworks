using ThingsLibrary.DataType;
using System.Diagnostics.CodeAnalysis;

namespace ThingsLibrary.Tests.DataType
{
    [TestClass, ExcludeFromCodeCoverage]
    public class BreadcrumbTests
    {
        [TestMethod]
        public void Constructor()
        {
            var title = "Test Title";
            var url = "/something/something2";
            var isActive = true;


            var breadCrumb = new Breadcrumb(title, url, isActive);

            Assert.AreEqual(title, breadCrumb.Title);
            Assert.AreEqual(url, breadCrumb.Url);
            Assert.AreEqual(isActive, breadCrumb.Active);
        }
    }
}

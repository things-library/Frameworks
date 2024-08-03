using System.Collections.Generic;

namespace ThingsLibrary.Tests.DataType.Extensions
{
    [TestClass, ExcludeFromCodeCoverage]
    public class EnumerableTests
    {
        [TestMethod]        
        public void ForEach()
        {
            IEnumerable<int> items = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }.ToList();

            int i = 0;
            items.ForEach(item => 
            {
                i++;
                Assert.AreEqual(i, item);
            });            
        }
    }
}
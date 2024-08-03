using System.Collections.Generic;
using System.Data.SqlTypes;

namespace ThingsLibrary.Tests.DataType
{
    [TestClass, ExcludeFromCodeCoverage]
    public class SequentialGuidTests
    {
        [TestMethod]
        public void Validate()
        {
            List<Guid> list = new List<Guid>();            

            // generate 1000 items to test with
            for (int i = 0; i < 1000; i++)
            {
                var value = ThingsLibrary.DataType.SequentialGuid.NewGuid();

                list.Add(value);                
            }

            // .net sort will sort guids like they are strings.. but what we care about is how SQL sorts the guids which is different.
            var list2 = list.OrderBy(x => new SqlGuid(x)).ToList();

            var dictionary = new Dictionary<Guid, Guid>();  // verify we aren't getting duplicates based on the time resolution
            for (int i = 0; i < 100; i++)
            {
                dictionary.Add(list[i], list[i]);    //test to make sure we don't have duplicates as well

                Assert.AreEqual(list[i], list2[i]);
            }
        }
    }
}

using System.Collections.Generic;

namespace ThingsLibrary.Tests.DataType.Extensions
{
    [TestClass, ExcludeFromCodeCoverage]
    public class PagedListTests
    {
        public class TestClass 
        { 
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }

        [TestMethod]
        public void IQuery_ToPagedResult()
        {
            var items = new List<TestClass>();
            for (int i = 1; i <= 42; i++)
            {
                items.Add(new TestClass() { Id = i, Name = i.ToString() });
            }
            
            var query = items.Where(x => x.Id > 3).AsQueryable();

            var pagedItems = query.ToPagedResult<TestClass>(0, 5);

            Assert.AreEqual(39, pagedItems.Count);
            Assert.AreEqual(4, pagedItems.Items.First().Id);

            pagedItems = query.ToPagedResult<TestClass>(3, 5);
            Assert.AreEqual(14, pagedItems.Items.First().Id);

            // page size = 0
            pagedItems = query.ToPagedResult<TestClass>(0, 0);

            Assert.AreEqual(0, pagedItems.PageSize);
            Assert.AreEqual(39, pagedItems.Count);
        }

        [TestMethod]
        public void List_ToPagedResult()
        {
            var items = new List<TestClass>();
            for (int i = 1; i <= 42; i++)
            {
                items.Add(new TestClass() { Id = i, Name = i.ToString() });
            }

            var pagedItems = items.ToPagedResult<TestClass>(0, 5);

            Assert.AreEqual(42, pagedItems.Count);
            Assert.AreEqual(1, pagedItems.Items.First().Id);

            pagedItems = items.ToPagedResult<TestClass>(3, 5);
            Assert.AreEqual(11, pagedItems.Items.First().Id);

            // page size = 0
            pagedItems = items.ToPagedResult<TestClass>(0, 0);

            Assert.AreEqual(0, pagedItems.PageSize);
            Assert.AreEqual(42, pagedItems.Count);
        }
    }
}

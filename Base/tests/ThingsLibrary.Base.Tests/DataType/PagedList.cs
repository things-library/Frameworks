// ================================================================================
// <copyright file="PagedList.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using ThingsLibrary.DataType;


namespace ThingsLibrary.Tests.DataType
{
    [TestClass, ExcludeFromCodeCoverage]
    public class PageListTests
    {
        [TestMethod]
        public void Constructor()
        {
            // empty constructor
            var pagedList = new PagedList<TestClass>();
            Assert.AreEqual(0, pagedList.TotalPages);

            // actual logical constructor
            var page = 5;
            var pageSize = 10;
            var count = 100;
            var pageCount = (int)System.Math.Ceiling((double)count / pageSize);

            var testItems = GenerateItems(count).Skip(page * pageSize).Take(pageSize);

            pagedList = new PagedList<TestClass>(page: page, pageSize: pageSize, count: count, data: testItems);

            Assert.AreEqual(page, pagedList.CurrentPage);

            int i = page * pageSize;
            foreach (var item in pagedList)
            {
                Assert.AreEqual(i, item.Id);
                i++;
            }

            i = page * pageSize;
            pagedList.ForEach(item =>
            {
                Assert.AreEqual(i, item.Id);
                i++;
            });

            var items = pagedList.Items;
            Assert.AreEqual(pageSize, items.Count());

            Assert.AreEqual(pageCount, pagedList.TotalPages);
        }

        [TestMethod]
        public void Constructor_ByData()
        {
            var count = 100;

            var testItems = GenerateItems(count);

            var pagedList = new PagedList<TestClass>(testItems);

            Assert.AreEqual(0, pagedList.CurrentPage);

            Assert.AreEqual(count, pagedList.PageSize);
            Assert.AreEqual(count, pagedList.Count);

            
        }

        [TestMethod]
        public void Constructor_Links()
        {
            var count = 100;

            var testItems = GenerateItems(count);

            var pagedList = new PagedList<TestClass>(testItems);
            
            pagedList.Links = new Dictionary<string, object>();
            pagedList.Links.Add("Test1", 123);
            pagedList.Links.Add("Test2", 456);
            pagedList.Links.Add("Test3", 789);

            Assert.AreEqual(3, pagedList.Links.Count);
            Assert.AreEqual(456, pagedList.Links["Test2"]);
        }

        public static IEnumerable<TestClass> GenerateItems(int itemCount)
        {
            var items = new List<TestClass>(itemCount);
            for(int i = 0; i < itemCount; i++)
            {
                items.Add(new TestClass { Id = i, Name = i.ToString() });
            }
            
            return items;
        }


        [ExcludeFromCodeCoverage]
        public class TestClass
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }
    }    
}

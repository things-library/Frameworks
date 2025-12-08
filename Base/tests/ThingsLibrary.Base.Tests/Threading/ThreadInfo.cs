// ================================================================================
// <copyright file="ThreadInfo.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using ThingsLibrary.Threading;

namespace ThingsLibrary.Tests.Threading
{
    [TestClass, ExcludeFromCodeCoverage]
    public class ThreadInfoTests
    {
        [TestMethod]
        public void Constructor()
        {
            var start = 10;
            var stop = 100;

            var obj = new object();

            var info = new ThreadInfo(start, stop)
            {
                Tag = obj
            };

            Assert.AreEqual(start, info.Start);
            Assert.AreEqual(stop, info.Stop);
            Assert.AreEqual(start, info.Current);

            Assert.AreEqual(obj, info.Tag);

            info.Current++;
            Assert.AreEqual(info.Start + 1, info.Current);
        }
    }
}

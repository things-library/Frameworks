// ================================================================================
// <copyright file="Zip.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.Tests.IO
{
    [TestClass, ExcludeFromCodeCoverage]
    public class ZipTests
    {
        [TestMethod]
        public void CompressThenUncompress()
        {
            var orgText = "Hello World, how are you?";

            var compressed = ThingsLibrary.IO.Compression.ZipString.Compress(orgText);

            var testText = ThingsLibrary.IO.Compression.ZipString.Uncompress(compressed);

            Assert.AreEqual(orgText, testText);                    
        }
    }
}

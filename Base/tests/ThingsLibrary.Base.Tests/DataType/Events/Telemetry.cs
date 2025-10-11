// ================================================================================
// <copyright file="Telemetry.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using ThingsLibrary.DataType.Events;

namespace ThingsLibrary.Base.Tests.DataType.Events
{
    [TestClass, ExcludeFromCodeCoverage]
    public class TelemetryTests
    {
        [TestMethod]
        public void Basic()
        {
            var telem = new TelemetryEvent("sens", DateTime.UtcNow);

            telem.Tags.Add("gn", "Mark");
            telem.Tags.Add("cp", "Starlight");
            telem.Tags.Add("r", "1");

            var sentence = telem.ToString();

            var expectedPrefix = $"${telem.Timestamp.ToUnixTimeMilliseconds()}|{telem.Type}|";

            Assert.IsTrue(sentence.StartsWith(expectedPrefix));
            Assert.IsTrue(sentence.Contains('*'));

            var telem2 = TelemetryEvent.Parse(sentence);

            Assert.IsTrue(telem2.Tags.Count == 3);
            Assert.AreEqual(telem2.Tags["gn"], telem.Tags["gn"]);
            Assert.AreEqual(telem2.Tags["cp"], telem.Tags["cp"]);
            Assert.AreEqual(telem2.Tags["r"], telem.Tags["r"]);
        }

    }
}

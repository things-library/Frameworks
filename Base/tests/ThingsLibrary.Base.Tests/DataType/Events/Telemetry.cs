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

            telem.Attributes.Add("gn", "Mark");
            telem.Attributes.Add("cp", "Starlight");
            telem.Attributes.Add("r", "1");

            var sentence = telem.ToString();

            var expectedPrefix = $"${telem.Timestamp.ToUnixTimeMilliseconds()}|{telem.Type}|";

            Assert.IsTrue(sentence.StartsWith(expectedPrefix));
            Assert.IsTrue(sentence.Contains('*'));

            var telem2 = TelemetryEvent.Parse(sentence);

            Assert.IsTrue(telem2.Attributes.Count == 3);
            Assert.AreEqual(telem2.Attributes["gn"], telem.Attributes["gn"]);
            Assert.AreEqual(telem2.Attributes["cp"], telem.Attributes["cp"]);
            Assert.AreEqual(telem2.Attributes["r"], telem.Attributes["r"]);
        }

    }
}

// ================================================================================
// <copyright file="Telemetry.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using ThingsLibrary.Schema.Library.Extensions;

namespace ThingsLibrary.DataType
{
    public class TelemetryEntry
    {
        [JsonPropertyName("date")]
        public DateTimeOffset Timestamp { get; set; } = DateTime.UtcNow;

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("tags")]
        public Dictionary<string, string> Attributes { get; set; } = [];

        public TelemetryEntry()
        {
            //nothing
        }

        public TelemetryEntry(string type, DateTimeOffset timestamp)
        {
            this.Type = type;
            this.Timestamp = timestamp;
        }



        /// <summary>
        /// Generate the telemetry sentence
        /// </summary>
        /// <returns>Telemetry Sentence.. IE: $1724387849602|PA|r:1|s:143|p:PPE Mask|q:1|p:000*79</returns>
        public override string ToString()
        {
            var sentence = new StringBuilder($"${this.Timestamp.ToUnixTimeMilliseconds()}|{this.Type}");
            sentence.Append(string.Join(string.Empty, this.Attributes.Select(x => $"|{x.Key}:{x.Value}")));

            sentence.AppendChecksum();

            return sentence.ToString();
        }

        #region --- Static ---

        public static TelemetryEntry Parse(string telemetrySentence) 
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

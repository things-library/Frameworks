// ================================================================================
// <copyright file="Telemetry.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using ThingsLibrary.DataType.Extensions;

namespace ThingsLibrary.DataType.Events
{
    public class TelemetryEvent
    {
        [JsonPropertyName("date")]
        public DateTimeOffset Timestamp { get; set; } = DateTime.UtcNow;

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("tags")]
        public Dictionary<string, string> Tags { get; set; } = [];

        public TelemetryEvent()
        {
            //nothing
        }

        public TelemetryEvent(string type, DateTimeOffset timestamp)
        {
            Type = type;
            Timestamp = timestamp;
        }

        /// <summary>
        /// Generate the telemetry sentence
        /// </summary>
        /// <returns>Telemetry Sentence.. IE: $1724387849602|PA|r:1|s:143|p:PPE Mask|q:1|p:000*79</returns>
        public override string ToString()
        {
            var sentence = new StringBuilder($"${Timestamp.ToUnixTimeMilliseconds()}|{Type}");
            sentence.Append(string.Join(string.Empty, Tags.Select(x => $"|{x.Key}:{x.Value}")));

            sentence.AppendChecksum();

            return sentence.ToString();
        }

        #region --- Static ---

        /// <summary>
        /// 
        /// </summary>
        /// <param name="telemetrySentence">Telemetry Sentence.. IE: $1724387849602|PA|r:1|s:143|p:PPE Mask|q:1|p:000*79</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Invalid sentence</exception>        
        public static TelemetryEvent Parse(string telemetrySentence)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(telemetrySentence);

            if (!telemetrySentence.ValidateChecksum())
            {
                throw new ArgumentException("Invalid checksum");
            }
            
            int num = telemetrySentence.LastIndexOf('*');   //it has to be in the sentence since the checksum validated
            
            string[] array = telemetrySentence.Substring(1, num - 1).Split('|');
            
            var date = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(array[0]));            

            var telemetryEvent = new TelemetryEvent(array[1], date);
            
            for (int i = 2; i < array.Length; i++)
            {
                num = array[i].IndexOf(':');
                if (num < 0) { continue; }
                                
                telemetryEvent.Tags[array[i][..num]] = array[i][(num + 1)..];                               
            }

            return telemetryEvent;
        }

        #endregion
    }
}

// ================================================================================
// <copyright file="GeoJson.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.DataType.Json
{
    public class GeoJsonFeatureCollection
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = "FeatureCollection";

        [JsonPropertyName("features")]
        public List<GeoJsonFeature> Features { get; set; } = [];
    }

    public class GeoJsonFeature
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = "Feature";

        [JsonPropertyName("properties")]
        public Dictionary<string, string> Properties { get; set; } = [];

        [JsonPropertyName("geometry")]
        public GeoJsonGeometry Geometry { get; set; } = new ();
    }

    public class GeoJsonGeometry
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;   //Examples: Point, Polygon, LineString

        //TODO: can also be List<double> for a single point
        //TODO: can also be List<List<double>> for a line string
        //TODO: can also be List<List<List<double>>> for a multi line string

        [JsonPropertyName("coordinates")]
        public List<List<List<double>>> Coordinates { get; set; } = [];
    }
}

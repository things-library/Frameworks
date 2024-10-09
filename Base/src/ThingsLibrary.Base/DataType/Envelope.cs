// ================================================================================
// <copyright file="Envelope.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using ThingsLibrary.DataType.Extensions;

namespace ThingsLibrary.DataType
{
    /// <summary>
    /// Envelope of points
    /// </summary>    
    public class Envelope
    {
        [JsonIgnore]
        public double MinX { get; private set; } = double.MaxValue;
        [JsonIgnore]
        public double MaxX { get; private set; } = double.MinValue;

        [JsonIgnore]
        public double MinY { get; private set; } = double.MaxValue;        
        [JsonIgnore]
        public double MaxY { get; private set; } = double.MinValue;

        [JsonIgnore]
        public double CenterX => (this.MaxX + this.MinX) / 2;
        [JsonIgnore]
        public double CenterY => (this.MaxY + this.MinY) / 2;

        /// <summary>
        /// Coordinate System
        /// </summary>
        [JsonPropertyName("srid"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int SRID { get; set; } = default;

        /// <summary>
        /// Centroid point of the envelope extents
        /// </summary>
        [JsonPropertyName("center")]
        public (double, double) Center => (this.CenterX, this.CenterY);
        

        [JsonPropertyName("max")]
        public (double, double)? Max => (this.MaxX, this.MaxY);

        /// <summary>
        /// Min value tuple (x,y)
        /// </summary>        
        [JsonPropertyName("min")]
        public (double, double)? Min => (this.MinX, this.MinY);

        [JsonIgnore]
        public double Width => (this.MaxX != double.MinValue ? this.MaxX - this.MinX : 0);

        [JsonIgnore]
        public double Height => (this.MaxY != double.MinValue ? this.MaxY - this.MinY : 0);
    

        public Envelope(int? srid = null) 
        {
            if (srid != null && srid != default)
            {
                this.SRID = srid.Value;
            }   
        }

        public Envelope((double, double) coordinate1, (double, double) coordinate2, int? srid = null)
        {
            if (srid != null && srid != default)
            {
                this.SRID = srid.Value;
            }

            this.Add(coordinate1, srid);
            this.Add(coordinate2, srid);
        }

        public Envelope(List<(double, double)> coordinates, int? srid = null)
        {
            if (srid > 0)
            {
                this.SRID = srid.Value;
            }

            this.Add(coordinates, srid);            
        }

        public Envelope(double x1, double y1, double x2, double y2, int? srid = null)
        {
            if (srid > 0)
            {
                this.SRID = srid.Value;
            }

            this.Add(x1, y1, srid);
            this.Add(x2, y2, srid);
        }

        /// <summary>
        /// Add coordinates to the envelope
        /// </summary>
        /// <param name="x">x / Longitude</param>
        /// <param name="y">y / Latitude</param>
        /// <param name="srid">Coordinate System</param>
        /// <exception cref="ArgumentException"></exception>
        public void Add(double x, double y, int? srid = null)
        {
            // valid coordinate?
            if (x == default && y == default) { return; }

            if(srid != null && srid != default)
            {
                if (this.SRID == default)
                {
                    this.SRID = srid.Value;
                }
                else if (this.SRID != srid)
                {
                    throw new ArgumentException("All coordinates must all have the same coordinate system");
                }
            }             

            if (x < this.MinX) { this.MinX = x; }
            if (x > this.MaxX) { this.MaxX = x; }

            if (y < this.MinY) { this.MinY = y; }
            if (y > this.MaxY) { this.MaxY = y; }
        }

        /// <summary>
        /// Add envelope to this envelope
        /// </summary>
        /// <param name="envelope">Envelope to add</param>
        public void Add(Envelope envelope)
        {
            // nothing to see here folks
            if (envelope == null) { return; }
                                  
            // add the two extremes
            this.Add(envelope.MinX, envelope.MinY, envelope.SRID);
            this.Add(envelope.MaxX, envelope.MaxY, envelope.SRID);
        }

        public void Add((double, double) coordinate, int? srid = null)
        {            
            this.Add(coordinate.Item1, coordinate.Item2, srid);
        }

        public void Add(IEnumerable<(double, double)> coordinates, int? srid = null)
        {
            coordinates.ForEach(coordinate => this.Add(coordinate.Item1, coordinate.Item2, srid));            
        }

        /// <summary>
        /// Adds the buffer value to the min/max values
        /// </summary>
        /// <param name="value">Value to buffer</param>
        public void Buffer(double value)
        {
            this.MinX -= value;
            this.MinY -= value;

            this.MaxX += value;
            this.MaxY += value;
        }

        /// <summary>
        /// Buffer by x and y amounts
        /// </summary>
        /// <param name="x">X value buffer</param>
        /// <param name="y">Y value buffer</param>
        public void Buffer(double x, double y)
        {
            this.MinX -= x;
            this.MinY -= y;

            this.MaxX += x;
            this.MaxY += y;
        }


        /// <summary>
        /// Simple test for if the coordinate is inside of the envelope
        /// </summary>
        /// <param name="coordinate">(x,y) tuple</param>
        /// <returns></returns>
        public bool Contains((double, double) coordinate)
        {
            return this.Contains(coordinate.Item1, coordinate.Item2);
        }

        /// <summary>
        /// Simple test for if the x/y is inside of the envelope
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <returns></returns>
        public bool Contains(double x, double y)
        {
            return (this.MinX <= x && x <= this.MaxX) && (this.MinY <= y && y <= this.MaxY);
        }

        #region --- Static Methods ---

        /// <summary>
        /// Create a envelope based on the extents
        /// </summary>
        /// <param name="coordinate1">Longitude/Latitude (x,y)</param>
        /// <param name="coordinate2">Longitude/Latitude (x,y)</param>
        /// <param name="srid">Coordinate System</param>
        /// <returns></returns>
        public static Envelope Create((double, double) coordinate1, (double, double) coordinate2, int? srid = null)
        {
            return new Envelope(coordinate1, coordinate2, srid);            
        }

        #endregion

        public override string ToString() => JsonSerializer.Serialize(this);
    }
}

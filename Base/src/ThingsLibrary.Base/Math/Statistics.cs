namespace ThingsLibrary.Math.Statistics
{
    #region --- Basic Statistical Analysis ---

    /// <summary>
    /// Designed to be extreamly lightweight, realtime/runtime statistics calculation
    /// </summary>
    public class AnalysisEntity
    {
        #region --- Properties ---

        public byte Min { get; private set; } = byte.MinValue;
        public byte Max { get; private set; } = byte.MaxValue;
        public long Sum { get; private set; } = 0;
        public long SumOfDerivation { get; private set; } = 0;
        public long Count { get; private set; } = 0;

        public double Mean => (this.Count > 0 ? (double)this.Sum / this.Count : 0);

        public double StdDev => (this.Count > 0 ? (System.Math.Sqrt(((double)this.SumOfDerivation / this.Count) - (this.Mean * this.Mean))) : 0);

        public double StdErr => (this.Count > 0 ? (this.StdDev / System.Math.Sqrt(Count)) : 0);

        public double Variance => (this.StdDev * this.StdDev);

        public byte Range => (byte)(this.Count > 0 ? (this.Max - this.Min) : 0);

        public long[] Histogram { get; init; } = new long[byte.MaxValue];

        #endregion

        public AnalysisEntity()
        {
            //nothing
        }

        /// <summary>
        /// Initalize the analysis based on properties.
        /// </summary>
        /// <param name="min">Min Value</param>
        /// <param name="max">Max Value</param>
        /// <param name="sum">Sum</param>
        /// <param name="sumOfDerivation">Sum of Derivation</param>
        /// <param name="count">Count of values</param>
        /// <remarks>
        /// NOTE: HISTOGRAM AND MODE WILL NOT COME OVER SINCE WE DONT PULL OVER THE HISTOGRAM
        /// </remarks>
        public AnalysisEntity(byte min, byte max, long sum, long sumOfDerivation, long count)
        {
            this.Min = min;
            this.Max = max;
            this.Sum = sum;
            this.SumOfDerivation = sumOfDerivation;
            this.Count = count;

            this.Histogram = new long[byte.MaxValue];
        }

        /// <summary>
        /// Initalize using histogram data
        /// </summary>
        /// <param name="histogram">Histogram to add</param>
        public AnalysisEntity(long[] histogram)
        {
            this.AddHistogram(histogram);
        }

        /// <summary>
        /// Clone the analysis
        /// </summary>
        /// <returns>Cloned <see cref="AnalysisEntity"/></returns>
        public AnalysisEntity Clone()
        {
            return new AnalysisEntity()
            {
                Min = this.Min,
                Max = this.Max,
                Sum = this.Sum,
                SumOfDerivation = this.SumOfDerivation,
                Count = this.Count
            };
        }

        /// <summary>
        /// Add histogram data
        /// </summary>
        /// <param name="histogram">Histogram to add</param>
        /// <exception cref="ArgumentException">If histogram does not have the right length</exception>
        public void AddHistogram(long[] histogram)
        {
            if (histogram.Length != byte.MaxValue) { throw new ArgumentException($"Invalid length for histogram.  Must be {byte.MaxValue} length."); }

            // combine the different histogram sets
            int i = 0;
            foreach (var value in histogram)
            {
                this.Add((byte)i, value);
                i++;
            }
        }

        /// <summary>
        /// Add array of values
        /// </summary>
        /// <param name="values"></param>
        public void Add(byte[] values)
        {
            foreach (var value in values)
            {
                this.Add(value);
            }
        }

        /// <summary>
        /// Add Value
        /// </summary>
        /// <param name="value">Value to add</param>
        public void Add(byte value)
        {
            if (this.Count == 0)
            {
                this.Min = value;
                this.Max = value;
            }
            else
            {
                if (value > this.Max) { this.Max = value; }
                if (value < this.Min) { this.Min = value; }
            }

            this.Sum += value;
            this.SumOfDerivation += value * value;
            this.Count++;

            this.Histogram[value]++;
        }

        /// <summary>
        /// Add value with frequency
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="frequency">Frequency</param>
        public void Add(byte value, long frequency)
        {
            // nothing to see here folks
            if (frequency == 0) { return; }

            if (this.Count == 0)
            {
                this.Min = value;
                this.Max = value;
            }
            else
            {
                if (value > this.Max) { this.Max = value; }
                if (value < this.Min) { this.Min = value; }
            }

            this.Sum += value * frequency;
            this.SumOfDerivation += value * value * frequency;
            this.Count += frequency;

            this.Histogram[value] += frequency;
        }

        public void Add(AnalysisEntity analysis)
        {
            if (analysis == null) { return; }
            if (analysis.Count <= 0) { return; }

            if (this.Count == 0)
            {
                this.Min = analysis.Min;
                this.Max = analysis.Max;
            }
            else
            {
                if (analysis.Min < this.Min) { this.Min = analysis.Min; }
                if (analysis.Max > this.Max) { this.Max = analysis.Max; }
            }

            this.Sum += analysis.Sum;
            this.SumOfDerivation += analysis.SumOfDerivation;
            this.Count += analysis.Count;
                        
            // combine the different histogram sets
            int i = 0;
            foreach (var value in analysis.Histogram)
            {
                this.Histogram[i] += value;
                i++;
            }            
        }


        public byte CalculateMode()
        {            
            byte mode = 0;
            double modeCount = 0;

            byte i = 0;
            foreach (var value in this.Histogram)
            {
                if (value > modeCount)
                {
                    modeCount = value;
                    mode = i;
                }
                i++;
            }
            return mode;
        }

        public byte GetByteLimit(double stdDev)
        {
            var limit = this.GetLimit(stdDev);
            if (limit <= 0)
            {
                return 0;
            }
            else if (limit > 255)
            {
                return 255;
            }
            else
            {
                return Convert.ToByte(limit);
            }
        }

        public double GetLimit(double stdDev)
        {
            if (this.Count == 0) { return 0; }

            return this.Mean + (stdDev * this.StdDev);
        }
    }

    /// <summary>
    /// Designed to be extreamly lightweight, realtime/runtime statistics calculation
    /// </summary>
    public class AnalysisEntity16
    {
        #region --- Properties ---
        /// <summary>
        /// Min Value
        /// </summary>
        public ushort Min { get; private set; } = ushort.MinValue;
        
        /// <summary>
        /// Max Value
        /// </summary>
        public ushort Max { get; private set; } = ushort.MaxValue;
        
        /// <summary>
        /// Sum
        /// </summary>
        public long Sum { get; private set; } = 0;
        
        /// <summary>
        /// Sum of Derivation
        /// </summary>
        public long SumOfDerivation { get; private set; } = 0;
        
        /// <summary>
        /// Number of stats in collection
        /// </summary>
        public long Count { get; set; } = 0;

        /// <summary>
        /// Mean
        /// </summary>
        public double Mean => this.Count > 0 ? (double)this.Sum / this.Count : 0;

        /// <summary>
        /// Standard Deviation
        /// </summary>
        public double StdDev => (this.Count > 0 ? System.Math.Sqrt(((double)this.SumOfDerivation / this.Count) - (this.Mean * this.Mean)) : 0);

        /// <summary>
        /// Standard Error
        /// </summary>
        public double StdErr => Count > 0 ? this.StdDev / System.Math.Sqrt(this.Count) : 0;

        /// <summary>
        /// Variance
        /// </summary>
        public double Variance => (this.StdDev * this.StdDev);

        /// <summary>
        /// Range of values
        /// </summary>
        public ushort Range => (ushort)(this.Count > 0 ? (this.Max - this.Min) : 0);

        /// <summary>
        /// Histogram
        /// </summary>
        public long[] Histogram { get; init; } = new long[ushort.MaxValue];

        #endregion

        public AnalysisEntity16()
        {
            //nothing
        }

        public AnalysisEntity16(ushort min, ushort max, long sum, long sumOfDerivation, long count)
        {
            this.Min = min;
            this.Max = max;
            this.Sum = sum;
            this.SumOfDerivation = sumOfDerivation;
            this.Count = count;

            this.Histogram = new long[ushort.MaxValue];
        }

        public AnalysisEntity16(long[] histogram)
        {
            this.AddHistogram(histogram);
        }

        /// <summary>
        /// Clone the analysis
        /// </summary>
        /// <returns>Cloned <see cref="AnalysisEntity16"/></returns>
        public AnalysisEntity16 Clone()
        {
            return new AnalysisEntity16()
            {
                Min = this.Min,
                Max = this.Max,
                Sum = this.Sum,
                SumOfDerivation = this.SumOfDerivation,
                Count = this.Count
            };
        }

        public void AddHistogram(long[] histogram)
        {
            if (histogram.Length != ushort.MaxValue) { throw new ArgumentException($"Invalid length for histogram.  Must be {ushort.MaxValue} length."); }

            // add the histogram values
            int i = 0;
            foreach (var value in histogram)
            {
                this.Add((byte)i, value);
                i++;
            }
        }

        /// <summary>
        /// Add array of values
        /// </summary>
        /// <param name="values">Values to add</param>
        public void Add(ushort[] values)
        {
            foreach (var value in values)
            {
                this.Add(value);
            }
        }

        /// <summary>
        /// Add single value
        /// </summary>
        /// <param name="value">Value to add</param>
        public void Add(ushort value)
        {
            if (this.Count == 0)
            {
                this.Min = value;
                this.Max = value;
            }
            else
            {
                if (value > this.Max) { this.Max = value; }
                if (value < this.Min) { this.Min = value; }
            }

            this.Sum += value;
            this.SumOfDerivation += value * value;
            this.Count++;

            this.Histogram[value]++;
        }

        /// <summary>
        /// Add value with frequency
        /// </summary>
        /// <param name="value">Value to add</param>
        /// <param name="frequency">Number of times value occurs</param>
        public void Add(ushort value, long frequency)
        {
            // nothing to see here folks
            if (frequency == 0) { return; }

            if (Count == 0)
            {
                this.Min = value;
                this.Max = value;
            }
            else
            {
                if (value > this.Max) { this.Max = value; }
                if (value < this.Min) { this.Min = value; }
            }

            this.Sum += value * frequency;
            this.SumOfDerivation += value * value * frequency;  // value ^2 * number of times
            this.Count += frequency;

            this.Histogram[value] += frequency;
        }

        /// <summary>
        /// Combine another analysis object
        /// </summary>
        /// <param name="analysis">Analysis to combine</param>
        public void Add(AnalysisEntity16 analysis)
        {
            if (analysis == null) { return; }
            if (analysis.Count <= 0) { return; }

            if (Count == 0)
            {
                this.Min = analysis.Min;
                this.Max = analysis.Max;
            }
            else
            {
                if (analysis.Min < this.Min) { this.Min = analysis.Min; }
                if (analysis.Max > this.Max) { this.Max = analysis.Max; }
            }

            this.Sum += analysis.Sum;
            this.SumOfDerivation += analysis.SumOfDerivation;
            this.Count += analysis.Count;

            // combine the different histogram sets
            int i = 0;
            foreach (var value in analysis.Histogram)
            {
                this.Histogram[i] += value;
                i++;
            }
        }

        /// <summary>
        /// Calculate the mode
        /// </summary>
        /// <returns></returns>
        public ushort CalculateMode()
        {
            ushort mode = 0;
            double modeCount = 0;

            ushort i = 0;
            foreach (var point in this.Histogram)
            {
                if (this.Histogram[i] > modeCount)
                {
                    modeCount = this.Histogram[i];
                    mode = i;
                }
                i++;
            }
            return mode;
        }

        public ushort GetUshortLimit(double stdDev)
        {
            var limit = this.GetLimit(stdDev);
            if (limit <= ushort.MinValue)
            {
                return ushort.MinValue;
            }
            else if (limit > ushort.MaxValue)
            {
                return ushort.MaxValue;
            }
            else
            {
                return Convert.ToByte(limit);
            }
        }

        public double GetLimit(double stdDev)
        {
            if (this.Count == 0) { return 0; }

            return this.Mean + (stdDev * this.StdDev);
        }
    }

    /// <summary>
    /// Designed to be extreamly lightweight, realtime/runtime statistics calculation
    /// </summary>
    public class AnalysisEntity64
    {
        #region --- Properties ---

        public double Min { get; private set; } = double.MinValue;
        public double Max { get; private set; } = double.MaxValue;
        public double Sum { get; private set; } = 0;
        public double SumOfDerivation { get; private set; } = 0;
        public long Count { get; set; } = 0;

        public double Mean => (this.Count > 0 ? (this.Sum / this.Count) : 0);

        public double StdDev => (this.Count > 0 ? System.Math.Sqrt((this.SumOfDerivation / this.Count) - (this.Mean * this.Mean)) : 0);

        public double StdErr => (this.Count > 0 ? this.StdDev / System.Math.Sqrt(this.Count) : 0);

        public double Variance => (this.StdDev * this.StdDev);

        public double Range => (this.Count > 0 ? (this.Max - this.Min) : 0);

        #endregion

        public AnalysisEntity64()
        {

        }

        public AnalysisEntity64(double min, double max, double sum, double sumOfDerivation, long count)
        {
            this.Min = min;
            this.Max = max;
            this.Sum = sum;
            this.SumOfDerivation = sumOfDerivation;
            this.Count = count;
        }

        public AnalysisEntity64 Clone()
        {
            return new AnalysisEntity64()
            {
                Min = this.Min,
                Max = this.Max,
                Sum = this.Sum,
                SumOfDerivation = this.SumOfDerivation,
                Count = Count
            };
        }

        public void Add(double[] values)
        {
            foreach (var value in values)
            {
                this.Add(value);
            }
        }

        public void Add(double value)
        {
            if (this.Count == 0)
            {
                this.Min = value;
                this.Max = value;
            }
            else
            {
                if (value > this.Max) { this.Max = value; }
                if (value < this.Min) { this.Min = value; }
            }

            this.Sum += value;
            this.SumOfDerivation += value * value;
            this.Count++;
        }

        public void Add(double value, int frequency)
        {
            // nothing to see here folks
            if (frequency == 0) { return; }

            if (this.Count == 0)
            {
                this.Min = value;
                this.Max = value;
            }
            else
            {
                if (value > this.Max) { this.Max = value; }
                if (value < this.Min) { this.Min = value; }
            }

            this.Sum += value * frequency;            
            this.SumOfDerivation += value * value * frequency;  // value ^2 * number of times
            this.Count += frequency;
        }

        public void Add(AnalysisEntity64 analysis)
        {
            if (analysis == null) { return; }
            if (analysis.Count <= 0) { return; }

            if (this.Count == 0)
            {
                this.Min = analysis.Min;
                this.Max = analysis.Max;
            }
            else
            {
                if (analysis.Min < Min) { Min = analysis.Min; }
                if (analysis.Max > Max) { Max = analysis.Max; }
            }

            this.Sum += analysis.Sum;
            this.SumOfDerivation += analysis.SumOfDerivation;
            this.Count += analysis.Count;
        }

        public double GetLimit(double stdDev)
        {
            if (this.Count == 0) { return 0; }

            return this.Mean + (stdDev * this.StdDev);
        }

        public override string ToString() => JsonSerializer.Serialize(this);
    }

    #endregion
}

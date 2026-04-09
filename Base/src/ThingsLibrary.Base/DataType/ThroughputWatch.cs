// ================================================================================
// <copyright file="ThroughputWatch.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.DataType
{
    /// <summary>
    /// Watch designed to calculate units per time (seconds)
    /// </summary>
    [DebuggerDisplay("{ValueCount} of {TotalValue}, Remaining={ValueLeft}, Samples={SampleCount}, EllapsedTime={EllapsedTime}")]
    public class ThroughputWatch
    {
        private readonly Stopwatch _stopwatch = new ();
        private readonly Queue<long> _incrementQueue = [];
        private readonly Queue<double> _valueQueue = [];

        private readonly object _lock = new ();

        // ================================================================================
        // Basic concept
        // - Keep track of when each increment happens in ellapsed time (ms)
        // - When calculating figure out difference in time and what we are measuring (increments or value change)
        // - Store ellapsed time (ms) in the increment queue since we know each increment is 1.  This allows us to know ellapsed time and increments
        // ================================================================================

        #region --- Increments --- 

        /// <summary>
        /// Increment Count
        /// </summary>
        public int IncrementCount { get; private set; }

        /// <summary>
        /// Total expected increments
        /// </summary>
        public int TotalIncrements { get; init; } = 0; //unknown

        /// <summary>
        ///  Increments left
        /// </summary>
        public long IncrementsLeft => this.TotalIncrements - this.IncrementCount;

        #endregion  

        #region --- Value ---

        /// <summary>
        /// Current value
        /// </summary>
        public double ValueCount { get; private set; }

        /// <summary>
        /// Total expected value
        /// </summary>
        public double TotalValue { get; init; } = 0; // unknown

        /// <summary>
        ///  Value left
        /// </summary>
        public double ValueLeft => this.TotalValue - this.ValueCount;

        #endregion

        /// <summary>
        /// Window size used for estimating
        /// </summary>
        public int SlidingWindowSize { get; private set; }

        /// <summary>
        /// Number of samples in the sliding window
        /// </summary>
        public int SampleCount => _incrementQueue.Count;

        /// <summary>
        /// Total Ellapsed Time
        /// </summary>
        public TimeSpan EllapsedTime => _stopwatch.Elapsed;

        /// <summary>
        /// Set up a smart watch
        /// </summary>        
        public ThroughputWatch()
        {
            this.Reset(0);
        }

        /// <summary>
        /// Set up a throughput watch with size
        /// </summary>
        /// <param name="slidingWindowSize">Sliding calculation window to use</param>
        public ThroughputWatch(int slidingWindowSize)
        {
            this.Reset(slidingWindowSize);
        }

        /// <summary>
        /// Resets the the counters (increments, value), sliding window
        /// </summary>        
        /// <param name="slidingWindowSize">Sample size for averaging, 0 = all increments factored in</param>
        public void Reset(int slidingWindowSize)
        {
            if (slidingWindowSize == 1) { throw new ArgumentException("Windows size must be 0 (no window), or greater than 1."); }

            // Set the sliding window sample size                        
            lock (_lock)
            {
                this.SlidingWindowSize = (slidingWindowSize > 0 ? (int)slidingWindowSize : 0);

                _incrementQueue.Clear();
                _valueQueue.Clear();

                this.IncrementCount = 0;
                this.ValueCount = 0;

                _stopwatch.Restart();
            }
        }

        /// <summary>
        /// Change the size of the sliding window
        /// </summary>
        /// <param name="slidingWindowSize"></param>
        public void SetSlidingWindow(int slidingWindowSize)
        {
            lock (_lock)
            {
                if (slidingWindowSize > 0)
                {
                    this.SlidingWindowSize = slidingWindowSize;

                    while (_incrementQueue.Count > this.SlidingWindowSize)
                    {
                        _ = _incrementQueue.Dequeue();
                        _ = _valueQueue.Dequeue();
                    }
                }
                else
                {
                    this.SlidingWindowSize = 0;

                    _incrementQueue.Clear();
                    _valueQueue.Clear();
                }
            }
        }

        /// <summary>
        /// Increment not caring about value
        /// </summary>
        public void Increment()
        {
            this.Increment(0);
        }

        /// <summary>
        /// Increment the lap/index
        /// </summary>
        public void Increment(double value)
        {
            lock (_lock)
            {
                this.IncrementCount++;
                this.ValueCount += value;

                // are we keeping track of a floating window
                if (this.SlidingWindowSize > 0)
                {
                    _incrementQueue.Enqueue(_stopwatch.ElapsedMilliseconds);
                    _valueQueue.Enqueue(this.ValueCount);

                    if (_incrementQueue.Count > this.SlidingWindowSize)
                    {
                        _ = _incrementQueue.Dequeue();
                        _ = _valueQueue.Dequeue();
                    }
                }
            }
        }

        #region --- Throughput ---

        /// <summary>
        /// Calculate increase in value per second
        /// </summary>
        /// <returns>Value Per Second</returns>
        public double CalculateValuePerSecond()
        {
            if (this.IncrementCount < 2) { return 0; }

            double valueDelta;
            double timeDelta;

            if (_incrementQueue.Count > 1)
            {
                valueDelta = _valueQueue.Last() - _valueQueue.First();
                timeDelta = (_incrementQueue.Last() - _incrementQueue.First()) / 1000d;
            }
            else
            {
                valueDelta = this.ValueCount;
                timeDelta = _stopwatch.ElapsedMilliseconds / 1000d;
            }
            if (timeDelta == 0) { return 0; }

            // divide the number of increments ellapsed time by milisecond
            return valueDelta / timeDelta;  // value per second
        }

        /// <summary>
        /// The amount of seconds per value
        /// </summary>
        /// <returns>Seconds Per Value</returns>
        public double CalculateSecondsPerValue()
        {
            return 1d / this.CalculateValuePerSecond();
        }

        /// <summary>
        /// Calculate the increase of increments over time
        /// </summary>
        /// <returns>Increments per second</returns>
        public double CalculateIncrementsPerSecond()
        {
            if (this.IncrementCount < 2) { return 0; }

            double incrementDelta;
            double timeDelta; // lets just put it into seconds

            if (_incrementQueue.Count > 1)  // window usable?
            {
                incrementDelta = _incrementQueue.Count - 1; // We are calculating the time it takes to go from the oldest item to the newest item.. so we are off by 1
                timeDelta = (_incrementQueue.Last() - _incrementQueue.First()) / 1000d;
            }
            else
            {
                incrementDelta = this.IncrementCount;
                timeDelta = _stopwatch.ElapsedMilliseconds / 1000d;
            }
            if (timeDelta == 0) { return 0; }

            // divide the number of indrements by ellapsed time
            return incrementDelta / timeDelta;  // increments per second
        }

        /// <summary>
        /// Calculates how much time elapses per increment
        /// </summary>
        /// <returns>Seconds Per Increment</returns>
        public double CalculateSecondsPerIncrement()
        {
            return 1d / this.CalculateIncrementsPerSecond();    //convert units per seconds to seconds per unit
        }

        #endregion

        #region --- ETA ---

        /// <summary>
        /// Calculate Estimate Time of Completion (ETA)
        /// </summary>
        /// <returns><see cref="TimeSpan"/> of time remaining</returns>
        /// <exception cref="ArgumentException">When no total count is defined.</exception>
        public TimeSpan? CalculateETA()
        {
            if (this.TotalValue > 0)
            {
                return CalculateValueETA();
            }
            else
            {
                return CalculateIncrementETA();
            }
        }

        /// <summary>
        /// Calculate the time remaining to reach the end of the expected total increments
        /// </summary>
        /// <returns><see cref="TimeSpan"/> of time remaining</returns>
        /// <exception cref="ArgumentException">When no total increment count is defined.</exception>
        public TimeSpan? CalculateIncrementETA()
        {
            if (this.TotalIncrements == 0) { throw new ArgumentException("Unable to calculate ETA as destination is undefined."); }
            if (this.IncrementCount >= this.TotalIncrements) { return TimeSpan.Zero; } //already arrived
            if (this.IncrementCount == 0) { return null; }

            double incrementsLeft = this.TotalIncrements - this.IncrementCount;
            double incrementsPerSecond = this.CalculateIncrementsPerSecond();

            return TimeSpan.FromSeconds(incrementsLeft / incrementsPerSecond);
        }

        /// <summary>
        /// Calculate the amount of time left to reach the end of the expected value
        /// </summary>
        /// <returns><see cref="TimeSpan"/> of time remaining</returns>
        /// <exception cref="ArgumentException">When no total value is defined.</exception>
        public TimeSpan? CalculateValueETA()
        {
            if (this.TotalValue == 0) { throw new ArgumentException("Unable to calculate ETA as destination is undefined."); }
            if (this.ValueCount >= this.TotalValue) { return TimeSpan.Zero; } //already arrived
            if (this.ValueCount == 0) { return null; }

            double valueLeft = this.TotalValue - this.ValueCount;
            double valuePerSecond = this.CalculateValuePerSecond();

            return TimeSpan.FromSeconds(valueLeft / valuePerSecond);
        }

        #endregion

        public override string ToString() => JsonSerializer.Serialize(this);
    }
}

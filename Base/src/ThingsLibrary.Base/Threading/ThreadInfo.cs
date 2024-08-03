namespace ThingsLibrary.Threading
{
    /// <summary>
    /// Thread details of where the thread should start and finish
    /// </summary>
    [DebuggerDisplay("Current: {Current} (Range: {Start} - {Stop})")]
    public class ThreadInfo
    {
        [JsonPropertyName("start")]
        public int Start { get; init; }

        [JsonPropertyName("stop")]
        public int Stop { get; init; }

        [JsonPropertyName("current")]
        public int Current { get; set; }    

        [JsonIgnore]
        public object? Tag { get; set; } 

        /// <summary>
        /// Threading Information
        /// </summary>
        /// <param name="start">Start index</param>
        /// <param name="stop">End index</param>
        public ThreadInfo(int start, int stop)
        {
            this.Start = start;
            this.Stop = stop;
            this.Current = start;          
        }

    }
}

namespace ThingsLibrary.DataType
{
    [DebuggerDisplay("Cron = {Cron}, TTL = {TTL}")]
    public class Schedule
    {
        /// <summary>
        /// Time To Live (TTL)
        /// </summary>
        public TimeSpan? TTL { get; set; }

        /// <summary>
        /// CRON Schedule string
        /// </summary>
        public string Cron { get; set; } = string.Empty;


        public override string ToString()
        {
            if(this.TTL != null)
            {
                return this.TTL.ToString()!;
            }
            else
            {
                return this.Cron;
            }
        }
    }
}

namespace Core.Cloud.Storage.File
{
    public class TransferProgressStatus
    {
        public string Message { get; private set; } = "";
        public double PercentComplete { get; private set; } = 0;

        public double BytesPerSecond { get; private set; } = 0;
        public TimeSpan? EstimatedTimeLeft { get; set; } = null;

        public double Mbps => this.BytesPerSecond / (double)1048576;     //(1024x1024) == bytes to megs    

        public TransferProgressStatus(string message, double percentComplete, double bytesPerSecond, TimeSpan? timeLeft)
        {
            this.Message = message;
            this.PercentComplete = percentComplete;

            this.BytesPerSecond = bytesPerSecond;
            this.EstimatedTimeLeft = timeLeft;
        }
    }    
}

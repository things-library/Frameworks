namespace ThingsLibrary.Storage.Events
{
    public class TransferCompleteEventArgs : EventArgs
    {
        public TransferCompleteEventArgs() : base()
        {
            //nothing
        }

        public TransferCompleteEventArgs(Exception error, bool cancelled) : base()
        {
            this.Error = error;
            this.Cancelled = cancelled;
        }

        public bool Cancelled { get; init; }
        public Exception Error { get; init; }   
    }
}

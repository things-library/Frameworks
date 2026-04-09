// ================================================================================
// <copyright file="TransferCompleteEventArgs.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

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

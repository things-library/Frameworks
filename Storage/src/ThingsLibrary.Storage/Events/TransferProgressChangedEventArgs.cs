// ================================================================================
// <copyright file="TransferProgressChangedEventArgs.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.Storage.Events
{
    public class TransferProgressChangedEventArgs : EventArgs
    {
        public TransferProgressChangedEventArgs() : base()
        {
            //nothing
        }

        public TransferProgressChangedEventArgs(long bytesTransferred, long totalBytesToTransfer) : base()
        { 
            this.BytesTransferred = bytesTransferred;
            this.TotalBytesToTransfer = totalBytesToTransfer;

            if(this.TotalBytesToTransfer > 0)
            {
                this.ProgressPercentage = (float)this.BytesTransferred / (float)this.TotalBytesToTransfer;
            }            
        }

        /// <summary>
        /// Gets the percentage of transfer completed
        /// </summary>
        public float ProgressPercentage { get; init; }

        /// <summary>
        /// Gets the number of bytes transferred.
        /// </summary>        
        public long BytesTransferred { get; init; }

        /// <summary>
        /// Gets the total number of bytes in a System.Net.WebClient data upload operation.
        /// </summary>
        public long TotalBytesToTransfer { get; init; }

   
    }
}

// ================================================================================
// <copyright file="TransferProgressReport.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.Storage.Contracts
{
    public class TransferProgressReportDto
    {
        public string Message { get; set; } = "";
        public DateTime? StartedOn { get; set; } = null;

        // CURRENT FILE UPLOADING
        public long CurrentFileBytesSent { get; set; } = 0;

        public long CompletedFileBytesTransferred { get; set; }

        // TOTALS    
        public int TotalFilesToTransfer { get; set; }
        public long TotalBytesToTransfer { get; set; } = 0;

        // COUNTS        
        public long FilesTransferred { get; set; } = 0;
        public long FilesSkipped { get; set; } = 0;
        public long FilesFailed { get; set; } = 0;

        // SPEED
        public long BytesPerSecond { get; set; } = 0;
        public TimeSpan? EstimatedTimeLeft { get; set; } = null;

        // ERRORS
        public int ErrorCount { get; set; } = 0;
        public Exception LastException { get; set; } = null;
        public DateTime? LastExceptionOn { get; set; } = null;

        public TransferProgressReportDto()
        {

        }
    }
}

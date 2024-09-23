// ================================================================================
// <copyright file="TransferProgressStatus.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.Storage.Contracts
{
    public class TransferProgressStatusDto
    {
        public string Message { get; private set; } = "";
        public double PercentComplete { get; private set; } = 0;

        public double BytesPerSecond { get; private set; } = 0;
        public TimeSpan? EstimatedTimeLeft { get; set; } = null;

        public double Mbps => BytesPerSecond / 1048576;     //(1024x1024) == bytes to megs    

        public TransferProgressStatusDto(string message, double percentComplete, double bytesPerSecond, TimeSpan? timeLeft)
        {
            Message = message;
            PercentComplete = percentComplete;

            BytesPerSecond = bytesPerSecond;
            EstimatedTimeLeft = timeLeft;
        }
    }
}

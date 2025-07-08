// ================================================================================
// <copyright file="ISmsNotifications.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using ThingsLibrary.DataType;

namespace ThingsLibrary.Notification
{
    public interface ISmsNotifications
    {
        public Task<ActionResponse> SendAsync(string toAddress, string message, CancellationToken cancellationToken);
    }
}

// ================================================================================
// <copyright file="HttpRequestData.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using ThingsLibrary.DataType;

namespace ThingsLibrary.Services.AzureFunctions.Extensions
{
    public static class HttpRequestDataExtensions
    {
        public static async Task<HttpResponseData> CreateResponseAsync(this HttpRequestData request, ActionResponse actionResponse, CancellationToken cancellationToken = default)
        {
            var response = request.CreateResponse();

            await response.WriteAsJsonAsync(actionResponse, cancellationToken);

            response.StatusCode = actionResponse.StatusCode;

            return response;
        }

        public static async Task<HttpResponseData> CreateResponseAsync<T>(this HttpRequestData request, ActionResponse<T> actionResponse, CancellationToken cancellationToken = default)
        {
            var response = request.CreateResponse();

            await response.WriteAsJsonAsync(actionResponse, cancellationToken);

            response.StatusCode = actionResponse.StatusCode;

            return response;
        }
    }
}

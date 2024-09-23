// ================================================================================
// <copyright file="JsonResponse.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using ThingsLibrary.DataType.Json;

namespace ThingsLibrary.DataType.Extensions
{
    public static partial class JsonResponseExtensions
    {
        public static JsonResponse ToResponse<T>(this JsonResponseList<T> jsonResponse) 
        {
            return new JsonResponse
            {
                Type = jsonResponse.Type,
                StatusCode = jsonResponse.StatusCode,
                Title = jsonResponse.Title,

                ErrorDetails = jsonResponse.ErrorDetails,
                Errors = jsonResponse.Errors,

                TraceId = jsonResponse.TraceId
            };
        }


        public static JsonResponse<T> ToResponse<T>(this JsonResponse jsonResponse) => new (jsonResponse);
       
        public static JsonResponseList<T> ToResponseList<T>(this JsonResponse jsonResponse) => new (jsonResponse);         
    }
}

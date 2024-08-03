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

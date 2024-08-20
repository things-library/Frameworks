namespace ThingsLibrary.Services.AzureFunctions.Extensions
{
    public static class HttpRequestDataExtensions
    {
        public static async Task<HttpResponseData> CreateResponseAsync(this HttpRequestData request, JsonResponse jsonResponse, CancellationToken cancellationToken = default)
        {
            var response = request.CreateResponse();

            await response.WriteAsJsonAsync(jsonResponse, cancellationToken);

            response.StatusCode = jsonResponse.StatusCode;

            return response;
        }

        public static async Task<HttpResponseData> CreateResponseAsync<T>(this HttpRequestData request, JsonResponse<T> jsonResponse, CancellationToken cancellationToken = default)
        {
            var response = request.CreateResponse();

            await response.WriteAsJsonAsync(jsonResponse, cancellationToken);

            response.StatusCode = jsonResponse.StatusCode;

            return response;
        }
    }
}

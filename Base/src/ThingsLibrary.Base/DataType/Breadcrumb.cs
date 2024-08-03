namespace ThingsLibrary.DataType
{
    [DebuggerDisplay("Title: {Title}, Active: {Active}, Url: {Url}")]
    public class Breadcrumb
    {
        [JsonPropertyName("title")]
        public string Title { get; init; } = string.Empty;

        [JsonPropertyName("url")]
        public string Url { get; init; } = string.Empty;

        [JsonPropertyName("active")]
        public bool Active { get; init; }

        /// <summary>
        /// Breadcrumb
        /// </summary>
        public Breadcrumb()
        {
            //for serialization
        }

        public Breadcrumb(string title, string url, bool isActive = false)
        {
            this.Title = title;
            this.Url = url;
            this.Active = isActive;
        }
    }
}

namespace ChannelEngineTestClient.Services.Models
{
    internal class ItemApiResponse<T>
    {
        public T Content { get; set; }

        public int StatusCode { get; set; }

        public int? LogId { get; set; }

        public bool Success { get; set; }

        public string Message { get; set; }

        public ValidationErrors ValidationErrors { get; set; }
    }
}

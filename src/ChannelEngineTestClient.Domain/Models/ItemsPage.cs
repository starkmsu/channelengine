namespace ChannelEngineTestClient.Domain.Models
{
    public class ItemsPage<T>
    {
        public T[] Content { get; set; }

        public int Count { get; set; }

        public int TotalCount { get; set; }

        public int ItemsPerPage { get; set; }
    }
}

using ChannelEngineTestClient.Domain.Enums;

namespace ChannelEngineTestClient.Domain.Models
{
    public class ProductExtraData
    {
        public string Key { get; set; }

        public string Value { get; set; }

        public ExtraDataType Type { get; set; }

        public bool IsPublic { get; set; }
    }
}

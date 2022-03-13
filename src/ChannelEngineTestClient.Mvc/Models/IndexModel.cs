using System.Collections.Generic;

namespace ChannelEngineTestClient.Mvc.Models
{
    public class IndexModel
    {
        public List<ProductData> TopOrders { get; set; }
    }

    public class ProductData
    {
        public string Name { get; set; }

        public int Sold { get; set; }

        public string GTIN { set; get; }

        public string ProductNo { get; set; }
    }
}

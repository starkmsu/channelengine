using ChannelEngineTestClient.Domain.Enums;

namespace ChannelEngineTestClient.Domain.Models
{
    public class Product
    {
        public bool IsActive { get; set; }
        public ProductExtraData[] ExtraData { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Brand { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public string Ean { get; set; }
        public string ManufacturerProductNumber { get; set; }
        public string MerchantProductNo { get; set; }
        public int Stock { get; set; }
        public decimal Price { get; set; }
        public decimal? MSRP { get; set; }
        public decimal? PurchasePrice { get; set; }
        public VatRateType VatRateType { get; set; }
        public decimal? ShippingCost { get; set; }
        public string ShippingTime { get; set; }
        public string Url { get; set; }
        public string ImageUrl { get; set; }
        public string ExtraImageUrl1 { get; set; }
        public string ExtraImageUrl2 { get; set; }
        public string ExtraImageUrl3 { get; set; }
        public string ExtraImageUrl4 { get; set; }
        public string ExtraImageUrl5 { get; set; }
        public string ExtraImageUrl6 { get; set; }
        public string ExtraImageUrl7 { get; set; }
        public string ExtraImageUrl8 { get; set; }
        public string ExtraImageUrl9 { get; set; }
        public string CategoryTrpublic { get; set; }
    }
}

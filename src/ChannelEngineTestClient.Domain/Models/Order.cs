using System;
using System.Collections.Generic;
using ChannelEngineTestClient.Domain.Enums;

namespace ChannelEngineTestClient.Domain.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string ChannelName { get; set; }
        public int ChannelId { get; set; }
        public string GlobalChannelName { get; set; }
        public int GlobalChannelId { get; set; }
        public Shipment ChannelOrderSupport { get; set; }
        public string ChannelOrderNo { get; set; }
        public string MerchantOrderNo { get; set; }
        public OrderStatus Status { get; set; }
        public bool IsBusinessOrder { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string MerchantComment { get; set; }
        public MerchantAddress BillingAddress { get; set; }
        public MerchantAddress ShippingAddress { get; set; }
        public decimal? SubTotalInclVat { get; set; }
        public decimal? SubTotalVat { get; set; }
        public decimal? ShippingCostsVat { get; set; }
        public decimal TotalInclVat { get; set; }
        public decimal? TotalVat { get; set; }
        public decimal? OriginalSubTotalInclVat { get; set; }
        public decimal? OriginalSubTotalVat { get; set; }
        public decimal? OriginalShippingCostsInclVat { get; set; }
        public decimal? OriginalShippingCostsVat { get; set; }
        public decimal? OriginalTotalInclVat { get; set; }
        public decimal? OriginalTotalVat { get; set; }
        public OrderLine[] Lines { get; set; }
        public decimal ShippingCostsInclVat { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string CompanyRegistrationNo { get; set; }
        public string VatNo { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentReferenceNo { get; set; }
        public string CurrencyCode { get; set; }
        public DateTime OrderDate { get; set; }
        public string ChannelCustomerNo { get; set; }
        public Dictionary<string, string> ExtraData { get; set; }
    }
}

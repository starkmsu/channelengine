using ChannelEngineTestClient.Domain.Enums;

namespace ChannelEngineTestClient.Domain.Models
{
    public class MerchantAddress
  {
		public string Line1 { get; set; }
		public string Line2 { get; set; }
		public string Line3 { get; set; }
		public Gender Gender { get; set; }
		public string CompanyName { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string StreetName { get; set; }
		public string HouseNr { get; set; }
		public string HouseNrAddition { get; set; }
		public string ZipCode { get; set; }
		public string City { get; set; }
		public string Region { get; set; }
		public string CountryIso { get; set; }
		public string Original { get; set; }
	}
}

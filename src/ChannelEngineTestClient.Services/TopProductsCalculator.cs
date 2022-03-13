using System.Collections.Generic;
using System.Linq;
using ChannelEngineTestClient.Domain.Models;
using ChannelEngineTestClient.Domain.Services;

namespace ChannelEngineTestClient.Services
{
    public class TopProductsCalculator : ITopProductsCalculator
    {
        public List<(int, string, string, string)> CalculateTopProducts(List<OrderLine> orderLines, int topCount)
        {
            var groups = orderLines.GroupBy(i => i.MerchantProductNo);
            var result = new List<(int, string, string, string)>();
            foreach (var group in groups)
            {
                var sum = group.Sum(i => i.Quantity);
                var first = group.First();
                result.Add((sum, first.Description, first.Gtin, first.MerchantProductNo));
            }

            return result.OrderByDescending(i => i.Item1).Take(topCount).ToList();
        }
    }
}

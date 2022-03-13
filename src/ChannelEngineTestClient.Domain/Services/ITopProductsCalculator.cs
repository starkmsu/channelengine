using System.Collections.Generic;
using ChannelEngineTestClient.Domain.Models;

namespace ChannelEngineTestClient.Domain.Services
{
    public interface ITopProductsCalculator
    {
        List<(int, string, string, string)> CalculateTopProducts(List<OrderLine> orderLines, int topCount);
    }
}

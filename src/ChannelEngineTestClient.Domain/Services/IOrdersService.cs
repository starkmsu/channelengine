using System.Threading;
using System.Threading.Tasks;
using ChannelEngineTestClient.Domain.Enums;
using ChannelEngineTestClient.Domain.Models;

namespace ChannelEngineTestClient.Domain.Services
{
    public interface IOrdersService
    {
        Task<ItemsPage<Order>> FetchOrdersAsync(
            int pageNumber,
            OrderStatus orderStatus,
            CancellationToken cancellationTokem = default);
    }
}

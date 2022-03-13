using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ChannelEngineTestClient.Domain.Models;

namespace ChannelEngineTestClient.Domain.Services
{
    public interface IProductsService
    {
        Task<Product> GetProductAsync(string merchantProductNo, CancellationToken cancellationTokem = default);

        Task<List<Product>> GetProductsAsync(IEnumerable<string> merchantProductNos, CancellationToken cancellationTokem = default);

        Task UpdateStockAsync(string merchantProductNo, int stock, CancellationToken cancellationTokem = default);
    }
}

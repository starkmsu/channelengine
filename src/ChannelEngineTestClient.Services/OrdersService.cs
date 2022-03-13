using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ChannelEngineTestClient.Domain.Enums;
using ChannelEngineTestClient.Domain.Models;
using ChannelEngineTestClient.Domain.Services;
using ChannelEngineTestClient.Services.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ChannelEngineTestClient.Services
{
    public class OrdersService : IOrdersService
    {
        private readonly string _baseUrl;
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;
        private readonly ILogger<OrdersService> _logger;

        public OrdersService(
            string baseUrl,
            string apiKey,
            IHttpClientFactory httpClientFactory,
            ILogger<OrdersService> logger)
        {
            _baseUrl = baseUrl;
            _apiKey = apiKey;
            _httpClient = httpClientFactory.CreateClient();
            _logger = logger;
        }

        public async Task<ItemsPage<Order>> FetchOrdersAsync(
            int pageNumber,
            OrderStatus orderStatus,
            CancellationToken cancellationTokem = default)
        {
            var url = $"{_baseUrl}/v2/orders?apikey={_apiKey}&page={pageNumber}&statuses={orderStatus}";
            try
            {
                var response = await _httpClient.GetAsync(url, cancellationTokem);
                var content = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError(content);
                    throw new InvalidOperationException($"Failed to fethc orders ({response.StatusCode}: {content})");
                }

                var ordersData = JsonConvert.DeserializeObject<CollectionApiResponse<Order>>(content);
                return new ItemsPage<Order>
                {
                    Content = ordersData.Content,
                    ItemsPerPage = ordersData.ItemsPerPage,
                    Count = ordersData.Count,
                    TotalCount = ordersData.TotalCount,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                throw;
            }
        }
    }
}

using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ChannelEngineTestClient.Domain.Models;
using ChannelEngineTestClient.Domain.Services;
using ChannelEngineTestClient.Services.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ChannelEngineTestClient.Services
{
    public class ProductsService : IProductsService
    {
        private readonly string _baseUrl;
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;
        private readonly ILogger<ProductsService> _logger;

        public ProductsService(
            string baseUrl,
            string apiKey,
            IHttpClientFactory httpClientFactory,
            ILogger<ProductsService> logger)
        {
            _baseUrl = baseUrl;
            _apiKey = apiKey;
            _httpClient = httpClientFactory.CreateClient();
            _logger = logger;
        }

        public async Task<Product> GetProductAsync(string merchantProductNo, CancellationToken cancellationTokem = default)
        {
            var url = $"{_baseUrl}/v2/products/{merchantProductNo}?apikey={_apiKey}";
            try
            {
                var response = await _httpClient.GetAsync(url, cancellationTokem);
                var content = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError(content);
                    throw new InvalidOperationException($"Failed to fethc orders ({response.StatusCode}: {content})");
                }

                var productData = JsonConvert.DeserializeObject<ItemApiResponse<Product>>(content);
                return productData.Content;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                throw;
            }
        }

        public async Task UpdateStockAsync(string merchantProductNo, int stock, CancellationToken cancellationTokem = default)
        {
            var url = $"{_baseUrl}/v2/products/{merchantProductNo}?apikey={_apiKey}";

            var request = new JsonPatchDocument();
            request.Operations.Add(
                new Microsoft.AspNetCore.JsonPatch.Operations.Operation
                {
                    op = "replace",
                    value = stock,
                    path = "Stock",
                });

            var jsonRequest = JsonConvert.SerializeObject(request);

            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json-patch+json");

            var response = await _httpClient.PatchAsync(url, content, cancellationTokem);
            if (!response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogError(responseContent);
                throw new InvalidOperationException($"Failed to update product stock ({response.StatusCode}: {content})");
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ChannelEngineTestClient.Domain.Models;
using ChannelEngineTestClient.Domain.Services;
using ChannelEngineTestClient.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ChannelEngineTestClient.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var baseUrl = "https://api-dev.channelengine.net/api";
            var apiKey = "541b989ef78ccb1bad630ea5b85c6ebff9ca3322";

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddHttpClient();
            serviceCollection.AddLogging(c =>
            {
                c.AddConsole()
                    .AddFilter("System.Net", LogLevel.Warning);
            });

            serviceCollection.AddTransient<IOrdersService>(sp =>
            {
                var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
                var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
                return new OrdersService(
                    baseUrl,
                    apiKey,
                    httpClientFactory,
                    loggerFactory.CreateLogger<OrdersService>());
            });
            serviceCollection.AddTransient<IProductsService>(sp =>
            {
                var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
                var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
                return new ProductsService(
                    baseUrl,
                    apiKey,
                    httpClientFactory,
                    loggerFactory.CreateLogger<ProductsService>());
            });

            var sp = serviceCollection.BuildServiceProvider();

            var ordersService = sp.GetRequiredService<IOrdersService>();
            var productsService = sp.GetRequiredService<IProductsService>();

            var topOrders = GetTopOrdersAsync(ordersService).GetAwaiter().GetResult();
            ShowTopOrdersAsync(productsService, topOrders).GetAwaiter().GetResult();
            if (topOrders.Count > 0)
                UpdateStockAsync(productsService, topOrders.First().MerchantProductNo).GetAwaiter().GetResult();

            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();
        }

        private static async Task<List<OrderLine>> GetTopOrdersAsync(IOrdersService ordersService)
        {
            Console.WriteLine("Fetching InProgress orders...");

            int pageNumber = 1;
            int count = 0;

            var orderLines = new List<OrderLine>();

            while (true)
            {
                try
                {
                    var ordersPage = ordersService.FetchOrdersAsync(pageNumber, Domain.Enums.OrderStatus.IN_PROGRESS).GetAwaiter().GetResult();

                    Console.WriteLine($"Fetched {ordersPage.Content.Length} InProgress orders.");

                    orderLines.AddRange(ordersPage.Content.SelectMany(i => i.Lines));

                    count += ordersPage.Count;
                    if (count >= ordersPage.TotalCount)
                        break;

                    ++pageNumber;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    break;
                }
            }

            var topCount = 5;
            var topOrders = orderLines.OrderByDescending(i => i.Quantity).Take(topCount).ToList();

            return topOrders;
        }

        private static async Task ShowTopOrdersAsync(IProductsService productsService, List<OrderLine> topOrders)
        {
            Console.WriteLine($"Top {topOrders.Count} orders:");
            foreach (var orderLine in topOrders)
            {
                try
                {
                    var product = productsService.GetProductAsync(orderLine.MerchantProductNo).GetAwaiter().GetResult();

                    Console.WriteLine($"Product order quantity: {orderLine.Quantity}");
                    Console.WriteLine($"Product name: {product.Name}");
                    Console.WriteLine($"Product GTIN: {orderLine.Gtin}");
                    Console.WriteLine($"Product stock: {product.Stock}");
                    Console.WriteLine();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private static async Task UpdateStockAsync(IProductsService productsService, string merchantProductNo)
        {
            try
            {
                var stock = 25;

                await productsService.UpdateStockAsync(merchantProductNo, stock);

                var product = await productsService.GetProductAsync(merchantProductNo);
                if (product.Stock != stock)
                    Console.WriteLine($"Failed to update product {merchantProductNo} stock to {stock}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}

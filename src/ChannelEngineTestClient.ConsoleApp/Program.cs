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
            // TODO - move these 2 to appsettings.json
            var apiBaseUrl = "https://api-dev.channelengine.net/api";
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
                    apiBaseUrl,
                    apiKey,
                    httpClientFactory,
                    loggerFactory.CreateLogger<OrdersService>());
            });
            serviceCollection.AddTransient<IProductsService>(sp =>
            {
                var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
                var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
                return new ProductsService(
                    apiBaseUrl,
                    apiKey,
                    httpClientFactory,
                    loggerFactory.CreateLogger<ProductsService>());
            });
            serviceCollection.AddTransient<ITopProductsCalculator, TopProductsCalculator>();

            var sp = serviceCollection.BuildServiceProvider();

            var ordersService = sp.GetRequiredService<IOrdersService>();
            var productsService = sp.GetRequiredService<IProductsService>();
            var topProductsCalculator = sp.GetRequiredService<ITopProductsCalculator>();

            var orderLines = GetOrderLinesAsync(ordersService).GetAwaiter().GetResult();
            var topOrders = topProductsCalculator.CalculateTopProducts(orderLines, 5);
            ShowTopOrders(topOrders);
            if (topOrders.Count > 0)
                UpdateStockAsync(productsService, topOrders.First().Item4, 25).GetAwaiter().GetResult();

            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();
        }

        private static async Task<List<OrderLine>> GetOrderLinesAsync(IOrdersService ordersService)
        {
            Console.WriteLine("Fetching InProgress orders...");

            int pageNumber = 1;
            int count = 0;

            var orderLines = new List<OrderLine>();

            while (true)
            {
                try
                {
                    var ordersPage = await ordersService.FetchOrdersAsync(pageNumber, Domain.Enums.OrderStatus.IN_PROGRESS);

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

            return orderLines;
        }

        private static void ShowTopOrders(List<(int, string, string, string)> topOrders)
        {
            Console.WriteLine();
            Console.WriteLine($"Top {topOrders.Count} orders:");

            foreach (var (sold, name, gtin, _) in topOrders)
            {
                try
                {
                    Console.WriteLine($"Product name: {name}");
                    Console.WriteLine($"Product sold quantity: {sold}");
                    Console.WriteLine($"Product GTIN: {gtin}");
                    Console.WriteLine();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private static async Task UpdateStockAsync(IProductsService productsService, string merchantProductNo, int stock)
        {
            try
            {
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

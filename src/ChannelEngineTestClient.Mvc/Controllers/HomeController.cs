﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ChannelEngineTestClient.Domain.Models;
using ChannelEngineTestClient.Domain.Services;
using ChannelEngineTestClient.Mvc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ChannelEngineTestClient.Mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly IOrdersService _ordersService;
        private readonly IProductsService _productsService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            IOrdersService ordersService,
            IProductsService productsService,
            ILogger<HomeController> logger)
        {
            _ordersService = ordersService;
            _productsService = productsService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var topOrders = await GetTopOrdersAsync(5);

            var model = new IndexModel
            {
                TopOrders = GetProductData(topOrders),
            };

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> UpdateStock(string productNo)
        {
            await UpdateStockAsync(productNo, 25);

            return RedirectToAction(nameof(Index));
        }

        private async Task<List<(int, string, string, string)>> GetTopOrdersAsync(int topCount)
        {
            _logger.LogInformation("Fetching InProgress orders...");

            int pageNumber = 1;
            int count = 0;

            var orderLines = new List<OrderLine>();

            while (true)
            {
                try
                {
                    var ordersPage = await _ordersService.FetchOrdersAsync(pageNumber, Domain.Enums.OrderStatus.IN_PROGRESS);

                    _logger.LogInformation($"Fetched {ordersPage.Content.Length} InProgress orders.");

                    orderLines.AddRange(ordersPage.Content.SelectMany(i => i.Lines));

                    count += ordersPage.Count;
                    if (count >= ordersPage.TotalCount)
                        break;

                    ++pageNumber;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    break;
                }
            }

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

        private  List<ProductData> GetProductData(List<(int, string, string, string)> topOrders)
        {
            var result = new List<ProductData>(topOrders.Count);

            foreach (var (sum, name, gtin, productNo) in topOrders)
            {
                result.Add(
                    new ProductData
                    {
                        Name = name,
                        Sold = sum,
                        GTIN = gtin,
                        ProductNo = productNo,
                    });
            }

            return result;
        }

        private async Task UpdateStockAsync(string merchantProductNo, int stock)
        {
            try
            {
                await _productsService.UpdateStockAsync(merchantProductNo, stock);

                var product = await _productsService.GetProductAsync(merchantProductNo);
                if (product.Stock != stock)
                    _logger.LogWarning($"Failed to update product {merchantProductNo} stock to {stock}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}

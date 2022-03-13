using System;
using System.Collections.Generic;
using System.Linq;
using ChannelEngineTestClient.Domain.Models;
using ChannelEngineTestClient.Domain.Services;
using ChannelEngineTestClient.Services;
using NUnit.Framework;

namespace ChannelEngineTestClient.Tests
{
    public class TopProductsTests
    {
        private ITopProductsCalculator _topProductsCalculator;

        [SetUp]
        public void Setup()
        {
            _topProductsCalculator = new TopProductsCalculator();
        }

        [TestCase(6, 3, 5)]
        [TestCase(5, 4, 10)]
        [TestCase(3, 3, 1)]
        [TestCase(10, 7, 3)]
        public void GeneralTest(int linesTotalCount, int differentProductsCount, int topCount)
        {
            var lines = new List<OrderLine>(differentProductsCount);
            for (int i = 0; i < differentProductsCount; ++i)
            {
                lines.Add(
                    new OrderLine
                    {
                        Gtin = Guid.NewGuid().ToString(),
                        Quantity = 1,
                        MerchantProductNo = Guid.NewGuid().ToString(),
                        Description = "Test product " + i,
                    });
            }

            var allLines = new List<OrderLine>(linesTotalCount);
            for (int i = 0; i < linesTotalCount; ++i)
            {
                allLines.Add(lines[i % differentProductsCount]);
            }

            var topProducts = _topProductsCalculator.CalculateTopProducts(allLines, topCount);

            Assert.True(topProducts.Count > 0);
            var minCount = Math.Min(differentProductsCount, topCount);
            Assert.AreEqual(topProducts.Count, minCount);
            Assert.AreEqual(minCount, topProducts.Select(i => i.Item4).Distinct().Count());
        }

        [Test]
        public void UnsortedUniqueLinesTest()
        {
            var lines = new List<OrderLine>
            {
                new OrderLine
                {
                    Gtin = Guid.NewGuid().ToString(),
                    Quantity = 1,
                    MerchantProductNo = Guid.NewGuid().ToString(),
                    Description = "Test product 1",
                },
                new OrderLine
                {
                    Gtin = Guid.NewGuid().ToString(),
                    Quantity = 4,
                    MerchantProductNo = Guid.NewGuid().ToString(),
                    Description = "Test product 2",
                },
                new OrderLine
                {
                    Gtin = Guid.NewGuid().ToString(),
                    Quantity = 2,
                    MerchantProductNo = Guid.NewGuid().ToString(),
                    Description = "Test product 3",
                },
            };

            var topProducts = _topProductsCalculator.CalculateTopProducts(lines, 2);

            Assert.AreEqual(2, topProducts.Count);
            Assert.AreEqual(4, topProducts[0].Item1);
            Assert.AreEqual(2, topProducts[1].Item1);
        }

        [Test]
        public void UnsortedNonUniqueLinesTest()
        {
            var productNos = new List<string>
            {
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
            };
            var gtins = new List<string>
            {
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
            };
            var lines = new List<OrderLine>
            {
                new OrderLine
                {
                    Gtin = gtins[0],
                    Quantity = 1,
                    MerchantProductNo = productNos[0],
                    Description = "Test product 1",
                },
                new OrderLine
                {
                    Gtin = gtins[1],
                    Quantity = 4,
                    MerchantProductNo = productNos[1],
                    Description = "Test product 2",
                },
                new OrderLine
                {
                    Gtin = gtins[2],
                    Quantity = 2,
                    MerchantProductNo = productNos[2],
                    Description = "Test product 3",
                },
                new OrderLine
                {
                    Gtin = gtins[0],
                    Quantity = 1,
                    MerchantProductNo = productNos[0],
                    Description = "Test product 1",
                },
                new OrderLine
                {
                    Gtin = gtins[1],
                    Quantity = 3,
                    MerchantProductNo = productNos[1],
                    Description = "Test product 2",
                },
                new OrderLine
                {
                    Gtin = gtins[2],
                    Quantity = 3,
                    MerchantProductNo = productNos[2],
                    Description = "Test product 3",
                },
            };

            var topProducts = _topProductsCalculator.CalculateTopProducts(lines, 2);

            Assert.AreEqual(2, topProducts.Count);
            Assert.AreEqual(7, topProducts[0].Item1);
            Assert.AreEqual(5, topProducts[1].Item1);
        }
    }
}
using DbInfrastructure.Repositories.IRepositories;
using DbInfrastructure.Services.IServices;
using EFGenericRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using DbInfrastructure.Entities;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;

namespace DbInfrastructure.Services
{
    public class ProductService : BaseService<Product>, IProductService
    {
        private IProductRepository ProductRepository { get; set; }
        private ILogger<ProductService> _logger { get; set; }
        private IMemoryCache _cache;
        private static readonly string _productsKey = "products";

        public ProductService(
            IProductRepository baseRepository,
            ILoggerFactory loggerFactory,
            IMemoryCache cache) : base(baseRepository,cache)
        {
            this._cache = cache;
            ProductRepository = baseRepository;
            _logger = loggerFactory.CreateLogger<ProductService>();
        }

        public async Task<IEnumerable<SelectListItem>> GetProductListItems()
        {
            _logger.LogInformation("GetTypes called.");
            var products = await ProductRepository.GetAllAsync();
            var items = new List<SelectListItem>
            {
                new SelectListItem() { Value = null, Text = "All", Selected = true }
            };
            foreach (Product product in products)
            {
                items.Add(new SelectListItem() { Value = product.Id.ToString(),
                    Text = product.Name });
            }

            return items;
        }

        public async Task<List<Product>> GetAllFromCache()
        {
            List<Product> result = await _cache.GetOrCreateAsync(_productsKey, async entry =>
            {
                var  items = await GetAllAsync();
                _logger.LogInformation($"Loaded {items.Count} products");
                return items;
            });

            return result;
        }

        public void RemoveCache()
        {
            _cache.Remove(_productsKey);
        }
    }
}

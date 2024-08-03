using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace DistributedCaching.Services
{
    public class ProductService
    {
        private static List<Product> _products = new()
        {
            new Product { Id = 1, Name = "Product 1" },
            new Product { Id = 2, Name = "Product 2" },
            new Product { Id = 3, Name = "Product 3" }
        };


        private readonly ProductCacheService _cacheService;
        private readonly DistributedCacheEntryOptions _cacheOptions;

        public ProductService(ProductCacheService cacheService)
        {
            _cacheService = cacheService;
            _cacheOptions = new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(2),
            };
        }

        public async Task<Product?> GetProductById(int id)
        {
            var cacheKey = $"Product_{id}";
            var cachedProduct = await _cacheService.GetFromCacheAsync<Product>(cacheKey);

            if (cachedProduct != null)
                return cachedProduct;

            var product = _products.FirstOrDefault(p => p.Id == id);

            if (product != null)
                await _cacheService.SetCacheAsync(cacheKey, product, _cacheOptions);

            return product;
        }

        public async Task<List<Product>?> GetProducts()
        {
            var cacheKey = "Products";
            var cachedProducts = await _cacheService.GetFromCacheAsync<List<Product>>(cacheKey);

            if (cachedProducts != null)
                return cachedProducts;

            var products = _products;

            if (products != null)
                await _cacheService.SetCacheAsync(cacheKey, products, _cacheOptions);

            return products;
        }

        public async Task AddProduct(string prodName)
        {
            var product = new Product
            {
                Id = _products.Max(p => p.Id) + 1,
                Name = prodName
            };
            _products.Add(product);

            await _cacheService.RemoveCacheAsync("Products");
        }


        public async Task UpdateProduct(int id, string prodName)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                product.Name = prodName;
                var cacheKey = $"Product_{id}";
                await _cacheService.SetCacheAsync(cacheKey, product, _cacheOptions);

                var cachedProducts = await _cacheService.GetFromCacheAsync<List<Product>>("Products");
                if (cachedProducts != null)
                {
                    var updatedProducts = cachedProducts.Select(p => p.Id == id ? product : p).ToList();
                    await _cacheService.SetCacheAsync("Products", updatedProducts, _cacheOptions);
                    await _cacheService.RemoveProductFilterCacheAsync();
                }
            }
        }

        public async Task DeleteProduct(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                _products.Remove(product);
            }

            var cacheKey = $"Product_{id}";
            await _cacheService.RemoveCacheAsync(cacheKey);

            var cachedProducts = await _cacheService.GetFromCacheAsync<List<Product>>("Products");
            if (cachedProducts != null)
            {
                var updatedProducts = cachedProducts.Where(p => p.Id != id).ToList();
                await _cacheService.SetCacheAsync("Products", updatedProducts, _cacheOptions);
                await _cacheService.RemoveProductFilterCacheAsync();
            }
        }

        public async Task<List<Product>?> GetBySearch(string search)
        {
            var cacheKey = $"ProductFilter_{search}";
            var cachedProducts = await _cacheService.GetFromCacheAsync<List<Product>>(cacheKey);

            if (cachedProducts != null)
                return cachedProducts;

            var products = _products.Where(p => p.Name.ToLower().Contains(search.ToLower())).ToList();

            if (products.Count > 0)
                await _cacheService.SetCacheAsync(cacheKey, products, _cacheOptions);

            return products;
        }
    }
}

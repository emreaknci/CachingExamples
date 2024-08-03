using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace InMemoryCaching.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ProductService productService;

        public ProductsController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            productService = new ProductService();
        }

        [HttpGet]
        public IActionResult Get()
        {
            var cacheKey = "Products";
            if (!_memoryCache.TryGetValue(cacheKey, out List<Product> products))
            {
                products = productService.GetProducts();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(10));
                _memoryCache.Set(cacheKey, products, cacheEntryOptions);
            }
            return Ok(products);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var cacheKey = $"Product-{id}";
            if (!_memoryCache.TryGetValue(cacheKey, out Product product))
            {
                product = productService.GetProductById(id);
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(10));
                _memoryCache.Set(cacheKey, product, cacheEntryOptions);
            }
            return Ok(product);
        }

        [HttpPost]
        public IActionResult Post([FromBody] string prodName)
        {
            productService.AddProduct(prodName);
            _memoryCache.Remove("Products");
            return Ok();
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] string prodName)
        {
            productService.UpdateProduct(id, prodName);
            _memoryCache.Remove($"Product-{id}");
            _memoryCache.Remove("Products");
            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            productService.DeleteProduct(id);
            _memoryCache.Remove($"Product-{id}");
            _memoryCache.Remove("Products");
            return Ok();
        }

        [HttpDelete]
        public IActionResult Delete()
        {
            productService.DeleteAllProducts();
            _memoryCache.Remove("Products");
            return Ok();
        }
    }
}

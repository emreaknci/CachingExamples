namespace InMemoryCaching
{
    public class ProductService
    {
        private readonly List<Product> _products = new()
        {
            new Product { Id = 1, Name = "Product 1" },
            new Product { Id = 2, Name = "Product 2" },
            new Product { Id = 3, Name = "Product 3" }
        };

        public Product GetProductById(int id) => _products.FirstOrDefault(p => p.Id == id);


        public List<Product> GetProducts() => _products;

        public void AddProduct(string prodName)
           => _products.Add(new Product { Id = _products.Count + 1, Name = prodName });

        public void UpdateProduct(int id, string prodName)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                product.Name = prodName;
            }
        }

        public void DeleteProduct(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                _products.Remove(product);
            }
        }

        public void DeleteAllProducts() => _products.Clear();
    }
}

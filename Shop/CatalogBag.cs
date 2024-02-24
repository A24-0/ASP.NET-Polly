using System.Collections.Concurrent;

namespace Shop
{
    public class CatalogBag 
    {
        private readonly ConcurrentBag<Product> _productsBag = new()
        {
            new("Чистый код", 500m),
            new("Код, помещающийся в голове", 1000m),
            new("Рефакторинг",  100m)
        };
        public Product[] GetProductsBag()
        {
            return _productsBag.ToArray();
        }

        public Product GetProductBag(string productName)
        {
            return _productsBag.First(p => p.Name == productName);
        }

        public void AddProductsBag(Product product)
        {
            _productsBag.Add(product);
        }
        public Product RemoveProductBag(Product product)
        {
            _productsBag.TryTake(out product);
            return product;
        }

    }
}

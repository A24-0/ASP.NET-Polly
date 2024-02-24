using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Shop;
using System.Collections.Concurrent;
using System.Threading;

namespace Shop
{
    public class InMemoryCatalog: ICatalog
    {
        private readonly List<Product> _products = new()
        {
            new("Чистый код", 500m),
            new("Код, помещающийся в голове", 1000m),
            new("Рефакторинг",  100m)
        };

        public Product[] GetProducts()
        {
            return _products.ToArray();
        }

        public Product GetProduct(string productName)
        {
            return _products.First(p => p.Name == productName);
        }

        public void AddProducts(Product product)
        {
            _products.Add(product);
        }

        public void DelProducts(Product product)
        {
            _products.Remove(product);
        }

        public void PutProducts(string productName, Product product)
        {
            var existingProduct = _products.FirstOrDefault(p => p.Name == productName);
            if (existingProduct != null)
            {
                existingProduct.Name = product.Name;
                existingProduct.Price = product.Price;
            }
        }

        private SemaphoreSlim _semaphore = new SemaphoreSlim(1); //задаем количество разрешений
        public async Task<List<Product>> GetProductsAsync()
        {
            await _semaphore.WaitAsync();
            try
            {
                return _products.ToList();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Product> GetProductAsync(string productName)
        {
            await _semaphore.WaitAsync();
            try
            {
                return _products.First(p => p.Name == productName);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task AddProductsAsync(Product product)
        {
            await _semaphore.WaitAsync();
            try
            {
                _products.Add(product);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<bool> RemoveProductsAsync(Product product)
        {
            await _semaphore.WaitAsync();
            try
            {
                return _products.Remove(product);
            }
            finally
            {
                _semaphore.Release();
            }
        }
        private readonly RealClock _clock=new RealClock();

        public decimal CalculateFinalPrice(Product product)
        {
            decimal finalPrice = product.Price;

            if (_clock.Current().DayOfWeek == DayOfWeek.Monday)
            {
                finalPrice -= (finalPrice * 0.3m);
            }

            return finalPrice;
        }

        public async Task<List<Product>> GetProductsDiscountsAsync()
        {
            await _semaphore.WaitAsync();
            try
            {
                List<Product> productsWithDiscounts = new List<Product>();
                RealClock realClock = new RealClock();

                foreach (var product in _products)
                {
                    decimal finalPrice = CalculateFinalPrice(product);

                    Product discountedProduct = new Product(product.Name, finalPrice);

                    productsWithDiscounts.Add(discountedProduct);
                }

                return productsWithDiscounts;
            }
            finally
            {
                _semaphore.Release();
            }
        }

    }
}



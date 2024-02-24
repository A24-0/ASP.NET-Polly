namespace Shop
{
    public interface ICatalog
    {
        Product[] GetProducts();
        Product GetProduct(string productName);
        void AddProducts(Product product);
        void DelProducts(Product product);
        void PutProducts(string productName, Product product);

        Task<List<Product>> GetProductsAsync();
        Task<Product> GetProductAsync(string productName);
        Task AddProductsAsync(Product product);
        Task<bool> RemoveProductsAsync(Product product);

        Task<List<Product>> GetProductsDiscountsAsync();

    }
}

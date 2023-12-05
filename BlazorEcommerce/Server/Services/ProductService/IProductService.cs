namespace BlazorEcommerce.Server.Services.ProductService
{
    public interface IProductService
    {
        Task<ServiceResponse<List<Product>>> GetProductsAsync();
        Task<ServiceResponse<Product>> GetProductAsync(int productId);
        Task<ServiceResponse<List<Product>>> GetProductByCategories(string categoryUrl);
        Task<ServiceResponse<ProductSearchResult>> SearchProducts(string seacrchText , int page);
        Task<ServiceResponse<List<string>>> GetProductSearchSuggestions(string seacrchText);
        Task<ServiceResponse<List<Product>>> GetAdminProducts();
        Task<ServiceResponse<Product>> CreateProduct(Product product);
        Task<ServiceResponse<Product>> UpdateProduct(Product product);
        Task<ServiceResponse<bool>> SoftDeleteProduct(int productId);
    }
}

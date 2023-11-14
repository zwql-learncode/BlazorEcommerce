using BlazorEcommerce.Shared.Entities;

namespace BlazorEcommerce.Server.Services.ProductService
{
    public class ProductService : IProductService
    {
        private readonly DataContext _context;

        public ProductService(DataContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<Product>> GetProductAsync(int productId)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == productId);
            if (product == null) 
            {
                return new ServiceResponse<Product>
                {
                    Success = false,
                    Message = "Không tìm thấy sản phẩm"
                };
            }
            return new ServiceResponse<Product>
            {
                Data = product,
                Message = "Thành công"
            };
        }

        public async Task<ServiceResponse<List<Product>>> GetProductByCategories(string categoryUrl)
        {
            var products = await _context.Products
                .Where(p => p.Category.Url.ToLower().Contains(categoryUrl.ToLower()))
                .ToListAsync();
            if (products == null)
            {
                return new ServiceResponse<List<Product>>
                {
                    Success = false,
                    Message = "Không tìm thấy sản phẩm"
                };
            }
            return new ServiceResponse<List<Product>>
            {
                Data = products,
                Message = "Thành công"
            }; ;

        }

        public async Task<ServiceResponse<List<Product>>> GetProductsAsync()
        {
            var products = await _context.Products.ToListAsync();
            var result = new ServiceResponse<List<Product>>
            { 
                Data = products,
                Message = "Thành công"
            };
            return result;
        }
    }
}

using BlazorEcommerce.Shared.Entities;
using Microsoft.AspNetCore.Mvc.RazorPages;

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
            var product = await _context.Products
                .Include(p => p.ProductVariants)
                .ThenInclude(v => v.ProductType)
                .FirstOrDefaultAsync(p => p.Id == productId);
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
                .Include(p => p.ProductVariants)
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
            }; 

        }

        public async Task<ServiceResponse<List<Product>>> GetProductsAsync()
        {
            var products = await _context.Products
                 .Include(p => p.ProductVariants)
                 .ToListAsync();
            var result = new ServiceResponse<List<Product>>
            {
                Data = products,
                Message = "Thành công"
            };
            return result;
        }

        public async Task<ServiceResponse<List<string>>> GetProductSearchSuggestions(string seacrchText)
        {
            var products = await FindProductsBySearchText(seacrchText);

            List<string> result = new List<string>();

            foreach (var product in products)
            {
                if(product.Title.Contains(seacrchText, StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(product.Title);
                }

                if(product.Description != null)
                {
                    var punctuation = product.Description.Where(char.IsPunctuation).Distinct().ToArray();
                    var words = product.Description.Split().Select(w => w.Trim(punctuation));
                    foreach (var word in words)
                    {
                        if(word.Contains(seacrchText, StringComparison.OrdinalIgnoreCase) && !result.Contains(word))
                        {
                            result.Add(word);
                        }
                    }
                }
            }

             return new ServiceResponse<List<string>> 
             { 
                 Data = result
             };
        }

        public async Task<ServiceResponse<ProductSearchResult>> SearchProducts(string seacrchText, int page)
        {
            var pageResults = 2f;
            var pageCount = Math.Ceiling((await FindProductsBySearchText(seacrchText)).Count / pageResults);


            var products = await _context.Products
                .Where(p => p.Title.ToLower().Contains(seacrchText.ToLower())
                || p.Description.ToLower().Contains(seacrchText.ToLower()))
                .Include (p => p.ProductVariants)
                .Skip((page - 1) * (int)pageResults)
                .Take((int)pageResults)
                .ToListAsync();
                
            if (products == null)
            {
                return new ServiceResponse<ProductSearchResult>
                {
                    Success = false,
                    Message = "Không tìm thấy sản phẩm"
                };
            }
            return new ServiceResponse<ProductSearchResult>
            {
                Data = new ProductSearchResult
                {
                    Products = products,
                    CurrentPage = page,
                    Pages = (int)pageCount
                },
                Message = "Thành công"
            }; 
        }

        private async Task<List<Product>> FindProductsBySearchText(string searchText)
        {
            return await _context.Products
                                .Where(p => p.Title.ToLower().Contains(searchText.ToLower()) ||
                                    p.Description.ToLower().Contains(searchText.ToLower()))
                                .Include(p => p.ProductVariants)
                                .ToListAsync();
        }
    }
}

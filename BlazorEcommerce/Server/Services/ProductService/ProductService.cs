using BlazorEcommerce.Shared.Entities;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BlazorEcommerce.Server.Services.ProductService
{
    public class ProductService : IProductService
    {
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductService(DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ServiceResponse<Product>> CreateProduct(Product product)
        {
            foreach(var variant in product.ProductVariants)
            {
                variant.ProductType = null;
            }
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return new ServiceResponse<Product>
            {
                Data = product,
            };
        }

        public async Task<ServiceResponse<bool>> SoftDeleteProduct(int productId)
        {
            var dbProduct = await _context.Products.FindAsync(productId);
            if(dbProduct == null) 
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Data = false,
                    Message = "Product not found."
                };
            }

            dbProduct.Deleted = true;

            await _context.SaveChangesAsync();
            return new ServiceResponse<bool> 
            {
                Success = true,
                Data = true
            };
        }

        public async Task<ServiceResponse<List<Product>>> GetAdminProducts()
        {
            var products = await _context.Products
                .Where(p => !p.Deleted)
                .Include(p => p.ProductVariants.Where(pv => !pv.IsDelete))
                .ThenInclude(pv => pv.ProductType)
                .ToListAsync();
            var result = new ServiceResponse<List<Product>>
            {
                Data = products
            };
            return result;
        }

        public async Task<ServiceResponse<Product>> GetProductAsync(int productId)
        {
            Product product = null;
            if (_httpContextAccessor.HttpContext.User.IsInRole("Admin"))
            {
                product = await _context.Products
                  .Include(p => p.ProductVariants.Where(pv => !pv.IsDelete))
                  .ThenInclude(v => v.ProductType)
                  .FirstOrDefaultAsync(p => p.Id == productId && !p.Deleted);
            }
            else
            {
                product = await _context.Products
                  .Include(p => p.ProductVariants.Where(pv => pv.Visible && !pv.IsDelete))
                  .ThenInclude(v => v.ProductType)
                  .FirstOrDefaultAsync(p => p.Id == productId && !p.Deleted && p.Visible);
            }
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
                .Where(p => p.Category.Url.ToLower().Contains(categoryUrl.ToLower())
                && p.Visible && !p.Deleted)
                .Include(p => p.ProductVariants.Where(pv => pv.Visible && !pv.IsDelete))
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
                 .Where(p => p.Visible && !p.Deleted)
                 .Include(p => p.ProductVariants.Where(pv => pv.Visible && !pv.IsDelete))
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
                if (product.Title.Contains(seacrchText, StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(product.Title);
                }

                if (product.Description != null)
                {
                    var punctuation = product.Description.Where(char.IsPunctuation).Distinct().ToArray();
                    var words = product.Description.Split().Select(w => w.Trim(punctuation));
                    foreach (var word in words)
                    {
                        if (word.Contains(seacrchText, StringComparison.OrdinalIgnoreCase) && !result.Contains(word))
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
                || p.Description.ToLower().Contains(seacrchText.ToLower())
                && p.Visible && !p.Deleted)
                .Include(p => p.ProductVariants)
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

        public async Task<ServiceResponse<Product>> UpdateProduct(Product product)
        {
            var dbProduct = await _context.Products.FirstOrDefaultAsync(p => p.Id == product.Id );
            if (dbProduct == null)
            {
                return new ServiceResponse<Product>
                {
                    Success = false,
                    Message = "Product not found."
                };
            }

            dbProduct.Title = product.Title;
            dbProduct.Description = product.Description;
            dbProduct.ImageUrl = product.ImageUrl;
            dbProduct.CategoryId = product.CategoryId;
            dbProduct.Visible = product.Visible;

            foreach(var variant in product.ProductVariants)
            {
                var dbVariant = await _context.ProductVariants.SingleOrDefaultAsync(pv => pv.ProductId == variant.ProductId && pv.ProductTypeId == variant.ProductTypeId);
                if(dbVariant == null)
                {
                    variant.ProductType = null;
                    _context.ProductVariants.Add(variant);
                }
                else
                {
                    dbVariant.ProductTypeId = variant.ProductTypeId;
                    dbVariant.Price = variant.Price;
                    dbVariant.OriginalPrice = variant.OriginalPrice;
                    dbVariant.Visible = variant.Visible;
                    dbVariant.IsDelete = variant.IsDelete;
                }
            }

            await _context.SaveChangesAsync();
            return new ServiceResponse<Product>
            {
                Success = true,
                Data = product
            };
        }

        private async Task<List<Product>> FindProductsBySearchText(string searchText)
        {
            return await _context.Products
                                .Where(p => p.Title.ToLower().Contains(searchText.ToLower()) ||
                                    p.Description.ToLower().Contains(searchText.ToLower()) &&
                                    p.Visible && !p.Deleted)
                                .Include(p => p.ProductVariants)
                                .ToListAsync();
        }
    }
}

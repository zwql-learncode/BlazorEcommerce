using BlazorEcommerce.Server.Services.AuthService;
using System.Security.Claims;

namespace BlazorEcommerce.Server.Services.CartService
{
    public class CartService : ICartService
    {
        private readonly DataContext _context;
        private readonly IAuthService _authService;

        public CartService(DataContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        public async Task<ServiceResponse<List<CartItemResponseDTO>>> GetCartProducts(List<CartItem> cartItems)
        {
            var result = new ServiceResponse<List<CartItemResponseDTO>>
            {
                Data = new List<CartItemResponseDTO>()
            };

            foreach (var item in cartItems)
            {
                var product = await _context.Products
                    .Where(p => p.Id == item.ProductId)
                    .FirstOrDefaultAsync();

                if (product == null)
                {
                    continue;
                }

                var productVariant = await _context.ProductVariants
                    .Where(v => v.ProductId == item.ProductId
                        && v.ProductTypeId == item.ProductTypeId)
                    .Include(v => v.ProductType)
                    .FirstOrDefaultAsync();

                if (productVariant == null)
                {
                    continue;
                }

                var cartProduct = new CartItemResponseDTO
                {
                    ProductId = product.Id,
                    ProductTitle = product.Title,
                    ImageUrl = product.ImageUrl,
                    Price = productVariant.Price,
                    ProductTypeId = productVariant.ProductTypeId,
                    ProductTypeName = productVariant.ProductType.Name,
                    Quantity = item.Quantity
                };

                result.Data.Add(cartProduct);
            }

            return result;
        }

        public async Task<ServiceResponse<List<CartItemResponseDTO>>> StoreCartItems(List<CartItem> cartItems)
        {
            //store cart items
            cartItems.ForEach(cartItem => cartItem.UserId = _authService.GetUserId());
            _context.CartItems.AddRange(cartItems);
            await _context.SaveChangesAsync();

            //get db cart items
            return await GetDbCartProducts();
        }

        public async Task<ServiceResponse<int>> GetCartItemsCount()
        {
            var count = (await _context.CartItems
                .Where(ci => ci.UserId == _authService.GetUserId())
                .ToListAsync()).Count;
            return new ServiceResponse<int> { Data = count };   
        }

        public async Task<ServiceResponse<List<CartItemResponseDTO>>> GetDbCartProducts()
        {
            var products = await _context.CartItems
                .Where(ci => ci.UserId == _authService.GetUserId())
                .ToListAsync();
            return await GetCartProducts(products);
        }

        public async Task<ServiceResponse<bool>> AddToCart(CartItem item)
        {
            item.UserId = _authService.GetUserId();
            var sameItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.ProductId == item.ProductId && ci.ProductTypeId == item.ProductTypeId && ci.UserId == item.UserId);
            if (sameItem == null)
            {
                _context.CartItems.Add(item);
            }
            else
            {
                sameItem.Quantity += item.Quantity;
            }

            await _context.SaveChangesAsync();

            return new ServiceResponse<bool>
            {
                Data = true
            };
        }

        public async Task<ServiceResponse<bool>> UpdateQuantity(CartItem item)
        {
            var dbCartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.ProductId == item.ProductId && ci.ProductTypeId == item.ProductTypeId && ci.UserId == _authService.GetUserId());
            if(dbCartItem == null)
            {
                return new ServiceResponse<bool>
                {
                    Data = false,
                    Message = "Cart item does not exists.",
                    Success = false
                };
            }
            dbCartItem.Quantity = item.Quantity;
            await _context.SaveChangesAsync();

            return new ServiceResponse<bool> { Data = true };
        }

        public async Task<ServiceResponse<bool>> RemoveItemFromCart(int productId, int productTypeId)
        {
            var dbCartItem = await _context.CartItems
               .FirstOrDefaultAsync(ci => ci.ProductId == productId && ci.ProductTypeId == productTypeId && ci.UserId == _authService.GetUserId());
            if (dbCartItem == null)
            {
                return new ServiceResponse<bool>
                {
                    Data = false,
                    Message = "Cart item does not exists.",
                    Success = false
                };
            }
            _context.CartItems.Remove(dbCartItem);
            await _context.SaveChangesAsync();

            return new ServiceResponse<bool> { Data = true };
        }
    }
}

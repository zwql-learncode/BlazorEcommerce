using BlazorEcommerce.Client.Services.ProductService;
using System.Xml.Xsl;

namespace BlazorEcommerce.Client.Services.CartService
{
    public class CartService : ICartService
    {
        private readonly ILocalStorageService _localStorage;
        private readonly IProductService _productService;

        public event Action OnChange;
        public CartService(ILocalStorageService localStorage, IProductService productService)
        {
            _localStorage = localStorage;
            _productService = productService;
        }

        public async Task AddToCart(ProductVariant productVariant)
        {
            var cart = await _localStorage.GetItemAsync<List<ProductVariant>>("cart");
            if(cart == null)
            {
                cart = new List<ProductVariant>();
            }
            cart.Add(productVariant);
            await _localStorage.SetItemAsync("cart", cart);

            var product = await _productService.GetProduct(productVariant.ProductId);

            OnChange.Invoke();
        }

        public async Task<List<CartItemDTO>> GetCartItems()
        {
            var result = new List<CartItemDTO>();
            var cart = await _localStorage.GetItemAsync<List<ProductVariant>>("cart");
            if(cart == null)
            {
                return result;
            }
            foreach( var item in cart)
            {
                var req = await _productService.GetProduct(item.ProductId);
                var product = req.Data;
                var cartItem = new CartItemDTO
                {
                    ProductId = product.Id,
                    ProductTitle = product.Title,
                    ImageUrl = product.ImageUrl,
                    ProductTypeId = item.ProductType.Id,
                };
                var variant = product.ProductVariants.FirstOrDefault(v => v.ProductTypeId == item.ProductTypeId);
                if(variant != null)
                {
                    cartItem.ProductTypeName = variant.ProductType.Name;
                    cartItem.Price = variant.Price;
                }

                result.Add(cartItem);
            }

            return result;
        }

        public async Task DeleteItem(CartItemDTO item)
        {
            var cart = await _localStorage.GetItemAsync<List<ProductVariant>>("cart");
            if( cart == null ) 
            { 
                return; 
            }
            var cartItem = cart.FirstOrDefault(x => x.ProductId == item.ProductId && x.ProductTypeId == item.ProductTypeId);
            cart.Remove(cartItem);

            await _localStorage.SetItemAsync("cart", cart);
            OnChange.Invoke();
        }
    }
}

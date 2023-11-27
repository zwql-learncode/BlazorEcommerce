using BlazorEcommerce.Client.Services.ProductService;
using System.Net.Http.Json;
using System.Xml.Xsl;

namespace BlazorEcommerce.Client.Services.CartService
{
    public class CartService : ICartService
    {
        private readonly ILocalStorageService _localStorage;
        private readonly IProductService _productService;
        private readonly HttpClient _http;

        public event Action OnChange;
        public CartService(ILocalStorageService localStorage, IProductService productService, HttpClient http)
        {
            _localStorage = localStorage;
            _productService = productService;
            _http = http;
        }

        public async Task AddToCart(CartItemDTO cartItem)
        {
            var cart = await _localStorage.GetItemAsync<List<CartItemDTO>>("cart");
            if(cart == null)
            {
                cart = new List<CartItemDTO>();
            }

            var sameItem = cart.Find(x => x.ProductId == cartItem.ProductId && x.ProductTypeId == cartItem.ProductTypeId);
            if(sameItem == null)
            {
                cart.Add(cartItem);
            }
            else
            {
                sameItem.Quantity += cartItem.Quantity;
            }

            await _localStorage.SetItemAsync("cart", cart);

            var product = await _productService.GetProduct(cartItem.ProductId);

            OnChange.Invoke();
        }

        public async Task<List<CartItemDTO>> GetCartItems()
        {
            var cart = await _localStorage.GetItemAsync<List<CartItemDTO>>("cart");
            if(cart == null)
            {
                return new List<CartItemDTO>();
            }
            return cart;
        }

        public async Task DeleteItem(CartItemDTO item)
        {
            var cart = await _localStorage.GetItemAsync<List<CartItemDTO>>("cart");
            if( cart == null ) 
            { 
                return; 
            }
            var cartItem = cart.FirstOrDefault(x => x.ProductId == item.ProductId && x.ProductTypeId == item.ProductTypeId);
            cart.Remove(cartItem);

            await _localStorage.SetItemAsync("cart", cart);
            OnChange.Invoke();
        }

        public async Task EmptyCart()
        {
            await _localStorage.RemoveItemAsync("cart");
            OnChange.Invoke();
        }

        public async Task<string> Checkout()
        {
            var result = await _http.PostAsJsonAsync("api/payment/checkout", await GetCartItems() );
            var url = await result.Content.ReadAsStringAsync();
            return url;
        }
    }
}

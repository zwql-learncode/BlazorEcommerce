using BlazorEcommerce.Server.Services.CartService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlazorEcommerce.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _service;

        public CartController(ICartService service)
        {
            _service = service;
        }

        [HttpPost("products")]
        public async Task<ActionResult<ServiceResponse<List<CartItemResponseDTO>>>> GetCartItem(List<CartItem> cartItems)
        {
            var result = await _service.GetCartProducts(cartItems);
            return Ok(result);
        }

        [HttpPost("store-product")]
        public async Task<ActionResult<ServiceResponse<List<CartItemResponseDTO>>>> StoreCartItem(List<CartItem> cartItems)
        {
            var result = await _service.StoreCartItems(cartItems);
            return Ok(result);
        }
        [HttpPost("add")]
        public async Task<ActionResult<ServiceResponse<bool>>> AddToCart(CartItem cartItem)
        {
            var result = await _service.AddToCart(cartItem);
            return Ok(result);
        }
        [HttpPut("update-quantity")]
        public async Task<ActionResult<ServiceResponse<bool>>> UpdateQuantity(CartItem cartItem)
        {
            var result = await _service.UpdateQuantity(cartItem);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpGet("count")]
        public async Task<ActionResult<ServiceResponse<int>>> GetItemsCount()
        {
            return await _service.GetCartItemsCount();
        }
        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<CartItemResponseDTO>>>> GetDbCartProduct()
        {
            var result = await _service.GetDbCartProducts();
            return Ok(result);
        }
        [HttpDelete("{productId}/{productTypeId}")]
        public async Task<ActionResult<ServiceResponse<List<CartItemResponseDTO>>>> RemoveItemFromCart(int productId, int productTypeId) 
        {
            var result = await _service.RemoveItemFromCart(productId, productTypeId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}

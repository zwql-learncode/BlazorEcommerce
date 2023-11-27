namespace BlazorEcommerce.Client.Services.CartService
{
    public interface ICartService
    {
        event Action OnChange;
        Task AddToCart(CartItemDTO cartItem);
        Task<List<CartItemDTO>> GetCartItems();
        Task DeleteItem(CartItemDTO item);
        Task EmptyCart();
    }
}

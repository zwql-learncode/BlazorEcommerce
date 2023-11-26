namespace BlazorEcommerce.Client.Services.CartService
{
    public interface ICartService
    {
        event Action OnChange;
        Task AddToCart(ProductVariant productVariant);
        Task<List<CartItemDTO>> GetCartItems();
        Task DeleteItem(CartItemDTO item);
    }
}

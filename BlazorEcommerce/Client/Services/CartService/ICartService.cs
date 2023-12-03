namespace BlazorEcommerce.Client.Services.CartService
{
    public interface ICartService
    {
        event Action OnChange;
        Task AddToCart(CartItem cartItem);
        Task<List<CartItemResponseDTO>> GetCartProduct();
        Task DeleteItem(int productId, int productTypeId);
        Task UpdateQuantity(CartItemResponseDTO item);
        //authenticated
        Task StoreCartItems(bool emptyLocalCart);
        Task GetCartItemsCount();
    }
}

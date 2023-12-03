namespace BlazorEcommerce.Client.Services.OrderService
{
    public interface IOrderService
    {
        Task<string> PlaceOrder();
        Task<List<OrderOverViewResponseDTO>> GetOrders();
        Task<OrderDetailsResponseDTO> GetOrderDetails(int orderId);
    }
}

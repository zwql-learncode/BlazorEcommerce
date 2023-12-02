namespace BlazorEcommerce.Server.Services.OrderSerivce
{
    public interface IOrderService
    {
        Task<ServiceResponse<bool>> PlaceOrder();
        Task<ServiceResponse<List<OrderOverViewResponseDTO>>> GetOrders();
        Task<ServiceResponse<OrderDetailsResponseDTO>> GetOrdersDetails(int orderId);
    }
}

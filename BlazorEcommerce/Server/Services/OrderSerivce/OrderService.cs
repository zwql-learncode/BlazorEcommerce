using BlazorEcommerce.Server.Services.AuthService;
using BlazorEcommerce.Server.Services.CartService;
using System.Security.Claims;

namespace BlazorEcommerce.Server.Services.OrderSerivce
{
    public class OrderService : IOrderService
    {
        private readonly DataContext _context;
        private readonly ICartService _cartService;
        private readonly IAuthService _authService;

        public OrderService(DataContext context, ICartService cartService, IAuthService authService)
        {
            _context = context;
            _cartService = cartService;
            _authService = authService;
        }

        public async Task<ServiceResponse<List<OrderOverViewResponseDTO>>> GetOrders()
        {
            var res = new ServiceResponse<List<OrderOverViewResponseDTO>>();

            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.UserId == _authService.GetUserId())
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            var orderResponse = new List<OrderOverViewResponseDTO>();

            orders.ForEach(o => orderResponse.Add(new OrderOverViewResponseDTO
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                TotalPrice = o.TotalPrice,
                Product = o.OrderItems.Count > 1 ?
                $"{o.OrderItems.First().Product.Title} and " + $"{o.OrderItems.Count - 1} more..." :
                o.OrderItems.First().Product.Title,
                ProductImageUrl = o.OrderItems.First().Product.ImageUrl
            }));

            res.Data = orderResponse;

            return res;
        }

        public async Task<ServiceResponse<OrderDetailsResponseDTO>> GetOrdersDetails(int orderId)
        {
            var res = new ServiceResponse<OrderDetailsResponseDTO>();
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.ProductType)
                .Where(o => o.UserId == _authService.GetUserId() && o.Id == orderId)
                .OrderByDescending(o => o.OrderDate)
                .FirstOrDefaultAsync();

            if (order == null)
            {
                res.Success = false;
                res.Message = "Order not found";
                return res;
            }

            var orderDetailResponse = new OrderDetailsResponseDTO
            {
                OrderDate = order.OrderDate,
                TotalPrice = order.TotalPrice,
                Products = new List<OrderDetailsProductResponseDTO>(),
            };

            order.OrderItems.ForEach(item =>
            {
                orderDetailResponse.Products.Add(new OrderDetailsProductResponseDTO
                {
                    ProductId = item.ProductId,
                    ImageUrl = item.Product.ImageUrl,
                    ProductType = item.ProductType.Name,
                    Quantity = item.Quantity,
                    Title = item.Product.Title,
                    TotalPrice = item.TotalPrice
                });
            });

            res.Data = orderDetailResponse;

            return res;
        }

        public async Task<ServiceResponse<bool>> PlaceOrder(int userId)
        {
            var products = (await _cartService.GetDbCartProducts(userId)).Data;
            decimal totalPrice = 0;
            products.ForEach(product => totalPrice += product.Price * product.Quantity);

            var ordertItem = new List<OrderItem>();
            products.ForEach(product => ordertItem.Add(new OrderItem
            {
                ProductId = product.ProductId,
                ProductTypeId = product.ProductTypeId,
                Quantity = product.Quantity,
                TotalPrice = product.Quantity * product.Price,
            }));

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                TotalPrice = totalPrice,
                OrderItems = ordertItem,
            };

            _context.Orders.Add(order);

            _context.CartItems.RemoveRange(_context.CartItems
                .Where(ci => ci.UserId == userId));

            await _context.SaveChangesAsync();

            return new ServiceResponse<bool> { Data = true };
        }
    }
}

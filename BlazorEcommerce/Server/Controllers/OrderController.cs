using BlazorEcommerce.Server.Services.OrderSerivce;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlazorEcommerce.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _service;

        public OrderController(IOrderService service)
        {
            _service = service;
        }
        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<OrderOverViewResponseDTO>>>> GetOrders()
        {
            var result = await _service.GetOrders();
            return Ok(result);
        }
        [HttpGet("{orderId}")]
        public async Task<ActionResult<ServiceResponse<OrderDetailsResponseDTO>>> GetOrdersDetails(int orderId)
        {
            var result = await _service.GetOrdersDetails(orderId);
            return Ok(result);
        }
    }
}

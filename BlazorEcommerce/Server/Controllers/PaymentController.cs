using BlazorEcommerce.Server.Services.PaymentService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlazorEcommerce.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _service;

        public PaymentController(IPaymentService service)
        {
            _service = service;
        }

        [HttpPost("checkout")]
        public ActionResult CheckoutSession(List<CartItemResponseDTO> cartItems)
        {
            var session = _service.CreateCheckoutSession(cartItems);
            return Ok(session.Url);
        }
    }
  
}

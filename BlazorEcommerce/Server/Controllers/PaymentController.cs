using BlazorEcommerce.Server.Services.PaymentService;
using Microsoft.AspNetCore.Authorization;
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

        [HttpPost("checkout"), Authorize]
        public async Task<ActionResult<string>> CheckoutSession()
        {
            var session = await _service.CreateCheckoutSession();
            return Ok(session.Url);
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<bool>>> FulfillOrder()
        {
            var res = await _service.FulfillOrder(Request);
            if (!res.Success)
            {
                return BadRequest(res.Message);
            }
            return Ok(res); 
        } 
    }
  
}

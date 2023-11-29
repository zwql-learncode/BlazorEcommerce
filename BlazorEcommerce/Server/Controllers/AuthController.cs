using BlazorEcommerce.Server.Services.AuthService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlazorEcommerce.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;

        public AuthController(IAuthService service)
        {
            _service = service;
        }
        [HttpPost("register")]
        public async Task<ActionResult<ServiceResponse<int>>> Register(UserRegisterDTO req)
        {
            var userEmail = new User
            {
                Email = req.Email
            };
            var res = await _service.Register(userEmail, req.Password);

            if(!res.Success)
            {
                return BadRequest(res);
            }
            return Ok(res);
        }
        [HttpPost("login")]
        public async Task<ActionResult<ServiceResponse<string>>> Login(UserLoginDTO req)
        {
            var res = await _service.Login(req.Email, req.Password);
            if (!res.Success)
            {
                return BadRequest(res);
            }
            return Ok(res);
        }
        [HttpPost("change-password"), Authorize]
        public async Task<ActionResult<ServiceResponse<bool>>> ChangePassword([FromBody]string newPassword)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var res = await _service.ChangePassword(int.Parse(userId), newPassword);

            if (!res.Success)
            {
                return BadRequest(res);
            }
            return Ok(res);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using PostalSystem.Api.Dtos;
using PostalSystem.Api.Services;

namespace PostalSystem.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IPostmanService _postmanService;

        public AuthController(IPostmanService postmanService)
        {
            _postmanService = postmanService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _postmanService.LoginAsync(request);

            if (!result.IsSuccess)
                return Unauthorized(result);

            return Ok(result.Data);
        }
    }
}
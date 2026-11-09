using Microsoft.AspNetCore.Mvc;
using PostalSystem.Api.Dtos;
using PostalSystem.Api.Services;

namespace PostalSystem.Api.Controllers
{
    [ApiController]
    [Route("api/packages")]
    public class PackagesController : ControllerBase
    {
        private readonly IPackageService _packageService;

        public PackagesController(IPackageService packageService)
        {
            _packageService = packageService;
        }

        // Helper to get PostmanId from header
        private int GetPostmanId()
        {
            if (!Request.Headers.TryGetValue("X-Postman-Id", out var value) || !int.TryParse(value, out var id))
            {
                throw new UnauthorizedAccessException("Postman ID is missing or invalid in headers.");
            }

            return id;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterPackageRequest request)
        {
            int postmanId = GetPostmanId();
            var result = await _packageService.RegisterPackageAsync(request, postmanId);

            if (!result.IsSuccess) 
                return BadRequest(result);
            
            return CreatedAtAction(nameof(GetHistory), new { trackingGuid = (Guid)result.Data! }, result.Data);

            // var responseData = (RegisterPackageResponse)result.Data!; 
            // // `!` at the end above, tells the compiler to NOT worry about the possibility of it being null 
            // // since we already checked above it!
            // // This below is a fancier way of doing the same thing as above but avoiding the bang operator(`!`):
            // // var responseData = result.Data as RegisterPackageResponse;
            // // if (result.Data == null)
            // //     return StatusCode(500, "Internal server error: Registration data was missing.");
            
            // return CreatedAtAction(nameof(GetHistory), new { trackingGuid = responseData.TrackingGuid }, result.Data);
        }

        [HttpPost("pickup")]
        public async Task<IActionResult> Pickup(Guid trackingGuid)
        {
            int postmanId = GetPostmanId();
            var result = await _packageService.PickupPackageAsync(trackingGuid, postmanId);
            
            if (!result.IsSuccess)
                return BadRequest(result);
            
            return Ok(result);
        }

        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer(Guid trackingGuid)
        {
            int postmanId = GetPostmanId();
            var result = await _packageService.TransferPackageAsync(trackingGuid, postmanId);
            
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("confirm-delivery")]
        public async Task<IActionResult> ConfirmDelivery([FromBody] ConfirmDeliveryRequest request)
        {
            int postmanId = GetPostmanId();
            var result = await _packageService.ConfirmDeliveryAsync(request, postmanId);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("cancel")]
        public async Task<IActionResult> Cancel(Guid trackingGuid)
        {
            int postmanId = GetPostmanId();
            var result = await _packageService.CancelPackageAsync(trackingGuid, postmanId);
            
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("history/{trackingGuid}")]
        public async Task<IActionResult> GetHistory(Guid trackingGuid)
        {
            var result = await _packageService.GetPackageHistoryAsync(trackingGuid);

            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result.Data);
        }
    }
}
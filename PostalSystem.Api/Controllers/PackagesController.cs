using Microsoft.AspNetCore.Mvc;
using PostalSystem.Api.Dtos;
using PostalSystem.Api.Models;
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
        public async Task<IActionResult> Register([FromBody] RegisterPackageRequest request, [FromHeader(Name = "X-Postman-Id")] int postmanId)
        {
            var result = await _packageService.RegisterPackageAsync(request, postmanId);

            if (!result.IsSuccess) 
                return BadRequest(result.Message);

            var package = (Package)result.Data!;
            
            // REST Standard: 201 Created
            // 1. Action name: GetHistory
            // 2. Route values: the trackingGuid for the URL
            // 3. Response body: the DTO with OTP and Guid
            return CreatedAtAction(
                nameof(GetHistory), 
                new { trackingGuid = package.TrackingGuid }, 
                new RegisterPackageResponse(package.TrackingGuid, package.DeliveryOTP)
            );
        }

        [HttpPost("pickup")]
        public async Task<IActionResult> Pickup(Guid trackingGuid)
        {
            int postmanId = GetPostmanId();
            var result = await _packageService.PickupPackageAsync(trackingGuid, postmanId);
            
            if (!result.IsSuccess)
                return BadRequest(result.Message);
            
            return Ok(new { message = result.Message });
        }

        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer(Guid trackingGuid)
        {
            int postmanId = GetPostmanId();
            var result = await _packageService.TransferPackageAsync(trackingGuid, postmanId);
            
            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return Ok(new { message = result.Message });
        }

        [HttpPost("confirm-delivery")]
        public async Task<IActionResult> ConfirmDelivery([FromBody] ConfirmDeliveryRequest request)
        {
            int postmanId = GetPostmanId();
            var result = await _packageService.ConfirmDeliveryAsync(request, postmanId);

            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return Ok(new { message = result.Message });
        }

        [HttpPost("cancel")]
        public async Task<IActionResult> Cancel(Guid trackingGuid)
        {
            int postmanId = GetPostmanId();
            var result = await _packageService.CancelPackageAsync(trackingGuid, postmanId);
            
            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return Ok(new { message = result.Message });
        }

        [HttpGet("history/{trackingGuid}")]
        public async Task<IActionResult> GetHistory(Guid trackingGuid)
        {
            var result = await _packageService.GetPackageHistoryAsync(trackingGuid);

            if (!result.IsSuccess)
                return NotFound(result.Message);

            return Ok(result.Data);
        }
    }
}
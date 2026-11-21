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

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterPackageRequest request, [FromHeader(Name = "X-Postman-Id")] int postmanId)
        {
            var result = await _packageService.RegisterPackageAsync(request, postmanId);

            if (!result.IsSuccess) 
                return BadRequest(new { message = result.Message });

            // Cast to the Response DTO
            var response = (RegisterPackageResponse)result.Data!;
            
            // REST Standard: 201 Created
            // 1. Action name: GetHistory
            // 2. Route values: the trackingGuid for the URL
            // 3. Response body: the DTO with OTP and Guid
            return CreatedAtAction(
                nameof(GetHistory), 
                new { trackingGuid = response.TrackingGuid }, 
                response
            );
        }

        [HttpPost("pickup")]
        public async Task<IActionResult> Pickup([FromBody] GuidRequest request, [FromHeader(Name = "X-Postman-Id")] int postmanId)
        {
            var result = await _packageService.PickupPackageAsync(request, postmanId);
            
            if (!result.IsSuccess)
                return BadRequest(new { message = result.Message });
            
            return Ok(new { message = result.Message });
        }

        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer([FromBody] GuidRequest request, [FromHeader(Name = "X-Postman-Id")] int postmanId)
        {
            var result = await _packageService.TransferPackageAsync(request, postmanId);
            
            if (!result.IsSuccess)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }

        [HttpPost("confirm-delivery")]
        public async Task<IActionResult> ConfirmDelivery([FromBody] ConfirmDeliveryRequest request, [FromHeader(Name = "X-Postman-Id")] int postmanId)
        {
            var result = await _packageService.ConfirmDeliveryAsync(request, postmanId);

            if (!result.IsSuccess)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }

        [HttpPost("cancel")]
        public async Task<IActionResult> Cancel([FromBody] GuidRequest request, [FromHeader(Name = "X-Postman-Id")] int postmanId)
        {
            var result = await _packageService.CancelPackageAsync(request.TrackingGuid, postmanId);
            
            if (!result.IsSuccess)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }

        [HttpGet("{trackingGuid}/history")]
        public async Task<IActionResult> GetHistory(Guid trackingGuid)
        {
            var result = await _packageService.GetPackageHistoryAsync(trackingGuid);

            if (!result.IsSuccess)
                return NotFound(new { message = result.Message });

            return Ok(result.Data);
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailable([FromHeader(Name = "X-Postman-Id")] int postmanId)
        {
            var result = await _packageService.GetWarehouseInventoryAsync(postmanId);

            if (!result.IsSuccess)
                return BadRequest(new { message = result.Message });

            return Ok(result.Data);
        }

        [HttpGet("in-transit")]
        public async Task<IActionResult> GetInTransit()
        {
            var result = await _packageService.GetInTransitPackagesAsync();

            if (!result.IsSuccess)
                return BadRequest(new { message = result.Message });

            return Ok(result.Data);
        }

        [HttpGet("picked-up")]
        public async Task<IActionResult> GetPickedUp([FromHeader(Name = "X-Postman-Id")] int postmanId)
        {
            var result = await _packageService.GetPickedUpPackagesAsync(postmanId);

            if (!result.IsSuccess)
                return BadRequest(new { message = result.Message });

            return Ok(result.Data);
        }

        [HttpGet("can-be-cancelled")]
        public async Task<IActionResult> CancelPackageListAsync([FromHeader(Name = "X-Postman-Id")] int postmanId)
        {
            var result = await _packageService.GetCancellablePackagesAsync(postmanId);

            if (!result.IsSuccess)
                return BadRequest(new { message = result.Message });

            return Ok(result.Data);
        }
    }
}
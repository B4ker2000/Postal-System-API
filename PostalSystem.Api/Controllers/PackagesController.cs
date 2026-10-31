using Microsoft.AspNetCore.Mvc;
using PostalSystem.Api.Dtos;
using PostalSystem.Api.Services;

namespace PostalSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PackagesController : ControllerBase
    {
        private readonly IPackageService _packageService;

        public PackagesController(IPackageService packageService)
        {
            _packageService = packageService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterPackageRequest request)
        {
            // separate the concerns of the request and the service call later
            try
            {
                var package = await _packageService.RegisterPackageAsync(request.PostmanId, request.PackageDto);
                return Ok(new { TrackingNumber = package.TrackingGuid, Message = "Package registered successfully!" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("pickup")]
        public async Task<IActionResult> Pickup([FromBody] PickupRequest request)
        {
            // SOC needed!
            try
            {
                var package = await _packageService.PickupPackageAsync(request.PostmanId, request.TrackingGuid);
                return Ok(new { Message = "Package picked up successfully!", Status = package.CalculatedStatus.ToString() });
            }
            catch (ArgumentException ex)
            {
               return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("confirm-delivery")]
        public async Task<IActionResult> ConfirmDelivery([FromBody] ConfirmDeliveryRequest request)
        {
            var result = await _packageService.ConfirmDeliveryAsync(request);
            if (!result)
            {
                return BadRequest("Invalid tracking GUID, OTP, or package is not in a deliverable state.");
            }
            return Ok("Package delivered successfully! Quack!");
        }
    }
}
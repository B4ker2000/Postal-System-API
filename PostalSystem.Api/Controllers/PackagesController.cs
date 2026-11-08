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
        public async Task<IActionResult> Register([FromBody] RegisterPackageDto request)
        {
            var result = await _packageService.RegisterPackageAsync(request);
            if (!result.IsSuccess) 
                return BadRequest(result.Message);
            
            var responseData = (RegisterPackageResponse)result.Data!; 
            // `!` at the end above, tells the compiler to NOT worry about the possibility of it being null 
            // since we already checked above it!
            // This below is a fancier way of doing the same thing as above but avoiding the bang operator(`!`):
            // var responseData = result.Data as RegisterPackageResponse;
            // if (result.Data == null)
            //     return StatusCode(500, "Internal server error: Registration data was missing.");
            
            return CreatedAtAction(nameof(GetHistory), new { trackingGuid = responseData.TrackingGuid }, result.Data);
        }

        [HttpPost("pickup")]
        public async Task<IActionResult> Pickup([FromBody] PickupRequest request)
        {
            var result = await _packageService.PickupPackageAsync(request);
            return result.IsSuccess ? Ok(result.Message) 
                   : BadRequest(result.Message);
        }

        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer([FromBody] TransferRequest request)
        {
            var result = await _packageService.TransferPackageAsync(request);
            return result.IsSuccess ? Ok(result.Message) 
                   : BadRequest(result.Message);
        }

        [HttpPost("confirm-delivery")]
        public async Task<IActionResult> ConfirmDelivery([FromBody] ConfirmDeliveryRequest request)
        {
            var result = await _packageService.ConfirmDeliveryAsync(request);
            return result.IsSuccess ? Ok(result.Message) 
                   : BadRequest(result.Message);
        }

        [HttpPost("cancel")]
        public async Task<IActionResult> Cancel([FromBody] CancelPackageRequest request)
        {
            var result = await _packageService.CancelPackageAsync(request);
            return result.IsSuccess ? Ok(result.Message) 
                   : BadRequest(result.Message);
        }

        [HttpGet("{trackingGuid}/history")]
        public async Task<IActionResult> GetHistory(Guid trackingGuid)
        {
            var result = await _packageService.GetPackageHistoryAsync(trackingGuid);

            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result.Data);
        }
    }
}
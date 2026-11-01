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
            var result = await _packageService.RegisterPackageAsync(request.PostmanId, request.PackageDto);
            return result.IsSuccess ? Ok(result.Data) 
                   : BadRequest(result.Message);
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
    }
}
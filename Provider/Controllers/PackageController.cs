using Microsoft.AspNetCore.Mvc;
using Provider.Models;
using Provider.Services;

namespace Provider.Controllers
{
    [Route("api/packages")]
    [ApiController]
    public class PackageController(IPackageService packageService) : ControllerBase
    {
        private readonly IPackageService _packageService = packageService;

        [HttpGet]
        public async Task<IActionResult> GetPackagesAsync()
        {
            var result = await _packageService.GetAllPackagesAsync();
            if (!result.Success)
                return NotFound(result.Error);

            return Ok(result.Data);
        }

        [HttpGet("event/{eventId}")]
        public async Task<IActionResult> GetPackagesForEventAsync(string eventId)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _packageService.GetPackagesForEventAsync(eventId);
            if (!result.Success)
                return NotFound(result.Error);
            return Ok(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> AddPackagesAsync([FromBody] List<PackageRegistrationDto> packages)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _packageService.AddPackagesAsync(packages);
            if (!result.Success)
                return BadRequest(result.Error);

            return StatusCode(201, result.Data);
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePackageAsync([FromBody] Package updatedPackage)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _packageService.UpdatePackage(updatedPackage);
            if (!result.Success)
                return NotFound(result.Error);
            return Ok(result.Data);
        }

        [HttpDelete("{packageId}")]
        public async Task<IActionResult> DeletePackageAsync(string packageId)
        {
            if (string.IsNullOrEmpty(packageId))
                return BadRequest("Package ID cannot be null or empty.");
            var result = await _packageService.DeletePackage(packageId);
            if (!result.Success)
                return NotFound(result.Error);
            return NoContent();
        }
    }
}

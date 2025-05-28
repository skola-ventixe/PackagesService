using Microsoft.AspNetCore.Mvc;
using Provider.Services;

namespace Provider.Controllers
{
    [Route("api/packages")]
    [ApiController]
    public class PackageController(PackageService packageService) : ControllerBase
    {
        private readonly PackageService _packageService = packageService;

        [HttpGet]
        public async Task<IActionResult> GetPackagesAsync()
        {
            var result = await _packageService.GetAllPackagesAsync();
            if (!result.Success)
                return NotFound(result.Error);

            return Ok(result.Data);
        }
    }
}

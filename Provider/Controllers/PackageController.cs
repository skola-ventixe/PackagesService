using Microsoft.AspNetCore.Mvc;

namespace Provider.Controllers
{
    [Route("api/packages")]
    [ApiController]
    public class PackageController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetPackages()
        {
            // This method should return a list of packages.
            // For now, we return a placeholder response.
            return Ok(new { Message = "This endpoint will return packages." });
        }
    }
}

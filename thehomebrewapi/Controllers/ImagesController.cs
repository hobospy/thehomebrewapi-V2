using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;
using thehomebrewapi.Helpers;

namespace thehomebrewapi.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/images")]
    public class ImagesController : ExtendedControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<ImagesController> _logger;

        public ImagesController(IWebHostEnvironment environment,
            ILogger<ImagesController> logger)
        {
            _environment = environment;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetImage(string imageName)
        {
            if (string.IsNullOrEmpty(imageName))
            {
                return NotFound();
            }

            var imagePath = "";

            if (_environment.EnvironmentName == Environments.Development)
                imagePath = @"C:\Users\cclar\Documents";
            else
                imagePath = _environment.ContentRootPath;

            imagePath = Path.Combine(imagePath, "HomebrewImages", imageName);

            if (!System.IO.File.Exists(imagePath))
            {
                return NotFound();
            }

            return PhysicalFile(imagePath, "image/png");
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace Explorer.API.Controllers
{
    [Authorize(Policy = "registeredUserPolicy")]
    [Route("api/images")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetExtension(file.FileName);
            var imagesPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "profile");
            var filePath = Path.Combine(imagesPath, uniqueFileName);

            if (!Directory.Exists(imagesPath))
            {
                Directory.CreateDirectory(imagesPath);
            }

            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var fileUrl = Path.Combine("/images/profile", uniqueFileName).Replace("\\", "/");
            var absoluteUrl = $"{Request.Scheme}://{Request.Host}{fileUrl}";

            return Ok(new { url = absoluteUrl });
        }
    }
}

using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace Microsoft.Azure.SignalR.VideoChat
{
    public class FileController : Controller
    {
        [Authorize]
        [HttpGet("/{*path}")]
        public IActionResult GetFile(string path)
        {
            var file = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", path);
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(path, out string contentType))
            {
                contentType = "application/octet-stream";
            }

            return PhysicalFile(file, contentType);
        }
    }
}

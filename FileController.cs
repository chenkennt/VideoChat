using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Microsoft.Azure.SignalR.VideoChat
{
    public class FileController : Controller
    {
        [Authorize]
        [HttpGet("/{*path}")]
        public IActionResult GetFile(string path)
        {
            var file = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", path);
            return PhysicalFile(file, "text/html");
        }
    }
}

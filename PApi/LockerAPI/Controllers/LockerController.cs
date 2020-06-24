using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using WebApi.Services;
using WebApi.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("Locker")]
    public class LockerController : ControllerBase
    {
        private readonly IS3Service _S3Service;

        public LockerController(IS3Service S3Service)
        {
            _S3Service = S3Service;
        }

        [HttpPost("{LockerName}")]
        public async Task<IActionResult> CreateLocker([FromRoute] string LockerName)
        {
            var response = await _S3Service.CreateLockerAsync(LockerName);
            return Ok(response);
        }

        
        [HttpPost("AddPodule/{LockerName}/{FileName}")]
        public async Task<IActionResult> AddFile([FromRoute] string LockerName, [FromRoute] string FileName, IFormFile File)
        {

            await _S3Service.AddPoduleAsync(LockerName, FileName, File);
            return Ok();
        }

        
        [HttpGet("GetPodule/{LockerName}/{FileName}")]
        public async Task<IActionResult> GetObjectAsync([FromRoute] string LockerName, [FromRoute] string FileName)
        {
            byte[] Podule = await _S3Service.GetPoduleAsync(LockerName, FileName);

            return File(Podule, "application/zip");
        }
    }
}

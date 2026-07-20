using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuanLyHoaLan.API.Models.Requests;
using QuanLyHoaLan.Application.Features.Images.Commands.UploadImage;
using QuanLyHoaLan.Application.Features.Images.Commands.DeleteImage;

namespace QuanLyHoaLan.API.Controllers;

[ApiVersion("1.0")]
public class ImagesController : BaseController
{
    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadImage([FromForm] UploadImageRequest request)
    {
        if (request.File == null || request.File.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        using var stream = request.File.OpenReadStream();
        var command = new UploadImageCommand 
        { 
            FileStream = stream, 
            FileName = request.File.FileName
        };

        var result = await Mediator.Send(command);
        return OkResult(result);
    }

    [HttpDelete("{publicId}")]
    public async Task<IActionResult> DeleteImage(string publicId)
    {
        var command = new DeleteImageCommand(publicId);
        var result = await Mediator.Send(command);
        return OkResult(result);
    }
}

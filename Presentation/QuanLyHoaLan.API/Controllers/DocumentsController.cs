using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuanLyHoaLan.Application.DTOs.Document;
using QuanLyHoaLan.Application.Features.Documents.Commands.DeleteDocument;
using QuanLyHoaLan.Application.Features.Documents.Commands.UploadDocument;
using QuanLyHoaLan.Application.Features.Documents.Queries.GetDocuments;
using QuanLyHoaLan.Application.Common.Models;
using QuanLyHoaLan.API.Models.Requests;

namespace QuanLyHoaLan.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DocumentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public DocumentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedList<AppDocumentDto>>> GetDocuments([FromQuery] GetDocumentsQuery query)
    {
        return await _mediator.Send(query);
    }

    [HttpPost("upload")]
    [Authorize]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<AppDocumentDto>> UploadDocument([FromForm] UploadDocumentRequest request)
    {
        if (request.File == null || request.File.Length == 0)
            return BadRequest("File is empty.");

        using var stream = request.File.OpenReadStream();
        var command = new UploadDocumentCommand 
        { 
            Title = request.Title,
            Description = request.Description,
            FileStream = stream,
            FileName = request.File.FileName
        };
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult> DeleteDocument(Guid id)
    {
        await _mediator.Send(new DeleteDocumentCommand { Id = id });
        return NoContent();
    }
}

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuanLyHoaLan.Application.DTOs.Document;
using QuanLyHoaLan.Application.Features.Documents.Commands.DeleteDocument;
using QuanLyHoaLan.Application.Features.Documents.Commands.UploadDocument;
using QuanLyHoaLan.Application.Features.Documents.Commands.UpdateDocument;
using QuanLyHoaLan.Application.Features.Documents.Queries.GetDocumentById;
using QuanLyHoaLan.Application.Features.Documents.Queries.GetDocuments;
using QuanLyHoaLan.Application.Common.Models;
using QuanLyHoaLan.API.Models.Requests;

namespace QuanLyHoaLan.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class DocumentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public DocumentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<PaginatedList<AppDocumentDto>>> GetDocuments([FromQuery] GetDocumentsQuery query)
    {
        return await _mediator.Send(query);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<AppDocumentDto>> GetDocumentById(Guid id)
    {
        return await _mediator.Send(new GetDocumentByIdQuery(id));
    }

    [HttpPost("upload")]
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
            FileName = request.File.FileName,
            CategoryId = request.CategoryId
        };
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdateDocument(
        Guid id,
        [FromBody] UpdateDocumentCommand command)
    {
        if (command.Id != Guid.Empty && command.Id != id)
        {
            return BadRequest("ID trên URL và body không khớp.");
        }

        command.Id = id;
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteDocument(Guid id)
    {
        await _mediator.Send(new DeleteDocumentCommand { Id = id });
        return NoContent();
    }
}

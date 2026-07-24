using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyHoaLan.Application.Features.DocumentCategories.Commands.CreateDocumentCategory;
using QuanLyHoaLan.Application.Features.DocumentCategories.Commands.DeleteDocumentCategory;
using QuanLyHoaLan.Application.Features.DocumentCategories.Commands.UpdateDocumentCategory;
using QuanLyHoaLan.Application.Features.DocumentCategories.Queries.GetDocumentCategories;
using QuanLyHoaLan.Application.Features.DocumentCategories.Queries.GetDocumentCategoryById;
using QuanLyHoaLan.Application.Features.DocumentCategories.Queries.GetDocumentCategoryTree;
using QuanLyHoaLan.Application.Features.Documents.Queries.GetDocuments;

namespace QuanLyHoaLan.API.Controllers;

[ApiController]
[Route("api/document-categories")]
[Authorize(Roles = "Admin")]
public class DocumentCategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public DocumentCategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetCategories([FromQuery] GetDocumentCategoriesQuery query)
    {
        return Ok(await _mediator.Send(query));
    }

    [HttpGet("tree")]
    [AllowAnonymous]
    public async Task<IActionResult> GetTree()
    {
        return Ok(await _mediator.Send(new GetDocumentCategoryTreeQuery()));
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id)
    {
        return Ok(await _mediator.Send(new GetDocumentCategoryByIdQuery(id)));
    }

    [HttpGet("{id:guid}/documents")]
    [AllowAnonymous]
    public async Task<IActionResult> GetDocumentsByCategory(
        Guid id,
        [FromQuery] GetDocumentsQuery query)
    {
        query.CategoryId = id;
        return Ok(await _mediator.Send(query));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDocumentCategoryCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateDocumentCategoryCommand command)
    {
        if (command.Id != Guid.Empty && command.Id != id)
        {
            return BadRequest("ID trên URL và body không khớp.");
        }

        command.Id = id;
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteDocumentCategoryCommand(id));
        return NoContent();
    }
}

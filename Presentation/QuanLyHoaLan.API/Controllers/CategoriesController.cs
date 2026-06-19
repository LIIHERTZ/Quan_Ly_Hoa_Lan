using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using QuanLyHoaLan.Application.Features.Categories.Commands.CreateCategory;
using QuanLyHoaLan.Application.Features.Categories.Commands.UpdateCategory;
using QuanLyHoaLan.Application.Features.Categories.Commands.DeleteCategory;
using QuanLyHoaLan.Application.Features.Categories.Queries.GetCategories;
using QuanLyHoaLan.Application.Features.Categories.Queries.GetCategoryById;

using Microsoft.AspNetCore.Authorization;

namespace QuanLyHoaLan.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Roles = "Admin")]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetCategories([FromQuery] GetCategoriesQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCategoryById(Guid id)
    {
        var result = await _mediator.Send(new GetCategoryByIdQuery(id));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetCategoryById), new { id = result }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] UpdateCategoryCommand command)
    {
        if (command.Id != Guid.Empty && command.Id != id)
        {
            return BadRequest("ID không khớp.");
        }
        command.Id = id;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(Guid id)
    {
        var result = await _mediator.Send(new DeleteCategoryCommand(id));
        return Ok(result);
    }
}

using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyHoaLan.Application.Features.Articles.Commands.CreateArticle;
using QuanLyHoaLan.Application.Features.Articles.Commands.DeleteArticle;
using QuanLyHoaLan.Application.Features.Articles.Commands.UpdateArticle;
using QuanLyHoaLan.Application.Features.Articles.Queries.GetArticleById;
using QuanLyHoaLan.Application.Features.Articles.Queries.GetArticles;
using QuanLyHoaLan.Domain.Constants;

namespace QuanLyHoaLan.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ArticlesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ArticlesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetArticles([FromQuery] GetArticlesQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetArticleById(Guid id)
    {
        var query = new GetArticleByIdQuery(id);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateArticle([FromBody] CreateArticleCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetArticleById), new { id = result }, result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateArticle(Guid id, [FromBody] UpdateArticleCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("ID in URL and body must match.");
        }

        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteArticle(Guid id)
    {
        var command = new DeleteArticleCommand(id);
        await _mediator.Send(command);
        return NoContent();
    }
}

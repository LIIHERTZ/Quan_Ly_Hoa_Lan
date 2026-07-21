using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyHoaLan.Application.Features.ArticleCategories.Commands.CreateArticleCategory;
using QuanLyHoaLan.Application.Features.ArticleCategories.Commands.DeleteArticleCategory;
using QuanLyHoaLan.Application.Features.ArticleCategories.Commands.UpdateArticleCategory;
using QuanLyHoaLan.Application.Features.ArticleCategories.Queries.GetArticleCategories;
using QuanLyHoaLan.Application.Features.ArticleCategories.Queries.GetArticleCategoryById;
using QuanLyHoaLan.Application.Features.ArticleCategories.Queries.GetArticleCategoryTree;
using QuanLyHoaLan.Application.Features.Articles.Queries.GetArticles;
using QuanLyHoaLan.Application.Features.Articles.Commands.DeleteArticle;
using QuanLyHoaLan.API.Models;
using QuanLyHoaLan.Domain.Enums;

namespace QuanLyHoaLan.API.Controllers;

[ApiController]
[Route("api/application-categories")]
[Authorize(Roles = "Admin")]
public class ApplicationCategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ApplicationCategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetCategories([FromQuery] GetArticleCategoriesQuery query)
    {
        query.UseType(ArticleCategoryType.APPLICATION);
        return Ok(await _mediator.Send(query));
    }

    [HttpGet("tree")]
    [AllowAnonymous]
    public async Task<IActionResult> GetTree()
    {
        return Ok(await _mediator.Send(new GetArticleCategoryTreeQuery(ArticleCategoryType.APPLICATION)));
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id)
    {
        return Ok(await _mediator.Send(
            new GetArticleCategoryByIdQuery(id, ArticleCategoryType.APPLICATION)));
    }

    [HttpGet("articles")]
    [AllowAnonymous]
    public async Task<IActionResult> GetArticles([FromQuery] GetArticlesQuery query)
    {
        query.UseCategoryType(ArticleCategoryType.APPLICATION);
        return Ok(await _mediator.Send(query));
    }

    [HttpGet("{id:guid}/articles")]
    [AllowAnonymous]
    public async Task<IActionResult> GetArticlesByCategory(Guid id, [FromQuery] GetArticlesQuery query)
    {
        query.UseCategoryType(ArticleCategoryType.APPLICATION);
        query.ArticleCategoryId = id;
        return Ok(await _mediator.Send(query));
    }

    [HttpPost("articles")]
    public async Task<IActionResult> CreateArticle([FromBody] SectionArticleRequest request)
    {
        var command = request.ToCreateCommand(ArticleCategoryType.APPLICATION);
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(ArticlesController.GetArticleById), "Articles", new { id }, id);
    }

    [HttpPut("articles/{articleId:guid}")]
    public async Task<IActionResult> UpdateArticle(
        Guid articleId,
        [FromBody] SectionArticleRequest request)
    {
        var command = request.ToUpdateCommand(articleId, ArticleCategoryType.APPLICATION);
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("articles/{articleId:guid}")]
    public async Task<IActionResult> DeleteArticle(Guid articleId)
    {
        await _mediator.Send(new DeleteArticleCommand(articleId, ArticleCategoryType.APPLICATION));
        return NoContent();
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateArticleCategoryCommand command)
    {
        command.Type = ArticleCategoryType.APPLICATION;
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateArticleCategoryCommand command)
    {
        if (command.Id != Guid.Empty && command.Id != id)
        {
            return BadRequest("ID trên URL và body không khớp.");
        }

        command.Id = id;
        command.Type = ArticleCategoryType.APPLICATION;
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteArticleCategoryCommand(id, ArticleCategoryType.APPLICATION));
        return NoContent();
    }
}

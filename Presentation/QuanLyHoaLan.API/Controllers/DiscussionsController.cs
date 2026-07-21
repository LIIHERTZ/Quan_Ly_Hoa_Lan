using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyHoaLan.Application.DTOs.Discussion;
using QuanLyHoaLan.Application.Features.Discussions.Commands.CreateDiscussionComment;
using QuanLyHoaLan.Application.Features.Discussions.Commands.CreateDiscussionPost;
using QuanLyHoaLan.Application.Features.Discussions.Commands.DeleteDiscussionPost;
using QuanLyHoaLan.Application.Features.Discussions.Queries.GetDiscussionPostById;
using QuanLyHoaLan.Application.Features.Discussions.Queries.GetDiscussionPosts;
using QuanLyHoaLan.Application.Common.Models;

namespace QuanLyHoaLan.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DiscussionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public DiscussionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedList<DiscussionPostDto>>> GetPosts([FromQuery] GetDiscussionPostsQuery query)
    {
        return await _mediator.Send(query);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DiscussionPostDto>> GetPostById(Guid id)
    {
        return await _mediator.Send(new GetDiscussionPostByIdQuery { Id = id });
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Guid>> CreatePost(CreateDiscussionPostCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetPostById), new { id = id }, id);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeletePost(Guid id)
    {
        await _mediator.Send(new DeleteDiscussionPostCommand(id));
        return NoContent();
    }

    public record CreateCommentRequest(string Content);

    [HttpPost("{id}/comments")]
    [Authorize]
    public async Task<ActionResult<Guid>> CreateComment(Guid id, [FromBody] CreateCommentRequest request)
    {
        var command = new CreateDiscussionCommentCommand
        {
            PostId = id,
            Content = request.Content
        };
        var commentId = await _mediator.Send(command);
        return Ok(commentId);
    }
}

using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using QuanLyHoaLan.Application.Features.Orchids.Commands.CreateOrchid;
using QuanLyHoaLan.Application.Features.Orchids.Commands.UpdateOrchid;
using QuanLyHoaLan.Application.Features.Orchids.Commands.DeleteOrchid;
using QuanLyHoaLan.Application.Features.Orchids.Queries.GetOrchids;
using QuanLyHoaLan.Application.Features.Orchids.Queries.GetOrchidById;

namespace QuanLyHoaLan.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class OrchidsController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrchidsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetOrchids([FromQuery] GetOrchidsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrchidById(Guid id)
    {
        var result = await _mediator.Send(new GetOrchidByIdQuery(id));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrchid([FromBody] CreateOrchidCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetOrchidById), new { id = result }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOrchid(Guid id, [FromBody] UpdateOrchidCommand command)
    {
        if (command.Id != Guid.Empty && command.Id != id)
        {
            return BadRequest("ID mismatch");
        }
        command.Id = id;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrchid(Guid id)
    {
        var result = await _mediator.Send(new DeleteOrchidCommand(id));
        return Ok(result);
    }
}

using System;
using MediatR;

namespace QuanLyHoaLan.Application.Features.Orchids.Commands.DeleteOrchid;

public class DeleteOrchidCommand : IRequest<bool>
{
    public Guid Id { get; set; }

    public DeleteOrchidCommand(Guid id)
    {
        Id = id;
    }
}

using System;
using MediatR;
using QuanLyHoaLan.Application.DTOs.Orchid;

namespace QuanLyHoaLan.Application.Features.Orchids.Queries.GetOrchidById;

public class GetOrchidByIdQuery : IRequest<OrchidDto>
{
    public Guid Id { get; set; }

    public GetOrchidByIdQuery(Guid id)
    {
        Id = id;
    }
}

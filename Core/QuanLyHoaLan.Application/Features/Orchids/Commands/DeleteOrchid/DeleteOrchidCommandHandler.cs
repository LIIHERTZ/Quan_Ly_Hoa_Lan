using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using QuanLyHoaLan.Domain.Entities;
using QuanLyHoaLan.Domain.Interfaces.Repositories;

namespace QuanLyHoaLan.Application.Features.Orchids.Commands.DeleteOrchid;

public class DeleteOrchidCommandHandler : IRequestHandler<DeleteOrchidCommand, bool>
{
    private readonly IBaseRepository<Orchid> _orchidRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteOrchidCommandHandler(IBaseRepository<Orchid> orchidRepository, IUnitOfWork unitOfWork)
    {
        _orchidRepository = orchidRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteOrchidCommand command, CancellationToken cancellationToken)
    {
        var orchid = await _orchidRepository.FindByIdAsync(command.Id);
        if (orchid == null)
        {
            throw new Exception($"Không tìm thấy Orchid với Id {command.Id}.");
        }

        var result = await _orchidRepository.DeleteAsync(orchid, cancellationToken);
        
        if (!result)
        {
            throw new Exception($"Xóa Orchid {command.Id} thất bại.");
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}

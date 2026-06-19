using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using QuanLyHoaLan.Domain.Entities;
using QuanLyHoaLan.Domain.Interfaces.Repositories;

namespace QuanLyHoaLan.Application.Features.Categories.Commands.DeleteCategory;

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, bool>
{
    private readonly IBaseRepository<Category> _categoryRepository;
    private readonly IBaseRepository<Orchid> _orchidRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCategoryCommandHandler(IBaseRepository<Category> categoryRepository, IBaseRepository<Orchid> orchidRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _orchidRepository = orchidRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteCategoryCommand command, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.FindByIdAsync(command.Id);
        if (category == null)
        {
            throw new Exception($"Không tìm thấy Category với Id {command.Id}.");
        }

        Expression<Func<Category, bool>>[] subCatFilters = new Expression<Func<Category, bool>>[] { x => x.ParentId == command.Id };
        bool hasSubCategories = await _categoryRepository.AnyAsync(subCatFilters);
        if (hasSubCategories)
        {
            throw new Exception("Không thể xóa danh mục này vì vẫn còn danh mục con bên trong.");
        }

        Expression<Func<Orchid, bool>>[] orchidFilters = new Expression<Func<Orchid, bool>>[] { x => x.Categories.Any(c => c.Id == command.Id) };
        bool hasOrchids = await _orchidRepository.AnyAsync(orchidFilters);
        if (hasOrchids)
        {
            throw new Exception("Không thể xóa danh mục này vì vẫn còn loài Lan đang thuộc danh mục.");
        }

        var result = await _categoryRepository.DeleteAsync(category, cancellationToken);
        
        if (!result)
        {
            throw new Exception($"Xóa Category {command.Id} thất bại.");
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}

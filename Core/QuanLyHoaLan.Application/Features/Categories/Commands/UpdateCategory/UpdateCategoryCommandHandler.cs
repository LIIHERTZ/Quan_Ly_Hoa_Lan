using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using QuanLyHoaLan.Domain.Entities;
using QuanLyHoaLan.Domain.Interfaces.Repositories;

namespace QuanLyHoaLan.Application.Features.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, bool>
{
    private readonly IBaseRepository<Category> _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCategoryCommandHandler(IBaseRepository<Category> categoryRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateCategoryCommand command, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.FindByIdAsync(command.Id);
        if (category == null)
        {
            throw new Exception($"Không tìm thấy Category với Id {command.Id}.");
        }

        if (category.Slug != command.Slug)
        {
            Expression<Func<Category, bool>>[] filters = new Expression<Func<Category, bool>>[] { x => x.Slug == command.Slug };
            bool slugExists = await _categoryRepository.AnyAsync(filters);
            if (slugExists)
            {
                throw new Exception("Slug đã tồn tại.");
            }
        }

        if (command.ParentId == command.Id)
        {
            throw new Exception("Danh mục cha không hợp lệ (không thể tự làm cha của chính mình).");
        }

        category.Name = command.Name;
        category.Description = command.Description;
        category.Slug = command.Slug;
        category.ParentId = command.ParentId;

        await _categoryRepository.UpdateAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}

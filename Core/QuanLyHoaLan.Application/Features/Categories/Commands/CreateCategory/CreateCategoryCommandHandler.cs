using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using QuanLyHoaLan.Domain.Entities;
using QuanLyHoaLan.Domain.Interfaces.Repositories;

namespace QuanLyHoaLan.Application.Features.Categories.Commands.CreateCategory;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Guid>
{
    private readonly IBaseRepository<Category> _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCategoryCommandHandler(IBaseRepository<Category> categoryRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateCategoryCommand command, CancellationToken cancellationToken)
    {
        Expression<Func<Category, bool>>[] filters = new Expression<Func<Category, bool>>[] { x => x.Slug == command.Slug };
        bool slugExists = await _categoryRepository.AnyAsync(filters);
        if (slugExists)
        {
            throw new Exception("Slug đã tồn tại.");
        }

        if (command.ParentId.HasValue && !await _categoryRepository.ExistsAsync(command.ParentId.Value))
        {
            throw new InvalidOperationException("Danh mục cha không tồn tại hoặc đã bị xóa.");
        }

        var category = new Category
        {
            Name = command.Name,
            Description = command.Description,
            Slug = command.Slug,
            ParentId = command.ParentId
        };

        await _categoryRepository.InsertAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return category.Id;
    }
}

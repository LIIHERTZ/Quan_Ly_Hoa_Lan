using System.Linq.Expressions;
using QuanLyHoaLan.Domain.Common;

namespace QuanLyHoaLan.Domain.Interfaces.Repositories;

public interface IBaseRepository<TEntity> where TEntity : BaseEntity
{
    Task<List<TEntity>> FindAsync(
        Expression<Func<TEntity, bool>>[]? filters = null,
        string? orderBy = null,
        int skip = 0,
        int limit = 0,
        Expression<Func<TEntity, object>>[]? includes = null);

    Task<FindResult<TEntity>> FindResultAsync(
        Expression<Func<TEntity, bool>>[]? filters = null,
        string? orderBy = null,
        int skip = 0,
        int limit = 0,
        Expression<Func<TEntity, object>>[]? includes = null);

    Task<FindResult<TResult>> FindProjectedResultAsync<TResult>(
        Expression<Func<TEntity, bool>>[]? filters,
        string? orderBy,
        int skip,
        int limit,
        Expression<Func<TEntity, TResult>> selector);

    Task<int> CountAsync(Expression<Func<TEntity, bool>>[]? filters = null);

    Task<bool> AnyAsync(Expression<Func<TEntity, bool>>[]? filters = null, bool includeDeleted = false);

    Task<bool> ExistsAsync(Guid id);

    Task<bool> ExistsAsync(IEnumerable<Guid> ids);

    Task<TEntity?> FindByIdAsync(Guid id, params Expression<Func<TEntity, object>>[] includes);

    Task<TEntity?> FindOneAsync(
        Expression<Func<TEntity, bool>>[]? filters = null,
        string? orderBy = null,
        Expression<Func<TEntity, object>>[]? includes = null);

    Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task InsertRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    Task<bool> HardDeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task HardDeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    IQueryable<TEntity> Query(bool includeDeleted = false);

    IQueryable<TEntity> QueryNoTracking(bool includeDeleted = false);
}

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using QuanLyHoaLan.Domain.Common;
using QuanLyHoaLan.Domain.Interfaces.Repositories;

namespace QuanLyHoaLan.Infrastructure.Persistence.Repositories;

public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseEntity
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    public BaseRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public async Task<List<TEntity>> FindAsync(
        Expression<Func<TEntity, bool>>[]? filters = null,
        string? orderBy = null,
        int skip = 0,
        int limit = 0,
        Expression<Func<TEntity, object>>[]? includes = null)
    {
        var query = BuildQuery(filters, orderBy, skip, limit, includes);
        return await query.ToListAsync();
    }

    public async Task<FindResult<TEntity>> FindResultAsync(
        Expression<Func<TEntity, bool>>[]? filters = null,
        string? orderBy = null,
        int skip = 0,
        int limit = 0,
        Expression<Func<TEntity, object>>[]? includes = null)
    {
        var query = BuildQuery(filters, orderBy, skip, limit, includes);
        var count = await CountAsync(filters);

        return new FindResult<TEntity>
        {
            TotalCount = count,
            Items = await query.ToListAsync()
        };
    }

    public async Task<int> CountAsync(Expression<Func<TEntity, bool>>[]? filters = null)
    {
        var query = Query();
        if (filters != null)
        {
            foreach (var filter in filters)
            {
                query = query.Where(filter);
            }
        }
        return await query.CountAsync();
    }

    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>>[]? filters = null, bool includeDeleted = false)
    {
        var query = Query(includeDeleted);
        if (filters != null)
        {
            foreach (var filter in filters)
            {
                query = query.Where(filter);
            }
        }
        return await query.AnyAsync();
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _dbSet.AnyAsync(e => e.Id == id && !e.IsDeleted);
    }

    public async Task<bool> ExistsAsync(IEnumerable<Guid> ids)
    {
        var idList = ids.ToList();
        var count = await _dbSet.CountAsync(e => idList.Contains(e.Id) && !e.IsDeleted);
        return count == idList.Count;
    }

    public async Task<TEntity?> FindByIdAsync(Guid id, params Expression<Func<TEntity, object>>[] includes)
    {
        var query = Query();
        foreach (var include in includes)
        {
            query = query.Include(include);
        }
        return await query.FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<TEntity?> FindOneAsync(
        Expression<Func<TEntity, bool>>[]? filters = null,
        string? orderBy = null,
        Expression<Func<TEntity, object>>[]? includes = null)
    {
        var query = BuildQuery(filters, orderBy, 0, 1, includes);
        return await query.FirstOrDefaultAsync();
    }

    public async Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        entity.CreatedAt = DateTime.UtcNow;
        await _dbSet.AddAsync(entity, cancellationToken);
        return entity;
    }

    public async Task InsertRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        var list = entities.ToList();
        foreach (var entity in list)
        {
            entity.CreatedAt = DateTime.UtcNow;
        }
        await _dbSet.AddRangeAsync(list, cancellationToken);
    }

    public Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _dbSet.Update(entity);
        return Task.FromResult(entity);
    }

    public Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        var list = entities.ToList();
        foreach (var entity in list)
        {
            entity.UpdatedAt = DateTime.UtcNow;
        }
        _dbSet.UpdateRange(list);
        return Task.CompletedTask;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await FindByIdAsync(id);
        if (entity == null) return false;
        
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        _dbSet.Update(entity);
        return true;
    }

    public Task<bool> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        _dbSet.Update(entity);
        return Task.FromResult(true);
    }

    public Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        var list = entities.ToList();
        foreach (var entity in list)
        {
            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
        }
        _dbSet.UpdateRange(list);
        return Task.CompletedTask;
    }

    public async Task<bool> HardDeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbSet.FindAsync(new object[] { id }, cancellationToken);
        if (entity == null) return false;
        
        _dbSet.Remove(entity);
        return true;
    }

    public Task HardDeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        _dbSet.RemoveRange(entities);
        return Task.CompletedTask;
    }

    public IQueryable<TEntity> Query(bool includeDeleted = false)
    {
        var query = _dbSet.AsQueryable();
        if (!includeDeleted)
        {
            query = query.Where(e => !e.IsDeleted);
        }
        return query;
    }

    public IQueryable<TEntity> QueryNoTracking(bool includeDeleted = false)
    {
        return Query(includeDeleted).AsNoTracking();
    }

    private IQueryable<TEntity> BuildQuery(
        Expression<Func<TEntity, bool>>[]? filters,
        string? orderBy,
        int skip,
        int limit,
        Expression<Func<TEntity, object>>[]? includes)
    {
        var query = Query();

        if (includes != null)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }

        if (filters != null)
        {
            foreach (var filter in filters)
            {
                query = query.Where(filter);
            }
        }

        query = ApplyOrdering(query, orderBy);

        if (skip > 0)
        {
            query = query.Skip(skip);
        }

        if (limit > 0)
        {
            query = query.Take(limit);
        }

        return query;
    }

    private static IQueryable<TEntity> ApplyOrdering(IQueryable<TEntity> query, string? orderBy)
    {
        if (string.IsNullOrWhiteSpace(orderBy))
        {
            return query;
        }

        var parts = orderBy.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var property = typeof(TEntity).GetProperties()
            .FirstOrDefault(candidate => string.Equals(candidate.Name, parts[0], StringComparison.OrdinalIgnoreCase));

        if (property == null)
        {
            return query;
        }

        var parameter = Expression.Parameter(typeof(TEntity), "entity");
        var propertyAccess = Expression.Property(parameter, property);
        var keySelector = Expression.Lambda(propertyAccess, parameter);
        var methodName = parts.Length > 1 && parts[1].Equals("desc", StringComparison.OrdinalIgnoreCase)
            ? nameof(Queryable.OrderByDescending)
            : nameof(Queryable.OrderBy);

        var orderedExpression = Expression.Call(
            typeof(Queryable),
            methodName,
            [typeof(TEntity), property.PropertyType],
            query.Expression,
            Expression.Quote(keySelector));

        return query.Provider.CreateQuery<TEntity>(orderedExpression);
    }
}

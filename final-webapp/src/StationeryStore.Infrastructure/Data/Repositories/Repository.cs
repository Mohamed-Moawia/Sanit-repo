using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using StationeryStore.Application.Interfaces;
using StationeryStore.Domain.Common;

namespace StationeryStore.Infrastructure.Data.Repositories;

/// <summary>
/// Generic repository implementation with specification support
/// </summary>
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly StoreDbContext _context;
    protected readonly DbSet<T> _dbSet;
    
    public Repository(StoreDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }
    
    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _dbSet.FindAsync(new object[] { id }, cancellationToken);
    
    public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _dbSet.ToListAsync(cancellationToken);
    
    public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        return entity;
    }
    
    public virtual Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }
    
    public virtual Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Remove(entity);
        return Task.CompletedTask;
    }
    
    public virtual async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        => await _dbSet.FindAsync(new object[] { id }, cancellationToken) != null;
    
    public virtual async Task<int> CountAsync(CancellationToken cancellationToken = default)
        => await _dbSet.CountAsync(cancellationToken);
}

/// <summary>
/// Specification repository with advanced querying capabilities
/// </summary>
public class SpecificationRepository<T> : Repository<T>, ISpecificationRepository<T> where T : class
{
    public SpecificationRepository(StoreDbContext context) : base(context)
    {
    }
    
    public virtual async Task<IEnumerable<T>> FindAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        var query = ApplySpecification(specification);
        return await query.ToListAsync(cancellationToken);
    }
    
    public virtual async Task<T?> FirstOrDefaultAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        var query = ApplySpecification(specification);
        return await query.FirstOrDefaultAsync(cancellationToken);
    }
    
    public virtual async Task<int> CountAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        var query = ApplySpecification(specification);
        return await query.CountAsync(cancellationToken);
    }
    
    protected virtual IQueryable<T> ApplySpecification(ISpecification<T> specification)
    {
        var query = _dbSet.AsQueryable();

        // Apply criteria
        if (specification.Criteria != null)
        {
            query = query.Where(specification.Criteria);
        }

        // Apply includes
        query = specification.Includes.Aggregate(query, (current, include) => current.Include(include));

        // Apply ordering
        if (specification.OrderBy.Count > 0)
        {
            query = specification.OrderBy.Aggregate(query, (current, orderBy) => current.OrderBy(orderBy));
        }
        else if (specification.OrderByDescending.Count > 0)
        {
            query = specification.OrderByDescending.Aggregate(query, (current, orderBy) => current.OrderByDescending(orderBy));
        }

        return query;
    }
}

/// <summary>
/// Base specification implementation
/// </summary>
public class Specification<T> : ISpecification<T>
{
    public Expression<Func<T, bool>> Criteria { get; private set; } = null!;
    public List<Expression<Func<T, object>>> OrderBy { get; } = new();
    public List<Expression<Func<T, object>>> OrderByDescending { get; } = new();
    public List<Expression<Func<T, object>>> Includes { get; } = new();

    public Specification() { }

    public Specification(Expression<Func<T, bool>> criteria)
    {
        Criteria = criteria;
    }

    public void AddInclude(Expression<Func<T, object>> includeExpression)
        => Includes.Add(includeExpression);

    public void AddOrderBy(Expression<Func<T, object>> orderBy)
        => OrderBy.Add(orderBy);

    public void AddOrderByDescending(Expression<Func<T, object>> orderBy)
        => OrderByDescending.Add(orderBy);
}

using System.Linq.Expressions;
using StationeryStore.Domain.Entities;

namespace StationeryStore.Application.Interfaces;

/// <summary>
/// Generic repository interface for CRUD operations
/// </summary>
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository with specification support
/// </summary>
public interface ISpecificationRepository<T> : IRepository<T> where T : class
{
    Task<IEnumerable<T>> FindAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);
    Task<T?> FirstOrDefaultAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);
    Task<int> CountAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);
}

/// <summary>
/// Specification pattern interface
/// </summary>
public interface ISpecification<T>
{
    Expression<Func<T, bool>> Criteria { get; }
    List<Expression<Func<T, object>>> OrderBy { get; }
    List<Expression<Func<T, object>>> OrderByDescending { get; }
    List<Expression<Func<T, object>>> Includes { get; }

    void AddInclude(Expression<Func<T, object>> includeExpression);
    void AddOrderBy(Expression<Func<T, object>> orderBy);
    void AddOrderByDescending(Expression<Func<T, object>> orderBy);
}

/// <summary>
/// Unit of Work pattern for transaction management
/// </summary>
public interface IUnitOfWork : IDisposable
{
    IRepository<T> Repository<T>() where T : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}

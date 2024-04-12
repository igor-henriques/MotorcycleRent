namespace MotorcycleRent.Domain.Interfaces;

public interface IBaseRepository<TEntity> where TEntity : BaseEntity
{
    Task<TEntity?> CreateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<TEntity?> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TEntity?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<long> CountDocumentsAsync(EstimatedDocumentCountOptions estimatedDocumentCountOptions, CancellationToken cancellationToken = default);
    Task<TEntity?> GetByAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    Task CreateIndexAsync(CreateIndexModel<TEntity> indexModel, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> GetAllAsync(FilterDefinition<TEntity> filterDefinition, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> GetAllByAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    Task<TEntity?> GetAsync(FilterDefinition<TEntity> filterDefinition, CancellationToken cancellationToken = default);
}

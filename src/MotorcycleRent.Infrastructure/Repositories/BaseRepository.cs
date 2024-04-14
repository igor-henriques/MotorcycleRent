using MongoDB.Bson.Serialization;

namespace MotorcycleRent.Infrastructure.Repositories;

public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseEntity
{
    private readonly IMongoCollection<TEntity> _collection;

    public BaseRepository(
        MongoClient client,
        IOptions<DatabaseOptions> options)
    {
        if (!BsonClassMap.IsClassMapRegistered(typeof(Administrator)))
        {
            BsonClassMap.RegisterClassMap<Administrator>();
        }        

        _collection = client.GetDatabase(options.Value.DatabaseName)
            .GetCollection<TEntity>(options.Value.GetCollectionName<TEntity>());        
    }

    public async Task CreateIndexAsync(CreateIndexModel<TEntity> indexModel, CancellationToken cancellationToken = default)
    {
        List<string?> indexes = (await (await _collection.Indexes.ListAsync(cancellationToken)).ToListAsync(cancellationToken: cancellationToken))
            .Select(indexDocument => indexDocument.GetElement("name").Value.ToString())
            .ToList();

        if (!indexes.Where(index => index!.Equals(indexModel.Options.Name, StringComparison.InvariantCultureIgnoreCase)).Any())
        {
            await _collection.Indexes.CreateOneAsync(indexModel, cancellationToken: cancellationToken);
        }
    }

    public async Task<TEntity?> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _collection.InsertOneAsync(entity, cancellationToken: cancellationToken);
        return entity;
    }
    public async Task<TEntity?> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _collection.ReplaceOneAsync(x => x.Id == entity.Id, entity, cancellationToken: cancellationToken);
        return entity;
    }
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _collection.DeleteOneAsync(x => x.Id == id, cancellationToken: cancellationToken);
    }
    public async Task<TEntity?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _collection.Find(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);
    }
    public async Task<TEntity?> GetAsync(FilterDefinition<TEntity> filterDefinition, CancellationToken cancellationToken = default)
    {
        return await _collection.Find(filterDefinition).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<TEntity?> GetByAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _collection.Find(predicate).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<long> CountDocumentsAsync(EstimatedDocumentCountOptions estimatedDocumentCountOptions, CancellationToken cancellationToken = default)
    {
        return await _collection.EstimatedDocumentCountAsync(estimatedDocumentCountOptions, cancellationToken);
    }

    public async Task<long> CountDocumentsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _collection.CountDocumentsAsync(predicate, cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _collection.Find(x => true).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> GetAllByAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _collection.Find(predicate).ToListAsync(cancellationToken);
    }
    public async Task<IEnumerable<TEntity>> GetAllAsync(FilterDefinition<TEntity> filterDefinition, CancellationToken cancellationToken = default)
    {
        return await _collection.Find(filterDefinition).ToListAsync(cancellationToken);
    }

    public async Task CreateManyAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await _collection.InsertManyAsync(entities, cancellationToken: cancellationToken);
    }
}

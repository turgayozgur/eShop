# Repository Pattern Best Practices

## Core Principles

1. **Interface-based Design**: Always define and implement a repository interface (e.g., `IBasketRepository`).
2. **Dependency Injection**: Repositories should receive dependencies through constructor injection.
3. **Asynchronous Operations**: Use async/await pattern for all I/O operations to ensure scalability.
4. **Encapsulated Data Access**: Hide the details of data storage and retrieval from the rest of the application.
5. **Strongly-typed Results**: Return domain entities or models, not raw data structures.

## Best Practices

### Structure and Organization

```csharp
public class EntityRepository(IDependency dependency) : IEntityRepository
{
    private readonly IStorageClient _storageClient = dependency.GetClient();

    // Implementation methods go here
}
```

### Dependency Management

- Use constructor injection with primary constructor syntax when possible
- Keep infrastructure dependencies (like database connections) encapsulated
- Consider using interfaces for storage clients to facilitate testing

### Key Management

- Use consistent key generation patterns for data storage
- Encapsulate key generation logic within the repository
- Consider using static helper methods for key formatting:

```csharp
private static RedisKey GetEntityKey(string id) => KeyPrefix.Append(id);
```

### Data Serialization/Deserialization

- Use high-performance serialization appropriate for your storage backend
- Consider source generation for improved performance with JSON:

```csharp
[JsonSerializable(typeof(Entity))]
[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
public partial class EntitySerializationContext : JsonSerializerContext
{
}
```

### Error Handling

- Log data access errors with appropriate context
- Return meaningful results on failure (null, empty collection, etc.)
- Include relevant telemetry for debugging:

```csharp
if (!success)
{
    _logger.LogInformation("Problem occurred persisting the item.");
    return null;
}
```

### CRUD Operations

#### Create/Update
- Validate entities before storage
- Return the persisted entity to confirm success
- Consider atomicity requirements

#### Read
- Handle non-existent entities gracefully
- Consider caching strategies for frequently accessed data
- Use projection when only a subset of data is needed

#### Delete
- Return a boolean to indicate success/failure
- Consider soft-delete patterns for auditable data

## Implementation Examples

### Retrieving an Entity

```csharp
public async Task<TEntity> GetEntityAsync(string id)
{
    var data = await _database.GetAsync(GetEntityKey(id));
    
    if (data is null || data.Length == 0)
    {
        return null;
    }
    
    return Deserialize(data);
}
```

### Updating an Entity

```csharp
public async Task<TEntity> UpdateEntityAsync(TEntity entity)
{
    var serialized = Serialize(entity);
    var success = await _database.SetAsync(GetEntityKey(entity.Id), serialized);

    if (!success)
    {
        _logger.LogWarning("Failed to update entity with ID {EntityId}", entity.Id);
        return null;
    }

    _logger.LogInformation("Entity with ID {EntityId} successfully persisted", entity.Id);
    return await GetEntityAsync(entity.Id);
}
```

## Common Mistakes to Avoid

1. Exposing storage implementation details outside of the repository
2. Making synchronous calls to storage backends
3. Not handling serialization/deserialization exceptions
4. Missing proper logging for debugging
5. Inconsistent key management across different repository methods

## Performance Considerations

1. Use byte arrays instead of strings for keys when possible
2. Leverage source generation for serialization
3. Consider batching operations when dealing with multiple entities
4. Use the appropriate concurrent data structures
5. Utilize connection pooling for database connections 
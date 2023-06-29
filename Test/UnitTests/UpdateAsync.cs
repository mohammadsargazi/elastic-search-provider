namespace Test.UnitTests;


public class UpdateAsync : BaseElasticRepositoryTests<TestEntity, TestElasticRepository>
{
    [Fact]
    public async Task ValidEntity_ReturnsUpdateResult()
    {
        // Arrange
        var entity = GenerateSampleEntity(Guid.NewGuid());
        var cancellationToken = CancellationToken.None;

        // Act
        await _repository.InsertAsync(entity, cancellationToken);
        var updatedEntity = entity with { Title = "UpdatedValue" };
        var result = await _repository.UpdateAsync(updatedEntity, cancellationToken);


        // Assert
        Assert.True(result.updatedCount > 0);
    }

    [Fact]
    public async Task InValidEntity_ReturnsUpdateResultWithZeroCount()
    {
        // Arrange
        var entity = GenerateSampleEntity(Guid.NewGuid());
        var cancellationToken = CancellationToken.None;

        // Act
        await _repository.InsertAsync(entity, cancellationToken);
        var updatedEntity = entity with { Title = "UpdatedValue", Version = Guid.NewGuid() };


        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _repository.UpdateAsync(updatedEntity, cancellationToken));
    }
    [Fact]
    public async Task InValidEntities_ReturnsUpdateResultWithZeroCount()
    {
        // Arrange
        var entities = Enumerable.Range(0, 10)
        .Select(_ => GenerateSampleEntity(Guid.NewGuid()))
        .ToList();

        var entities2 = Enumerable.Range(0, 10)
       .Select(_ => GenerateSampleEntity(Guid.NewGuid()))
       .ToList();


        var cancellationToken = CancellationToken.None;

        // Act
        await _repository.InsertAllAsync(entities, cancellationToken);
        var updatedEntities = entities.Select(
            entity => entity with { Title = "UpdatedValue", Version = Guid.NewGuid() }).ToList();

        updatedEntities.AddRange(entities2);

        var result = await _repository.UpdateAllAsync(updatedEntities, cancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _repository.UpdateAllAsync(updatedEntities, cancellationToken));
    }
}
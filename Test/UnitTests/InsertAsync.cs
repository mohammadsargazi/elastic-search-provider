namespace Test.UnitTests;

public class InsertAsync : BaseElasticRepositoryTests<TestEntity, TestElasticRepository>
{
    [Fact]
    public async Task ValidEntity_ReturnsInsertResult()
    {
        // Arrange
        var entity = GenerateSampleEntity(Guid.NewGuid());
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _repository.InsertAsync(entity, cancellationToken);

        // Assert
        Assert.True(result.InsertedCount > 0);
    }

    [Fact]
    public async Task ValidEntities_AllInsertedSuccessfully()
    {
        // Arrange
        var entities = Enumerable.Range(0, 10)
          .Select(_ => GenerateSampleEntity(Guid.NewGuid()))
          .ToList();

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _repository.InsertAllAsync(entities, cancellationToken);

        // Assert
        Assert.True(!result.Any(x => x.InsertedCount == 0));
    }
}

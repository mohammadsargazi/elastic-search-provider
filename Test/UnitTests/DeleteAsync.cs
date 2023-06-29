namespace Test.UnitTests;

public class DeleteAsync : BaseElasticRepositoryTests<TestEntity, TestElasticRepository>
{

    [Fact]
    public async Task WhenEntityIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        TestEntity entity = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _repository.DeleteAsync(entity, CancellationToken.None));
    }

    [Fact]
    public async Task ExistingEntity_ReturnsDeleteResult()
    {
        // Arrange
        var entity = GenerateSampleEntity(Guid.NewGuid());


        // Act
        await _repository.InsertAsync(entity, CancellationToken.None);
        var result = await _repository.DeleteAsync(entity, CancellationToken.None);

        // Assert
        Assert.True(result.DeletedCount > 0);
    }

    [Fact]
    public async Task InValidEntity_ReturnInvalidOperationException()
    {
        // Arrange
        var entity = GenerateSampleEntity(Guid.NewGuid());


        // Act
        await _repository.InsertAsync(entity, CancellationToken.None);
        var inValidEntity = entity with { Version = Guid.NewGuid() };

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _repository.DeleteAsync(inValidEntity, CancellationToken.None));
    }
}


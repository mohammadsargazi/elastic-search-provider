namespace Test.UnitTests;

public class ListPagedAsync : BaseElasticRepositoryTests<TestEntity, TestElasticRepository>
{
    [Fact]
    public async Task ReturnsPagedData()
    {
        // Arrange
        var entities = Enumerable.Range(0, 10)
          .Select(_ => GenerateSampleEntity(Guid.NewGuid()))
          .ToList();
        var cancellationToken = CancellationToken.None;
        await _repository.InsertAllAsync(entities, cancellationToken);

        // Act
        var requestMock = new Mock<IPagedRequest>();
        requestMock.SetupGet(r => r.Page).Returns(1);
        requestMock.SetupGet(r => r.PageSize).Returns(10);
        var result = await _repository.ListPagedAsync(requestMock.Object, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(result.Result.Count, entities.Count);
    }
}
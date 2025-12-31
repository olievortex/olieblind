using Moq;
using olieblind.data;
using olieblind.data.Entities;
using olieblind.lib.ForecastModels;

namespace olieblind.test.ForecastModelsTests;

public class ModelForecastBusinessTests
{
    #region GetIndexPage tests

    [Test]
    public async Task GetIndexPageAsync_ReturnsValue_ValidParameters()
    {
        // Arrange
        var ct = CancellationToken.None;
        var effective = new DateTime(2021, 7, 18);
        var sourceUrl = Guid.NewGuid().ToString();
        var repo = new Mock<IMyRepository>();
        repo.Setup(s => s.ProductMapGetLatest(ct))
            .ReturnsAsync(
                new ProductMapEntity
                {
                    Id = 123,
                    Effective = effective,
                    ForecastHour = 6,
                    ProductId = 1,
                    SourceUrl = sourceUrl,
                    IsActive = true,
                    Timestamp = DateTime.UtcNow,
                }
            );
        repo.Setup(s => s.ProductMapItemList(123, ct))
            .ReturnsAsync(
                [
                    new ProductMapItemEntity
                    {
                        Id = 1,
                        ProductMapId = 123,
                        ParameterId = 11,
                        Title = "Test",
                        Url = "http://test.com",
                        IsActive = true,
                        Timestamp = DateTime.UtcNow,
                    }
                ]
            );
        var testable = new ModelForecastBusiness(repo.Object);

        // Act
        var result = await testable.GetIndexPageAsync(ct);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Header.Id, Is.EqualTo(123));
            Assert.That(result.Header.Effective, Is.EqualTo(effective));
            Assert.That(result.Header.ForecastHour, Is.EqualTo(6));
            Assert.That(result.Items[0].ParameterId, Is.EqualTo(11));
            Assert.That(result.Items[0].Title, Is.EqualTo("Test"));
            Assert.That(result.Items[0].Url, Is.EqualTo("http://test.com"));
        }
    }

    #endregion

    #region GetProduct tests

    [Test]
    public async Task GetProductAsync_ReturnsValue_ValidParameters()
    {
        // Arrange
        var ct = CancellationToken.None;
        var effective = new DateTime(2021, 7, 18);
        var sourceUrl = Guid.NewGuid().ToString();
        var repo = new Mock<IMyRepository>();
        repo.Setup(s => s.ProductMapGet(123, ct))
            .ReturnsAsync(
                new ProductMapEntity
                {
                    Id = 123,
                    Effective = effective,
                    ForecastHour = 6,
                    ProductId = 1,
                    SourceUrl = sourceUrl,
                    IsActive = true,
                    Timestamp = DateTime.UtcNow,
                }
            );
        repo.Setup(s => s.ProductMapItemList(123, ct))
            .ReturnsAsync(
                [
                    new ProductMapItemEntity
                    {
                        Id = 1,
                        ProductMapId = 123,
                        ParameterId = 11,
                        Title = "Test",
                        Url = "http://test.com",
                        IsActive = true,
                        Timestamp = DateTime.UtcNow,
                    }
                ]
            );
        var testable = new ModelForecastBusiness(repo.Object);

        // Act
        var result = (await testable.GetProductAsync(123, ct))!;

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Header.Id, Is.EqualTo(123));
            Assert.That(result.Header.Effective, Is.EqualTo(effective));
            Assert.That(result.Header.ForecastHour, Is.EqualTo(6));
            Assert.That(result.Items[0].ParameterId, Is.EqualTo(11));
            Assert.That(result.Items[0].Title, Is.EqualTo("Test"));
            Assert.That(result.Items[0].Url, Is.EqualTo("http://test.com"));
        }
    }

    [Test]
    public async Task GetProductAsync_ReturnsNull_NotFound()
    {
        // Arrange
        var ct = CancellationToken.None;
        var repo = new Mock<IMyRepository>();
        var testable = new ModelForecastBusiness(repo.Object);

        // Act
        var result = await testable.GetProductAsync(123, ct);

        // Assert
        Assert.That(result, Is.Null);
    }

    #endregion

    #region GetProductList tests

    [Test]
    public async Task GetProductListAsync_ReturnsList_ValidParameters()
    {
        // Arrange
        var ct = CancellationToken.None;
        var effective = new DateTime(2021, 7, 18);
        var sourceUrl = Guid.NewGuid().ToString();
        var repo = new Mock<IMyRepository>();
        repo.Setup(s => s.ProductMapList(ct))
            .ReturnsAsync(
                [
                    new ProductMapEntity
                    {
                        Id = 123,
                        Effective = effective,
                        ForecastHour = 6,
                        ProductId = 1,
                        SourceUrl = sourceUrl,
                        IsActive = true,
                        Timestamp = DateTime.UtcNow,
                    }
                ]
            );
        var testable = new ModelForecastBusiness(repo.Object);

        // Act
        var result = await testable.GetProductListAsync(ct);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Id, Is.EqualTo(123));
            Assert.That(result[0].Effective, Is.EqualTo(effective));
            Assert.That(result[0].ForecastHour, Is.EqualTo(6));
        }
    }

    #endregion
}

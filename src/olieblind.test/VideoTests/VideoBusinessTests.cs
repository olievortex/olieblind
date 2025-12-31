using Moq;
using olieblind.data;
using olieblind.data.Entities;
using olieblind.lib.Video;

namespace olieblind.test.VideoTests;

public class VideoBusinessTests
{
    [Test]
    public async Task GetVideoAsync_ReturnsNull_ValidParameters()
    {
        // Arrange
        var ct = CancellationToken.None;
        var id = 123;
        var repo = new Mock<IMyRepository>();
        var testable = new VideoBusiness(repo.Object);

        // Act
        var result = (await testable.GetVideoAsync(id, ct))!;

        // Test
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetVideoAsync_ReturnsVideo_ValidParameters()
    {
        // Arrange
        var ct = CancellationToken.None;
        var id = 123;
        var category = Guid.NewGuid().ToString();
        var posterUri = Guid.NewGuid().ToString();
        var title = Guid.NewGuid().ToString();
        var videoUri = Guid.NewGuid().ToString();
        var transcript = Guid.NewGuid().ToString();
        var repo = new Mock<IMyRepository>();
        repo.Setup(s => s.ProductVideoGet(id, ct))
            .ReturnsAsync(
                new ProductVideoEntity
                {
                    Id = id,
                    PosterUrl = posterUri,
                    Title = title,
                    VideoUrl = videoUri,
                    Category = category,
                    Transcript = transcript
                }
            );
        var testable = new VideoBusiness(repo.Object);

        // Act
        var result = (await testable.GetVideoAsync(id, ct))!;

        // Test
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(id));
            Assert.That(result.PosterUrl, Is.EqualTo(posterUri));
            Assert.That(result.Title, Is.EqualTo(title));
            Assert.That(result.VideoUrl, Is.EqualTo(videoUri));
            Assert.That(result.Transcript, Is.EqualTo(transcript));
            Assert.That(result.Category, Is.EqualTo(category));
        }
    }

    [Test]
    public async Task GetVideoListAsync_ReturnsVideo_ValidParameters()
    {
        // Arrange
        var ct = CancellationToken.None;
        var category = Guid.NewGuid().ToString();
        var repo = new Mock<IMyRepository>();
        repo.Setup(s => s.ProductVideoGetList(ct))
            .ReturnsAsync(
                [new ProductVideoEntity { Category = category }]
            );
        var testable = new VideoBusiness(repo.Object);

        // Act
        var result = await testable.GetVideoListAsync(category, ct);

        // Test
        Assert.That(result, Has.Count.EqualTo(1));
    }

    [Test]
    public async Task GetIndexPageAsync_ReturnsPage_ValidParameters()
    {
        // Arrange
        var ct = CancellationToken.None;
        var id2 = 123;
        var posterUri = Guid.NewGuid().ToString();
        var title = Guid.NewGuid().ToString();
        var videoUri = Guid.NewGuid().ToString();
        var transcript = Guid.NewGuid().ToString();
        var repo = new Mock<IMyRepository>();
        repo.Setup(s => s.ProductVideoGetListMostRecent(ct))
            .ReturnsAsync(
                [
                    new()
                    {
                        Id = id2,
                        Category = "Day 1 Convective Outlook",

                        Title = title,
                        VideoUrl = videoUri,
                        Transcript = transcript,
                        PosterUrl = posterUri
                    },
                    new()
                    {
                        Category = "Day 2 Convective Outlook"
                    },
                    new()
                    {
                        Category = "Day 3 Convective Outlook"
                    },
                    new()
                    {
                        Category = "Drought Monitor"
                    }
                ]
            );
        var testable = new VideoBusiness(repo.Object);

        // Act
        var result = await testable.GetIndexPageAsync(ct);
        var dayOne = result.SpcDayOne!;

        // Test
        using (Assert.EnterMultipleScope())
        {
            Assert.That(dayOne.Id, Is.EqualTo(id2));
            Assert.That(dayOne.PosterUrl, Is.EqualTo(posterUri));
            Assert.That(dayOne.Title, Is.EqualTo(title));
            Assert.That(dayOne.VideoUrl, Is.EqualTo(videoUri));
            Assert.That(dayOne.Transcript, Is.EqualTo(transcript));
            Assert.That(result.SpcDayTwo, Is.Not.Null);
            Assert.That(result.SpcDayThree, Is.Not.Null);
            Assert.That(result.DroughtMonitor, Is.Not.Null);
        }
    }

    [Test]
    public void Pivot_ReturnNull_CategoryNotFound()
    {
        // Arrange
        var items = new List<ProductVideoEntity>()
        {
            new()
            {
                Category = "Meow"
            }
        };

        // Act
        var result = VideoBusiness.Pivot(items, "bark");

        // Assert
        Assert.That(result, Is.Null);
    }
}
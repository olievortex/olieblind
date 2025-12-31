using olieblind.data.Entities;
using olieblind.lib.Models;

namespace olieblind.test.ModelTests;

public class ProductVideoModelTests
{
    [Test]
    public void From_VideoModel_ProductVideoEntity()
    {
        // Arrange
        var id = 123;
        var category = Guid.NewGuid().ToString();
        var transcript = Guid.NewGuid().ToString();
        var posterUri = Guid.NewGuid().ToString();
        var title = Guid.NewGuid().ToString();
        var videoUri = Guid.NewGuid().ToString();
        var entity = new ProductVideoEntity
        {
            Id = id,
            Category = category,
            Transcript = transcript,
            PosterUrl = posterUri,
            Title = title,
            VideoUrl = videoUri,
        };

        // Act
        var result = ProductVideoModel.Map(entity);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(id));
            Assert.That(result.Category, Is.EqualTo(category));
            Assert.That(result.Transcript, Is.EqualTo(transcript));
            Assert.That(result.PosterUrl, Is.EqualTo(posterUri));
            Assert.That(result.Title, Is.EqualTo(title));
            Assert.That(result.VideoUrl, Is.EqualTo(videoUri));
        }
    }
}

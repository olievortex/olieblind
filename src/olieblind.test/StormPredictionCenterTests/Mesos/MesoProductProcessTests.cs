using Moq;
using olieblind.data;
using olieblind.data.Entities;
using olieblind.lib.StormPredictionCenter.Interfaces;
using olieblind.lib.StormPredictionCenter.Mesos;

namespace olieblind.test.StormPredictionCenterTests.Mesos;

public class MesoProductProcessTests
{
    #region Download

    [Test]
    public async Task DownloadAsync_ShortCircuit_NoFile()
    {
        // Arrange
        const int year = 2023;
        const int index = 253;
        var ct = CancellationToken.None;
        var source = new Mock<IMesoProductSource>();
        var testable = new MesoProductProcess(source.Object, null!, null!);

        // Act
        var result = await testable.Download(year, index, null!, ct);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task DownloadAsync_CompletesSteps_NewFile()
    {
        // Arrange
        const int year = 2023;
        const int index = 253;
        var entity = new SpcMesoProductEntity();
        var effectiveTime = new DateTime(2023, 3, 15, 21, 35, 0);
        var areasAffected = Guid.NewGuid().ToString();
        var concerning = Guid.NewGuid().ToString();
        var narrative = Guid.NewGuid().ToString();
        var html = Guid.NewGuid().ToString();
        var ct = CancellationToken.None;
        var source = new Mock<IMesoProductSource>();
        source.Setup(s => s.DownloadHtml(year, index, ct)).ReturnsAsync(html);
        var repo = new Mock<IMyRepository>();
        repo.Setup(s => s.SpcMesoProductCreate(It.IsAny<SpcMesoProductEntity>(), ct))
            .Callback((SpcMesoProductEntity e, CancellationToken _) => entity = e);
        var parse = new Mock<IMesoProductParsing>();
        parse.Setup(s => s.GetEffectiveTime(It.IsAny<string>())).Returns(effectiveTime);
        parse.Setup(s => s.GetAreasAffected(It.IsAny<string>())).Returns(areasAffected);
        parse.Setup(s => s.GetConcerning(It.IsAny<string>())).Returns(concerning);
        parse.Setup(s => s.GetNarrative(It.IsAny<string>())).Returns(narrative);
        var testable = new MesoProductProcess(source.Object, parse.Object, repo.Object);

        // Act
        var result = await testable.Download(year, index, null!, ct);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.True);
            Assert.That(entity.AreasAffected, Is.EqualTo(areasAffected));
            Assert.That(entity.EffectiveDate, Is.EqualTo("2023-03-15"));
            Assert.That(entity.Concerning, Is.EqualTo(concerning));
            Assert.That(entity.Narrative, Is.EqualTo(narrative));
            Assert.That(entity.Html, Is.EqualTo(html));
        }
    }

    #endregion
}
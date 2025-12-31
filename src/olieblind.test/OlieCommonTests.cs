using Moq;
using olieblind.lib;
using olieblind.lib.Services;

namespace olieblind.test;

public class OlieCommonTests
{
    #region CreateLocalTmpPath

    [Test]
    public void CreateLocalTmpPath_ReturnsPath_Always()
    {
        // Arrange
        const string extension = ".olie";

        // Act
        var result = OlieCommon.CreateLocalTmpPath(extension);

        // Assert
        Assert.That(result, Does.EndWith(".olie"));
    }

    #endregion

    #region DeleteTEmpFiles

    [Test]
    public void DeleteTempFiles_DeletesFiles_WithParams()
    {
        // Arrange
        var ows = new Mock<IOlieWebService>();
        var files = new List<string> { "a.tmp", "b.tmp" };
        const string file = "c.tmp";

        // Act
        OlieCommon.DeleteTempFiles(ows.Object, files, file);

        // Assert
        ows.Verify(v => v.FileDelete(It.IsAny<string>()), Times.Exactly(3));
    }

    #endregion

    #region IsDaylightSavingsTime

    [Test]
    public void IsDaylightSavingsTime_ReturnsTrue_Summer()
    {
        // Arrange
        var date = new DateTime(2023, 6, 12);

        // Act
        var result = OlieCommon.IsDaylightSavingsTime(date);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void IsDaylightSavingsTime_ReturnsFalse_Winter()
    {
        // Arrange
        var date = new DateTime(2023, 2, 12);

        // Act
        var result = OlieCommon.IsDaylightSavingsTime(date);

        // Assert
        Assert.That(result, Is.False);
    }

    #endregion

    #region GetCurrentTimeZoneOffset

    [Test]
    public void GetCurrentTimeZoneOffset_ReturnsTrue_Summer()
    {
        // Arrange
        var date = new DateTime(2023, 6, 12);

        // Act
        var result = OlieCommon.GetCurrentTimeZoneOffset(date);

        // Assert
        Assert.That(result, Is.EqualTo(-5));
    }

    [Test]
    public void GetCurrentTimeZoneOffset_ReturnsFalse_Winter()
    {
        // Arrange
        var date = new DateTime(2023, 2, 12);

        // Act
        var result = OlieCommon.GetCurrentTimeZoneOffset(date);

        // Assert
        Assert.That(result, Is.EqualTo(-6));
    }

    #endregion

    #region ParseSpcEffectiveDate

    [Test]
    public void ParseEffectiveDate_ThrowsException_EmptyText()
    {
        Assert.Throws<ArgumentNullException>(() => OlieCommon.ParseSpcEffectiveDate(string.Empty));
    }

    #endregion

    #region TimeZoneToOffset

    [Test]
    public void TimeZoneToOffset_CorrectOffset_BatchA()
    {
        // Act, Arrange, Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(OlieCommon.TimeZoneToOffset("utc"), Is.Zero);
            Assert.That(OlieCommon.TimeZoneToOffset("edt"), Is.EqualTo(-4));
            Assert.That(OlieCommon.TimeZoneToOffset("est"), Is.EqualTo(-5));
            Assert.That(OlieCommon.TimeZoneToOffset("cdt"), Is.EqualTo(-5));
            Assert.That(OlieCommon.TimeZoneToOffset("cst"), Is.EqualTo(-6));
            Assert.That(OlieCommon.TimeZoneToOffset("mdt"), Is.EqualTo(-6));
            Assert.That(OlieCommon.TimeZoneToOffset("mst"), Is.EqualTo(-7));
            Assert.That(OlieCommon.TimeZoneToOffset("pdt"), Is.EqualTo(-7));
            Assert.That(OlieCommon.TimeZoneToOffset("pst"), Is.EqualTo(-8));
            Assert.Throws<ApplicationException>(() => OlieCommon.TimeZoneToOffset("dillon"));
        }
    }

    #endregion
}
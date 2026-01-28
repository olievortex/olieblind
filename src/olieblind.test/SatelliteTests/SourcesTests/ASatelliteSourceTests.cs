using olieblind.data.Enums;
using olieblind.lib.Satellite.Sources;

namespace olieblind.test.SatelliteTests.SourcesTests;

public class ASatelliteSourceTests
{
    #region GetEffectiveDate

    [Test]
    public void GetEffectiveDate_Date_ValidParameters()
    {
        // Arrange
        const string effectiveDate = "2010-07-21";

        // Act
        var result = ASatelliteSource.GetEffectiveDate(effectiveDate);

        // Assert
        Assert.That(result, Is.EqualTo(new DateTime(2010, 7, 21)));
    }

    #endregion

    #region GetEffectiveStart

    [Test]
    public void GetEffectiveStart_ReturnsDateTime_Owl()
    {
        // Arrange
        var effectiveDate = new DateTime(2021, 5, 18);
        var expected = new DateTime(2021, 5, 18, 6, 0, 0);

        // Act
        var result = ASatelliteSource.GetEffectiveStart(effectiveDate, DayPartsEnum.Owl);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void GetEffectiveStart_ReturnsDateTime_Morning()
    {
        // Arrange
        var effectiveDate = new DateTime(2021, 5, 18);
        var expected = new DateTime(2021, 5, 18, 12, 0, 0);

        // Act
        var result = ASatelliteSource.GetEffectiveStart(effectiveDate, DayPartsEnum.Morning);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void GetEffectiveStart_ReturnsDateTime_Afternoon()
    {
        // Arrange
        var effectiveDate = new DateTime(2021, 5, 18);
        var expected = new DateTime(2021, 5, 18, 18, 0, 0);

        // Act
        var result = ASatelliteSource.GetEffectiveStart(effectiveDate, DayPartsEnum.Afternoon);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void GetEffectiveStart_ReturnsDateTime_Night()
    {
        // Arrange
        var effectiveDate = new DateTime(2021, 5, 18);
        var expected = new DateTime(2021, 5, 19);

        // Act
        var result = ASatelliteSource.GetEffectiveStart(effectiveDate, DayPartsEnum.Night);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    #endregion

    #region GetEffectiveStop

    [Test]
    public void GetEffectiveStop_ReturnsDateTime_Owl()
    {
        // Arrange
        var effectiveDate = new DateTime(2021, 5, 18);
        var expected = new DateTime(2021, 5, 18, 11, 0, 0);

        // Act
        var result = ASatelliteSource.GetEffectiveStop(effectiveDate, DayPartsEnum.Owl);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void GetEffectiveStop_ReturnsDateTime_Morning()
    {
        // Arrange
        var effectiveDate = new DateTime(2021, 5, 18);
        var expected = new DateTime(2021, 5, 18, 17, 0, 0);

        // Act
        var result = ASatelliteSource.GetEffectiveStop(effectiveDate, DayPartsEnum.Morning);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void GetEffectiveStop_ReturnsDateTime_Afternoon()
    {
        // Arrange
        var effectiveDate = new DateTime(2021, 5, 18);
        var expected = new DateTime(2021, 5, 18, 23, 0, 0);

        // Act
        var result = ASatelliteSource.GetEffectiveStop(effectiveDate, DayPartsEnum.Afternoon);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void GetEffectiveStop_ReturnsDateTime_Night()
    {
        // Arrange
        var effectiveDate = new DateTime(2021, 5, 18);
        var expected = new DateTime(2021, 5, 19, 5, 0, 0);

        // Act
        var result = ASatelliteSource.GetEffectiveStop(effectiveDate, DayPartsEnum.Night);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    #endregion

    #region GetPath

    [Test]
    public void GetPrefix_Prefix_ValidParameters()
    {
        // Arrange
        const string metal = "gold";
        var effectiveDate = new DateTime(2021, 7, 21);

        // Act
        var result = ASatelliteSource.GetPath(effectiveDate, metal);

        // Assert
        Assert.That(result, Is.EqualTo("gold/aws/satellite/2021/07/21"));
    }

    #endregion
}

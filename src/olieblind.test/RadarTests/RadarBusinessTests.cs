using Moq;
using olieblind.data;
using olieblind.data.Entities;
using olieblind.lib.Radar;
using olieblind.lib.Radar.Interfaces;

namespace olieblind.test.RadarTests;

public class RadarBusinessTests
{
    #region DownloadInventoryForClosestRadar

    [Test]
    public async Task DownloadInventoryForClosestRadar_FirstRadar_HasData()
    {
        // Arrange
        var effectiveTime = new DateTime(2021, 7, 21, 18, 25, 0);
        var ct = CancellationToken.None;
        const double latitude = 45;
        const double longitude = -93;
        var expected = new List<RadarSiteEntity> {
            new() { Id = "a", Latitude = 44, Longitude = -94 },
            new() { Id = "b", Latitude = 50, Longitude = -100 }
         };
        var cache = new List<RadarInventoryEntity>();
        var radarSites = new List<RadarSiteEntity>();
        var source = new Mock<IRadarSource>();
        source.Setup(s => s.FindClosestRadars(radarSites, latitude, longitude))
            .Returns(expected);
        source.Setup(s => s.AddRadarInventory(cache, expected[0], effectiveTime, null!, ct))
            .ReturnsAsync(new RadarInventoryEntity() { FileList = ["a"] });
        var testable = new RadarBusiness(source.Object, null!);

        // Act
        var result =
            await testable.DownloadInventoryForClosestRadar(radarSites, cache, effectiveTime, latitude, longitude,
                null!, ct);

        // Assert
        Assert.That(result, Is.EqualTo(expected[0]));
    }

    [Test]
    public async Task DownloadInventoryForClosestRadar_SecondRadar_FirstNoData()
    {
        // Arrange
        var effectiveTime = new DateTime(2021, 7, 21, 18, 25, 0);
        var ct = CancellationToken.None;
        const double latitude = 45;
        const double longitude = -93;
        var expected = new List<RadarSiteEntity> {
            new() { Id = "a", Latitude = 44, Longitude = -94 },
            new() { Id = "b", Latitude = 50, Longitude = -100 }
         };
        var cache = new List<RadarInventoryEntity>();
        var radarSites = new List<RadarSiteEntity>();
        var source = new Mock<IRadarSource>();
        source.Setup(s => s.FindClosestRadars(radarSites, latitude, longitude))
            .Returns(expected);
        source.Setup(s => s.AddRadarInventory(cache, expected[0], effectiveTime, null!, ct))
            .ReturnsAsync(new RadarInventoryEntity() { FileList = [] });
        source.Setup(s => s.AddRadarInventory(cache, expected[1], effectiveTime, null!, ct))
            .ReturnsAsync(new RadarInventoryEntity() { FileList = ["a"] });
        var testable = new RadarBusiness(source.Object, null!);

        // Act
        var result =
            await testable.DownloadInventoryForClosestRadar(radarSites, cache, effectiveTime, latitude, longitude,
                null!, ct);

        // Assert
        Assert.That(result, Is.EqualTo(expected[1]));
    }

    [Test]
    public async Task DownloadInventoryForClosestRadar_Exception_NoData()
    {
        // Arrange
        var effectiveTime = new DateTime(2021, 7, 21, 18, 25, 0);
        var ct = CancellationToken.None;
        const double latitude = 45;
        const double longitude = -93;
        var expected = new List<RadarSiteEntity> {
            new() { Id = "a", Latitude = 44, Longitude = -94 },
            new() { Id = "b", Latitude = 50, Longitude = -100 }
         };
        var cache = new List<RadarInventoryEntity>();
        var radarSites = new List<RadarSiteEntity>();
        var source = new Mock<IRadarSource>();
        source.Setup(s => s.FindClosestRadars(radarSites, latitude, longitude))
            .Returns(expected);
        source.Setup(s => s.AddRadarInventory(cache, It.IsAny<RadarSiteEntity>(), effectiveTime, null!, ct))
            .ReturnsAsync(new RadarInventoryEntity() { FileList = [] });
        var testable = new RadarBusiness(source.Object, null!);

        // Act, Assert
        Assert.ThrowsAsync<InvalidOperationException>(() => testable.DownloadInventoryForClosestRadar(radarSites, cache, effectiveTime, latitude, longitude, null!, ct));
    }

    #endregion

    #region GetPrimaryRadarSites

    [Test]
    public async Task GetPrimaryRadarSites_AllSteps_ValidParameters()
    {
        // Arrange
        var ct = CancellationToken.None;
        var expected = new List<RadarSiteEntity> { new() { Id = "KILO" }, new() { Id = "PABC" } };
        var repo = new Mock<IMyRepository>();
        var testable = new RadarBusiness(null!, repo.Object);
        repo.Setup(s => s.RadarSiteGetAll(ct)).ReturnsAsync(expected);

        // Act
        var result = await testable.GetPrimaryRadarSites(ct);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result[0], Is.EqualTo(expected[0]));
            Assert.That(result, Has.Count.EqualTo(1));
        }
    }

    #endregion

    #region PopulateRadarSitesFromCsv

    [Test]
    public async Task PopulateCosmosFromCsv_AddsRecord_ValidParameters()
    {
        // Arrange
        var ct = CancellationToken.None;
        var entities = new List<RadarSiteEntity>();
        const string value =
            """
                NCDCID   ICAO WBAN  NAME                           COUNTRY              ST COUNTY                         LAT       LON        ELEV   UTC   STNTYPE                                            
                -------- ---- ----- ------------------------------ -------------------- -- ------------------------------ --------- ---------- ------ ----- -------------------------------------------------- 
                30001924 KTLH 93805 TALLAHASSEE                    UNITED STATES        FL LEON                           30.397583 -84.328944 177    -5    NEXRAD                                             
                30001938 LPLA 13201 LAJES AB                       AZORES                                                 38.73028  -27.32167  3334   -1    NEXRAD           
                30001795 KABX 03019 ALBUQUERQUE                    UNITED STATES        NM BERNALILLO                     35.149722 -106.82388 5951   -7    NEXRAD                                             
                30112656 TBOS       BOSTON                         UNITED STATES        MA NORFOLK                        42.158056 -70.933056 262    -5    TDWR                                               

                """;
        var source = new Mock<IRadarSource>();
        var repo = new Mock<IMyRepository>();
        repo.Setup(s => s.RadarSiteCreate(It.IsAny<List<RadarSiteEntity>>(), ct))
            .Callback((List<RadarSiteEntity> e, CancellationToken _) => entities = e);
        var testable = new RadarBusiness(source.Object, repo.Object);

        // Act
        await testable.PopulateRadarSitesFromCsv(value, ct);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(entities, Has.Count.EqualTo(2));
            Assert.That(entities[1].Id, Is.EqualTo("KABX"));
            Assert.That(entities[1].Name, Is.EqualTo("ALBUQUERQUE"));
            Assert.That(entities[1].State, Is.EqualTo("NM"));
            Assert.That(entities[1].Timestamp, Is.Not.EqualTo(DateTime.MinValue));
            Assert.That(entities[1].Latitude, Is.EqualTo(35.149722));
            Assert.That(entities[1].Longitude, Is.EqualTo(-106.82388));
        }
    }

    #endregion
}
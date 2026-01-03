using Microsoft.EntityFrameworkCore;
using olieblind.data.Entities;
using System.Diagnostics.CodeAnalysis;

namespace olieblind.data;

[ExcludeFromCodeCoverage]
public class MyContext(DbContextOptions<MyContext> options) : DbContext(options)
{
    public virtual DbSet<ProductMapEntity> ProductMaps { get; set; }
    public virtual DbSet<ProductMapItemEntity> ProductMapItems { get; set; }
    public virtual DbSet<ProductVideoEntity> ProductVideos { get; set; }
    public virtual DbSet<RadarInventoryEntity> RadarInventories { get; set; }
    public virtual DbSet<RadarSiteEntity> RadarSites { get; set; }
    public virtual DbSet<SpcMesoProductEntity> SpcMesoProducts { get; set; }
    public virtual DbSet<StormEventsDailyDetailEntity> StormEventsDailyDetails { get; set; }
    public virtual DbSet<StormEventsDailySummaryEntity> StormEventsDailySummaries { get; set; }
    public virtual DbSet<StormEventsDatabaseEntity> StormEventsDatabases { get; set; }
    public virtual DbSet<StormEventsReportEntity> StormEventsReports { get; set; }
    public virtual DbSet<UserCookieConsentLogEntity> UserCookieConsentLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<RadarInventoryEntity>()
            .Property(e => e.FileList)
            .HasColumnType("json");
    }
}

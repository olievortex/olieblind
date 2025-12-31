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
    public virtual DbSet<UserCookieConsentLogEntity> UserCookieConsentLogs { get; set; }
}

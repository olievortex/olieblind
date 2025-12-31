using olieblind.data.Entities;

namespace olieblind.lib.Models;

public class ProductVideoModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string PosterUrl { get; set; } = string.Empty;
    public string VideoUrl { get; set; } = string.Empty;
    public string Transcript { get; set; } = string.Empty;

    public static ProductVideoModel Map(ProductVideoEntity entity)
    {
        return new ProductVideoModel
        {
            Id = entity.Id,
            Title = entity.Title,
            Category = entity.Category,
            PosterUrl = entity.PosterUrl,
            VideoUrl = entity.VideoUrl,
            Transcript = entity.Transcript
        };
    }
}
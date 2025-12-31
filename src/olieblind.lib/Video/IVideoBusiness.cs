using olieblind.lib.Models;

namespace olieblind.lib.Video;

public interface IVideoBusiness
{
    Task<ProductVideoModel?> GetVideoAsync(int id, CancellationToken ct);
    Task<List<ProductVideoModel>> GetVideoListAsync(string category, CancellationToken ct);

    Task<ProductVideoPageModel> GetIndexPageAsync(CancellationToken ct);
}
using olieblind.lib.Models;

namespace olieblind.lib.ForecastModels;

public interface IModelForecastBusiness
{
    Task<ModelForecastIndexModel> GetIndexPageAsync(CancellationToken ct);

    Task<ModelForecastIndexModel?> GetProductAsync(int productId, CancellationToken ct);

    Task<List<ModelForecastIndexModel.HeaderModel>> GetProductListAsync(CancellationToken ct);
}

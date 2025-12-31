using olieblind.data;
using olieblind.lib.Models;

namespace olieblind.lib.ForecastModels;

public class ModelForecastBusiness(IMyRepository repo) : IModelForecastBusiness
{
    public async Task<ModelForecastIndexModel> GetIndexPageAsync(CancellationToken ct)
    {
        var productMap = await repo.ProductMapGetLatest(ct);

        var result = new ModelForecastIndexModel
        {
            Header = ModelForecastIndexModel.HeaderModel.Map(productMap),
            Items = await repo.ProductMapItemList(productMap.Id, ct)
                .ContinueWith(t => t.Result.Select(ModelForecastIndexModel.ItemModel.Map).ToList(), ct)
        };

        return result;
    }

    public async Task<ModelForecastIndexModel?> GetProductAsync(int productId, CancellationToken ct)
    {
        var productMap = await repo.ProductMapGet(productId, ct);
        if (productMap is null) return null;

        var result = new ModelForecastIndexModel
        {
            Header = ModelForecastIndexModel.HeaderModel.Map(productMap),
            Items = (await repo.ProductMapItemList(productMap.Id, ct))
                .Select(ModelForecastIndexModel.ItemModel.Map)
                .ToList()
        };

        return result;
    }

    public async Task<List<ModelForecastIndexModel.HeaderModel>> GetProductListAsync(CancellationToken ct)
    {
        var result = await repo.ProductMapList(ct)
            .ContinueWith(t => t.Result.Select(ModelForecastIndexModel.HeaderModel.Map).ToList(), ct);

        return result;
    }
}

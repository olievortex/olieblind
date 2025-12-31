using Microsoft.AspNetCore.Mvc;
using olieblind.lib.Models;

namespace olieblind.web.Components;

public class ModelForecastTileViewComponent : ViewComponent
{
    public class ViewModel(ModelForecastIndexModel.ItemModel? model)
    {
        public int ParameterId { get; set; } = model?.ParameterId ?? -1;
        public string Title { get; set; } = model?.Title ?? string.Empty;
        public string Url { get; set; } = model?.Url ?? string.Empty;
    };

    public Task<IViewComponentResult> InvokeAsync(ViewModel model)
    {
        return Task.FromResult<IViewComponentResult>(View(model));
    }
}

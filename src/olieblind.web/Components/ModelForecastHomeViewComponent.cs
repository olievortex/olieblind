using Microsoft.AspNetCore.Mvc;
using olieblind.lib.Models;

namespace olieblind.web.Components;

public class ModelForecastHomeViewComponent : ViewComponent
{
    public Task<IViewComponentResult> InvokeAsync(ModelForecastIndexModel model)
    {
        return Task.FromResult<IViewComponentResult>(View(model));
    }
}

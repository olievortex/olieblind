using Microsoft.AspNetCore.Mvc;
using olieblind.lib.Models;

namespace olieblind.web.Components;

public class VideoPageHomeViewComponent : ViewComponent
{
    public Task<IViewComponentResult> InvokeAsync(ProductVideoPageModel model)
    {
        return Task.FromResult<IViewComponentResult>(View(model));
    }
}

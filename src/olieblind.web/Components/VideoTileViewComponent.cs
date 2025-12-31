using Microsoft.AspNetCore.Mvc;
using olieblind.lib.Models;

namespace olieblind.web.Components;

public class VideoTileViewComponent : ViewComponent
{
    public class ViewModel(string category, ProductVideoModel? video, bool showHistory = true)
    {
        public string Category { get; set; } = category;
        public int Id { get; set; } = video?.Id ?? -1;
        public string Title { get; set; } = video?.Title ?? string.Empty;
        public string PosterUri { get; set; } = video?.PosterUrl ?? string.Empty;
        public string VideoUri { get; set; } = video?.VideoUrl ?? string.Empty;
        public bool ShowHistory { get; set; } = showHistory;
    };

    public Task<IViewComponentResult> InvokeAsync(ViewModel model)
    {
        return Task.FromResult<IViewComponentResult>(View(model));
    }
}

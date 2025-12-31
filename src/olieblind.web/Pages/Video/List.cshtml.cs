using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using olieblind.lib.Models;

namespace olieblind.web.Pages.Video;

public class List(IHttpClientFactory httpClientFactory) : PageModel
{
    public List<ProductVideoModel> VideoListModel { get; set; } = [];
    public string Category { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync(string category, CancellationToken ct)
    {
        var httpClient = Program.GetOlieBlue(httpClientFactory);
        var httpResponseMessage = await httpClient.GetAsync($"api/video/list?category={category}", ct);

        if (httpResponseMessage.IsSuccessStatusCode)
        {
            var body = await httpResponseMessage.Content.ReadAsStringAsync(ct);
            VideoListModel = JsonConvert.DeserializeObject<List<ProductVideoModel>>(body) ?? [];
        }

        Category = category;

        return Page();
    }
}
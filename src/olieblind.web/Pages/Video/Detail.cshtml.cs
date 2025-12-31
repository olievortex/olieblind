using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using olieblind.lib.Models;

namespace olieblind.web.Pages.Video;

public class Detail(IHttpClientFactory httpClientFactory) : PageModel
{
    public ProductVideoModel VideoModel { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(string id, CancellationToken ct)
    {
        var httpClient = Program.GetOlieBlue(httpClientFactory);
        var httpResponseMessage = await httpClient.GetAsync($"api/video?id={id}", ct);

        if (httpResponseMessage.IsSuccessStatusCode)
        {
            var body = await httpResponseMessage.Content.ReadAsStringAsync(ct);
            var result = JsonConvert.DeserializeObject<ProductVideoModel>(body);

            if (result is null) return NotFound();

            VideoModel = result;
        }

        return Page();
    }
}
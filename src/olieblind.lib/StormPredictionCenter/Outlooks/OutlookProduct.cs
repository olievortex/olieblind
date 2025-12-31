using olieblind.lib.Services;
using olieblind.lib.StormPredictionCenter.Models;
using System.Drawing;

namespace olieblind.lib.StormPredictionCenter.Outlooks;

public class OutlookProduct(IOlieWebService ows, IOlieImageService ois) : IOutlookProduct
{
    private const string CurrentDayOneBaseUrl = "https://www.spc.noaa.gov/products/outlook/";

    public async Task<string> GetCurrentIndexAsync(int dayNumber, CancellationToken ct)
    {
        var currentIndex = $"day{dayNumber}otlk.html";
        return await ows.ApiGetString($"{CurrentDayOneBaseUrl}{currentIndex}", ct);
    }

    public async Task<byte[]> GetCurrentImageAsync(string imageName, CancellationToken ct)
    {
        return await ows.ApiGetBytes($"{CurrentDayOneBaseUrl}{imageName}", ct);
    }

    public async Task<byte[]> AddBrandingToImageAsync(OutlookBrandingModel branding, byte[] bitmap, Point finalSize,
        CancellationToken ct)
    {
        var result = await ois.ResizeAndAddText(bitmap, finalSize, branding.Category, branding.FontName, branding.FontSize,
            branding.Position, branding.Offset, ct);

        return result;
    }

    public OutlookBrandingModel GetBrandingByImageName(string imageName, string fontName, Point finalSize)
    {
        var ratio = finalSize.Y / 1080.0;
        var parts = Path.GetFileNameWithoutExtension(imageName).Split('_');
        var yPos = (int)(ratio * 5);
        var fontSize = (float)(128.0 * ratio);
        var offset = Math.Min((int)(5 * ratio), 1);

        if (parts.Length == 2 && parts[0].Contains("prob", StringComparison.OrdinalIgnoreCase))
            return new OutlookBrandingModel("Probability", new Point((int)(1150 * ratio), yPos), fontName, fontSize, offset);

        if (parts.Length != 3)
            return
                new OutlookBrandingModel("Categories", new Point((int)(1150 * ratio), yPos), fontName, fontSize, offset);

        return parts[2] switch
        {
            "torn" => new OutlookBrandingModel("Tornado", new Point((int)(1300 * ratio), yPos), fontName, fontSize, offset),
            "wind" => new OutlookBrandingModel("Wind", new Point((int)(1400 * ratio), yPos), fontName, fontSize, offset),
            "hail" => new OutlookBrandingModel("Hail", new Point((int)(1425 * ratio), yPos), fontName, fontSize, offset),
            _ => throw new InvalidOperationException($"Unknown part type {parts[2]}")
        };
    }
}